using AutoLectureRecorder.DependencyInjection;
using AutoLectureRecorder.Sections.Login;
using AutoLectureRecorder.Sections.MainMenu;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Seeding;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AutoLectureRecorder.Resources.Themes;
using Application = System.Windows.Application;

namespace AutoLectureRecorder;

public partial class App : Application
{
    public AppBootstrapper? Bootstrapper { get; private set; }

    private readonly Mutex _mutex;
    private readonly bool _mutexCreated;

    public App()
    {
        // Use a global mutex to stop the app from running more than once
        string mutexId = $"Global\\{GetType().GUID}";

        MutexAccessRule allowEveryoneRule = new(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            MutexRights.FullControl,
            AccessControlType.Allow);
        MutexSecurity securitySettings = new();
        securitySettings.AddAccessRule(allowEveryoneRule);

        _mutex = new Mutex(initiallyOwned: true, mutexId, out _mutexCreated);
        _mutex.SetAccessControl(securitySettings);
    }

    //[DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")] 
    //public static extern bool ShouldSystemUseDarkMode();

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (_mutexCreated == false)
        {
            Application.Current.Shutdown();
        }

        // If we are in development populate the database with sample data
        if (Debugger.IsAttached)
        {
            await new SampleData(
                    new SqliteDataAccess("Data Source=.\\AutoLectureRecorderDB.db;"))
                        .Seed();
        }

        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        // This is required for the WebView2 to work inside the app
        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9222");

        bool showStartupWindow = false;

        // Show a startup window as a loading screen while the app loads
        StartupWindow? startupWindow = null;
        long startTime = -1;
        if (showStartupWindow)
        {
            startupWindow = new StartupWindow();
            startupWindow.Show();

            startTime = Stopwatch.GetTimestamp();
        }

        // Load the dependency injection system
        Bootstrapper = new AppBootstrapper();
        var services = Bootstrapper.AppHost.Services;
        var mainWindow = services.GetRequiredService<MainWindow>();

        // Get database access to see if the user is logged in
        var studentAccountData = services.GetRequiredService<IStudentAccountRepository>();
        var studentAccount = await studentAccountData.GetStudentAccountAsync()!;
        var isLoggedIn = studentAccount != null;

        if (showStartupWindow)
        {
            //var endTime = Stopwatch.GetTimestamp();
            //var diff = Stopwatch.GetElapsedTime(startTime, endTime);

            //// If the loading takes less that the amount of seconds below
            //// wait until at least that amount is over in order to show the
            //// beautiful loading screen, along with it's animations
            //if (diff < TimeSpan.FromSeconds(2.5))
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(2.5).Subtract(diff));
            //}

            //startupWindow!.Close();
        }

        // Set the main window, to fix a bug that caused the
        // Application.Current.MainWindow to work on debug, but fail in production
        this.MainWindow = mainWindow;

        mainWindow.Show();

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

        base.OnExit(e);

        // Release the mutex OnExit
        if (_mutexCreated)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch (ApplicationException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, ex.GetType().FullName, 
                                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        _mutex.Dispose();
    }

    // Access to app Styles
    public static Style? GetStyleFromResourceDictionary(string styleName, string resourceDictionaryName)
    {
        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri($"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/{resourceDictionaryName}",
                UriKind.RelativeOrAbsolute)
        };
        return resourceDictionary[styleName] as Style;
    }

    private static ResourceDictionary? _resourceDictionary;
    private static ColorTheme? _currentDictionaryTheme;

    public static ResourceDictionary GetCurrentThemeDictionary()
    {
        if (_currentDictionaryTheme != null && _currentDictionaryTheme == ThemeManager.CurrentColorTheme)
        {
            return _resourceDictionary!;
        }

        if (_currentDictionaryTheme == null)
        {
            _currentDictionaryTheme = ThemeManager.CurrentColorTheme;
        }

        if (ThemeManager.CurrentColorTheme == ColorTheme.Light)
        {
            _resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(
                    $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/LightTheme.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }
        else
        {
            _resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(
                    $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/DarkTheme.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }

        return _resourceDictionary;
    }
}
