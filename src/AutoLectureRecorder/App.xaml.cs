using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Windows;
using AutoLectureRecorder.Application.Options;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.WindowsServices;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.StartupConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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

        protected override void OnStartup(StartupEventArgs e)
        {
            if (_appMutex.IsAppRunning()) return;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            bool isBackgroundRunEnabled = e.Args.Length > 0 && e.Args[0] == "-background";

            // This is required for the WebView2 to work inside the app
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", 
                WebView.BrowserArguments.RemoteDebuggingPortArgument);

            _appBootstrapper = new AppBootstrapper();
            var services = _appBootstrapper.AppHost.Services;

            var mainWindow = services.GetRequiredService<MainWindow>();
            this.MainWindow = mainWindow;

            var mainWindowViewModel = services.GetRequiredService<MainWindowViewModel>();
            mainWindowViewModel.NavigationService.Navigate(typeof(LoginViewModel), HostNames.MainWindowHost);
            
            mainWindow.Show();

            var observeShowWindowTask = Task.Run(() => ObserveShowWindowPipe());
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

                        // mainWindow?.ViewModel.ShowAppCommand.Execute(mainWindow).Subscribe();
                    });
                }
            }
        }
    }
}