using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AutoLectureRecorder.Application.Common.Abstractions.LecturesSchedule;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
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
            var recorder = _appBootstrapper!.AppHost.Services.GetRequiredService<IRecorder>();
            
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
        
        private async Task SetupLecturesScheduler()
        {
            var mediatorPublisher = _appBootstrapper!.AppHost.Services.GetRequiredService<IPublisher>();
            var lecturesScheduler = _appBootstrapper!.AppHost.Services.GetRequiredService<ILecturesScheduler>();
            
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