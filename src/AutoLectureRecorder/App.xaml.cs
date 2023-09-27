using System;
using System.Collections.Generic;
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
using AutoLectureRecorder.Common.Core.Abstractions;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Common.WindowsServices;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.Pages.MainMenu;
using AutoLectureRecorder.Pages.RecordLecture;
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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            
            if (_appMutex.IsAppRunning()) return;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            
            bool isBackgroundRunEnabled = e.Args.Length > 0 && e.Args.Contains("-background");

            // This is required for the WebView2 to work inside the app
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", 
                WebViewOptions.Network.RemoteDebuggingPortArgument);

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
                MakeWindowFullScreenOnTooLargeScreen();
            }

            var observeShowWindowTask = Task.Run(() => ObserveShowWindowPipe());
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Log.Fatal(ex, "An unhandled exception occurred");
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Make the window fullscreen if it's dimensions exceed the screen dimensions
        /// </summary>
        public void MakeWindowFullScreenOnTooLargeScreen()
        {
            var screen = Screen.FromPoint(new System.Drawing.Point((int)this.MainWindow!.Left, (int)this.MainWindow.Top));
            if (screen.WorkingArea.Width < this.MainWindow.Width
                || screen.WorkingArea.Height < this.MainWindow.Height)
            {
                this.MainWindow.WindowState = WindowState.Maximized;
            }
        }
        
        private ReactiveCommand<ReactiveScheduledLecture?, Unit> NavigateToRecordLecture => ReactiveCommand
            .CreateFromTask<ReactiveScheduledLecture?, Unit>(async reactiveScheduledLecture =>
        {
            if (reactiveScheduledLecture is null)
            {
                Log.Logger.Error("Failed to start recording window, because the lecture was null");
                return Unit.Default;
            }
            
            Log.Logger.Information("Lecture starting...");

            var mediator = _appBootstrapper!.AppHost.Services.GetRequiredService<IMediator>();
            var windowFactory = _appBootstrapper!.AppHost.Services.GetRequiredService<IWindowFactory>();
            var navigationService = _appBootstrapper!.AppHost.Services.GetRequiredService<INavigationService>();
            
            var navigationParameters = new Dictionary<string, object>()
            {
                { NavigationParameters.RecordLecture.LectureToRecord, reactiveScheduledLecture }
            };
            navigationService.AddNavigationParameters(typeof(RecordWindowViewModel), navigationParameters);

            var recordingWindow = windowFactory.CreateRecordWindow();
            recordingWindow.Show();
            
            await mediator.Publish(new NextScheduledLectureEvent());

            return Unit.Default;
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
            var recordingDirectories = recordingDirectoriesTask.Result?.ToArray();

            var screen = Screen.PrimaryScreen;
            if (recordingDirectories is null || recordingDirectories.Any() == false)
            {
                var defaultRecordingSettings = recorder.GetDefaultSettings(screen!.Bounds.Width, screen.Bounds.Height);
                await recordingsRepository.AddRecordingDirectory(defaultRecordingSettings.RecordingsLocalPath);
            }
            
            if (generalSettings == null && recordingSettings == null)
            {
                await settingsRepository.ResetAllSettings(Screen.PrimaryScreen!.Bounds.Width, 
                    Screen.PrimaryScreen.Bounds.Height, 
                    recorder);
                return;
            }
            
            if (recordingSettings is null)
            {
                await settingsRepository.ResetRecordingSettings(screen!.Bounds.Width, screen.Bounds.Height, recorder);
            }

            if (generalSettings is null)
            {
                await settingsRepository.ResetGeneralSettings();
            }
        }
        
        private async Task SetupLecturesScheduler()
        {
            var mediator = _appBootstrapper!.AppHost.Services.GetRequiredService<IMediator>();
            var lecturesScheduler = _appBootstrapper.AppHost.Services.GetRequiredService<ILecturesScheduler>();
            
            await mediator.Publish(new NextScheduledLectureEvent());
            
            lecturesScheduler.NextScheduledLectureWillBegin
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(nextLectureWillBegin => nextLectureWillBegin)
                .Select(_ => lecturesScheduler.NextScheduledLecture)
                .InvokeCommand(NavigateToRecordLecture);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _appBootstrapper?.AppHost.StopAsync()!;

            await Log.CloseAndFlushAsync();
            
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
            
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