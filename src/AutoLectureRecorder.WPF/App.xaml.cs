using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.WPF.DependencyInjection;
using AutoLectureRecorder.WPF.Sections.Login;
using AutoLectureRecorder.WPF.Sections.MainMenu;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace AutoLectureRecorder.WPF;

public partial class App : Application
{
    public AppBootstrapper? Bootstrapper { get; private set; }

    private Mutex mutex;
    private bool mutexCreated;

    public App()
    {
        string mutexId = $"Global\\{GetType().GUID}";

        MutexAccessRule allowEveryoneRule = new MutexAccessRule(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            MutexRights.FullControl,
            AccessControlType.Allow);
        MutexSecurity securitySettings = new MutexSecurity();
        securitySettings.AddAccessRule(allowEveryoneRule);

        // initiallyOwned: true == false + mutex.WaitOne()
        mutex = new Mutex(initiallyOwned: true, mutexId, out mutexCreated);
        mutex.SetAccessControl(securitySettings);
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (!mutexCreated)
        {
            //System.Windows.MessageBox.Show("AutoLectureRecorder is already running!", "Initialization Failed", 
            //                                MessageBoxButton.OK, MessageBoxImage.Error);
            //Process.GetProcessesByName()
            Application.Current.Shutdown();
        }

        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9222");

        bool showStartupWindow = false;

        StartupWindow startupWindow = null;
        long startTime = -1;
        if (showStartupWindow)
        {
            startupWindow = new StartupWindow();
            startupWindow.Show();

            startTime = Stopwatch.GetTimestamp();
        }

        Bootstrapper = new AppBootstrapper();
        var services = Bootstrapper.AppHost.Services;
        var mainWindow = services.GetRequiredService<MainWindow>();
        var studentAccountData = services.GetRequiredService<IStudentAccountData>();
        var studentAccount = await studentAccountData.GetStudentAccountAsync()!;
        var isLoggedIn = studentAccount != null;

        if (showStartupWindow)
        {
            var endTime = Stopwatch.GetTimestamp();
            var diff = Stopwatch.GetElapsedTime(startTime, endTime);

            if (diff < TimeSpan.FromSeconds(2.5))
            {
                await Task.Delay(TimeSpan.FromSeconds(2.5).Subtract(diff));
            }

            startupWindow!.Close();
        }

        mainWindow.Show();

        Screen screen = Screen.FromPoint(new System.Drawing.Point((int)mainWindow.Left, (int)mainWindow.Top));
        if (screen.WorkingArea.Width < mainWindow.Width
            || screen.WorkingArea.Height < mainWindow.Height)
        {
            mainWindow.WindowState = WindowState.Maximized;
        }

        var router = services.GetRequiredService<MainWindowViewModel>().Router;

        if (isLoggedIn)
        {
            router.Navigate.Execute(services.GetRequiredService<MainMenuViewModel>());
        }
        else
        {
            router.Navigate.Execute(services.GetRequiredService<LoginViewModel>());
        }
        
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (Bootstrapper != null)
        {
            await Bootstrapper.AppHost.StopAsync();
        }

        base.OnExit(e);

        if (mutexCreated)
        {
            try
            {
                mutex.ReleaseMutex();
            }
            catch (ApplicationException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, ex.GetType().FullName, 
                                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        mutex.Dispose();
    }

    public Style? GetStyleFromResourceDictionary(string styleName, string resourceDictionaryName)
    {
        var titleBarResources = new ResourceDictionary();
        titleBarResources.Source = new Uri($"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/{resourceDictionaryName}",
                        UriKind.RelativeOrAbsolute);
        return titleBarResources[styleName] as Style;
    }

    public ResourceDictionary? GetResourceDictionary(string resourceDictionaryName, string relativePath)
    {
        var titleBarResources = new ResourceDictionary();
        titleBarResources.Source = new Uri(
            $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/{relativePath}/{resourceDictionaryName}",
            UriKind.RelativeOrAbsolute);
        return titleBarResources;
    }
}
