using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AutoLectureRecorder.Application.Common.Abstractions.LecturesSchedule;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Application.Common.Abstractions.SampleData;
using AutoLectureRecorder.Application.Common.Options;
using AutoLectureRecorder.Application.ScheduledLectures.Events;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.WindowsServices;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.Pages.MainMenu;
using AutoLectureRecorder.StartupConfiguration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder
{
    public partial class App : System.Windows.Application
    {
        private AppBootstrapper? _appBootstrapper;
        private readonly AppMutex _appMutex;
        
        public App()
        {
            _appMutex = new AppMutex();
            _appMutex.InitializeMutex();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (_appMutex.IsAppRunning()) return;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            // TODO: Make alr actually background on startup
            bool isBackgroundRunEnabled = e.Args.Length > 0 && e.Args[0] == "-background";

            // This is required for the WebView2 to work inside the app
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", 
                WebViewOptions.BrowserArguments.RemoteDebuggingPortArgument);

            _appBootstrapper = new AppBootstrapper();
            var services = _appBootstrapper.AppHost.Services;

            if (Debugger.IsAttached)
            {
                var sampleData = services.GetRequiredService<ISampleData>();
                await sampleData.Seed();
            }

            var mainWindow = services.GetRequiredService<MainWindow>();
            this.MainWindow = mainWindow;

            var studentRepository = services.GetRequiredService<IStudentAccountRepository>();
            bool isLoggedIn = await studentRepository.GetStudentAccount() is not null;

            var mainWindowViewModel = services.GetRequiredService<MainWindowViewModel>();

            mainWindowViewModel.NavigationService.Navigate(
                isLoggedIn ? typeof(MainMenuViewModel) 
                           : typeof(LoginViewModel), HostNames.MainWindowHost);

            var initializeSettingsTask = InitializeSettings();
            var setupLecturesSchedulerTask = SetupLecturesScheduler();

            await Task.WhenAll(initializeSettingsTask, setupLecturesSchedulerTask);
            
            if (isBackgroundRunEnabled == false)
            {
                mainWindow.Show();
                
                // Make the window fullscreen if it's dimensions exceed the screen dimensions
                var screen = Screen.FromPoint(new System.Drawing.Point((int)mainWindow.Left, (int)mainWindow.Top));
                if (screen.WorkingArea.Width < mainWindow.Width
                    || screen.WorkingArea.Height < mainWindow.Height)
                {
                    mainWindow.WindowState = WindowState.Maximized;
                }
            }

            var observeShowWindowTask = Task.Run(() => ObserveShowWindowPipe());
        }

        private ReactiveCommand<Unit, Unit> NavigateToRecordLecture => ReactiveCommand.CreateFromTask(async () =>
        {
            Log.Logger.Information("Lecture starting...");
            var mediatorPublisher = _appBootstrapper!.AppHost.Services.GetRequiredService<IPublisher>();
            await mediatorPublisher.Publish(new NextScheduledLectureEvent());
        });

        private async Task InitializeSettings()
        {
            var settingsRepository = _appBootstrapper!.AppHost.Services.GetRequiredService<ISettingsRepository>();
            var recordingsRepository = _appBootstrapper.AppHost.Services.GetRequiredService<IRecordingsRepository>();
            var recorder = _appBootstrapper!.AppHost.Services.GetRequiredService<IRecorder>();
            
            var getGeneralSettingsTask = settingsRepository.GetRecordingSettings();
            var getRecordingSettingsTask = settingsRepository.GetRecordingSettings();
            var recordingDirectoriesTask = recordingsRepository.GetAllRecordingDirectories();

            await Task.WhenAll(getGeneralSettingsTask, getRecordingSettingsTask, recordingDirectoriesTask);

            var generalSettings = getGeneralSettingsTask.Result;
            var recordingSettings = getRecordingSettingsTask.Result;
            var recordingDirectories = recordingDirectoriesTask.Result;

            if (generalSettings == null && recordingSettings == null)
            {
                await settingsRepository.ResetAllSettings(Screen.PrimaryScreen!.Bounds.Width, 
                    Screen.PrimaryScreen.Bounds.Height, 
                    recorder);
                return;
            }

            var screen = Screen.PrimaryScreen;
            if (recordingSettings is null)
            {
                await settingsRepository.ResetRecordingSettings(screen!.Bounds.Width, screen.Bounds.Height, recorder);
            }

            if (generalSettings is null)
            {
                await settingsRepository.ResetGeneralSettings();
            }

            if (recordingDirectories is null || recordingDirectories.Any() == false)
            {
                var defaultRecordingSettings = recorder.GetDefaultSettings(screen!.Bounds.Width, screen.Bounds.Height);
                await recordingsRepository.AddRecordingDirectory(defaultRecordingSettings.RecordingsLocalPath);
            }
        }
        
        private async Task SetupLecturesScheduler()
        {
            var mediatorPublisher = _appBootstrapper!.AppHost.Services.GetRequiredService<IPublisher>();
            var lecturesScheduler = _appBootstrapper.AppHost.Services.GetRequiredService<ILecturesScheduler>();
            
            await mediatorPublisher.Publish(new NextScheduledLectureEvent());
            lecturesScheduler.NextScheduledLectureWillBegin
                .Where(nextLectureWillBegin => nextLectureWillBegin)
                .Select(_ => Unit.Default)
                .InvokeCommand(NavigateToRecordLecture);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_appBootstrapper is not null)
            {
                await _appBootstrapper.AppHost.StopAsync();
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
                string message = reader.ReadLine()!;

                if (message == Pipes.ShowWindowPipe)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        var mainWindow = (MainWindow)this.MainWindow!;
                        mainWindow?.ViewModel!.ShowAppCommand.Execute(mainWindow).Subscribe();
                    });
                }
            }
        }
    }
}