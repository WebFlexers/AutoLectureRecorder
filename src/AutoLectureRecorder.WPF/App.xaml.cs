using AutoLectureRecorder.WPF.DependencyInjection;
using AutoLectureRecorder.WPF.Sections.Home;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.WPF;

public partial class App : Application
{
    public AppBootstrapper? Bootstrapper { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        var startupWindow = new StartupWindow();
        startupWindow.Show();

        var startTime = Stopwatch.GetTimestamp();

        Bootstrapper = new AppBootstrapper();
        var services = Bootstrapper.AppHost.Services;
        var mainWindow = services.GetRequiredService<MainWindow>();

        var endTime = Stopwatch.GetTimestamp();
        var diff = Stopwatch.GetElapsedTime(startTime, endTime);

        if (diff < TimeSpan.FromSeconds(2.5))
        {
            await Task.Delay(TimeSpan.FromSeconds(2.5).Subtract(diff));
        }

        startupWindow.Close();
        mainWindow.Show();
        var router = services.GetRequiredService<MainWindowViewModel>().Router;
        router.Navigate.Execute(services.GetRequiredService<HomeViewModel>());
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (Bootstrapper != null)
        {
            await Bootstrapper.AppHost.StopAsync();
        }

        base.OnExit(e);
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
