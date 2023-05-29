using AutoLectureRecorder.DependencyInjection;
using AutoLectureRecorder.Sections.Login;
using AutoLectureRecorder.Sections.MainMenu;
using AutoLectureRecorder.Services.DataAccess.Repositories;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Seeding;
using AutoLectureRecorder.Services.Recording;
using AutoLectureRecorder.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace AutoLectureRecorder;

public partial class App : Application
{
    public AppBootstrapper? Bootstrapper { get; private set; }
    private AppMutex _appMutex = new();

    private const string ConnectionString = "Data Source=.\\AutoLectureRecorderDB.db;";

    public App()
    {
        _appMutex.InitializeMutex();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        if (_appMutex.IsAppRunning()) return;

        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        bool isBackgroundRunEnabled = false;
        if (e.Args.Length > 0 && e.Args[0] == "-background")
        {
            isBackgroundRunEnabled = true;
        }

        var sqliteDataAccess = new SqliteDataAccess(ConnectionString);
        bool showStartupWindow = await ShouldShowStartupWindow(sqliteDataAccess);

        // If we are in development populate the database with sample data
        if (Debugger.IsAttached)
        {
            //showStartupWindow = false;
            await new SampleData(sqliteDataAccess).Seed();
        }

        // This is required for the WebView2 to work inside the app
        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9222");

        // Show a startup window as a splash screen while the app loads
        StartupWindow? startupWindow = null;
        long startTime = -1;
        if (showStartupWindow && isBackgroundRunEnabled == false)
        {
            startupWindow = new StartupWindow();
            startupWindow.Show();

            startTime = Stopwatch.GetTimestamp();
        }

        (IServiceProvider services, bool isLoggedIn) = await Task.Run(InitializeBootstrapper);

        if (showStartupWindow && isBackgroundRunEnabled == false)
        {
            var endTime = Stopwatch.GetTimestamp();
            var diff = Stopwatch.GetElapsedTime(startTime, endTime);

            // If the loading takes less that the amount of seconds below
            // wait until at least that amount is over in order to show the
            // beautiful loading screen, along with it's animations
            if (diff < TimeSpan.FromSeconds(2.5))
            {
                await Task.Delay(TimeSpan.FromSeconds(2.5).Subtract(diff));
            }
        }

        var mainWindow = services.GetRequiredService<MainWindow>();

        // Set the main window, to fix a bug that caused the
        // Application.Current.MainWindow to work on debug, but fail in production
        this.MainWindow = mainWindow;

        startupWindow?.Close();

        // Make the window fullscreen if it's dimensions exceed the screen dimensions
        var screen = Screen.FromPoint(new System.Drawing.Point((int)mainWindow.Left, (int)mainWindow.Top));
        if (screen.WorkingArea.Width < mainWindow.Width
            || screen.WorkingArea.Height < mainWindow.Height)
        {
            mainWindow.WindowState = WindowState.Maximized;
        }

        // Navigate to either the Login or the Main Menu screen
        var router = services.GetRequiredService<MainWindowViewModel>().Router;

        if (isLoggedIn)
        {
            router.NavigateAndReset.Execute(services.GetRequiredService<MainMenuViewModel>());
        }
        else
        {
            router.NavigateAndReset.Execute(services.GetRequiredService<LoginViewModel>());
        }

        if (isBackgroundRunEnabled == false)
        {
            mainWindow.Show();
        }

        Task.Run(ObserveShowWindowPipe);
    }

    private async Task<(IServiceProvider services, bool isUserLoggedIn)> 
        InitializeBootstrapper()
    {
        // Load the dependency injection system
        Bootstrapper = new AppBootstrapper();
        var services = Bootstrapper.AppHost.Services;
        await InitializeSettings(services.GetRequiredService<ISettingsRepository>(),
                                 services.GetRequiredService<IRecorder>());

        // Get database access to see if the user is logged in
        var studentAccountData = services.GetRequiredService<IStudentAccountRepository>();
        var studentAccount = await studentAccountData.GetStudentAccount();
        var isLoggedIn = studentAccount != null;

        return (services, isLoggedIn);
    }

    private async Task InitializeSettings(ISettingsRepository settingsRepository, IRecorder recorder)
    {
        var getGeneralSettingsTask = settingsRepository.GetRecordingSettings();
        var getRecordingSettingsTask = settingsRepository.GetRecordingSettings();

        await Task.WhenAll(getGeneralSettingsTask, getRecordingSettingsTask);

        var generalSettings = getGeneralSettingsTask.Result;
        var recordingSettings = getRecordingSettingsTask.Result;

        if (generalSettings == null && recordingSettings == null)
        {
            await settingsRepository.ResetAllSettings(Screen.PrimaryScreen!.Bounds.Width, 
                                                      Screen.PrimaryScreen.Bounds.Height, 
                                                      recorder);
            return;
        }

        if (recordingSettings == null)
        {
            var screen = Screen.PrimaryScreen;
            await settingsRepository.ResetRecordingSettings(screen!.Bounds.Width, screen.Bounds.Height, recorder);
        }

        if (generalSettings == null)
        {
            await settingsRepository.ResetGeneralSettings();
        }
    }

    private async Task<bool> ShouldShowStartupWindow(SqliteDataAccess sqliteDataAccess)
    {
        var settingsRepository = new SettingsRepository(sqliteDataAccess);
        var generalSettings = await settingsRepository.GetGeneralSettings();

        if (generalSettings != null && generalSettings.ShowSplashScreen == false)
        {
            return false;
        }

        return true;
    }

    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        Log.Fatal(ex, "An unhandled exception occurred");
        Log.CloseAndFlush();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (Bootstrapper != null)
        {
            await Bootstrapper.AppHost.StopAsync();
        }

        await Log.CloseAndFlushAsync();

        _appMutex.ReleaseMutex();

        base.OnExit(e);
    }

    private Task ObserveShowWindowPipe()
    {
        while (true)
        {
            using NamedPipeServerStream pipeServer = new NamedPipeServerStream(Pipes.ShowWindowPipe, PipeDirection.In);
            
            pipeServer.WaitForConnection();

            using StreamReader reader = new StreamReader(pipeServer); 
            string message = reader.ReadLine();

            if (message == Pipes.ShowWindowPipe)
            {
                Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = (MainWindow)this.MainWindow;
                    mainWindow?.ViewModel.ShowAppCommand.Execute(mainWindow).Subscribe();
                });
            }
        }
    }
}
