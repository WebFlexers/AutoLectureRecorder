using AutoLectureRecorder.WPF.DependencyInjection;
using AutoLectureRecorder.WPF.Sections.Home;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;

namespace AutoLectureRecorder.WPF;
public partial class App : Application
{
    public AppBootstrapper Bootstrapper { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        Bootstrapper = new AppBootstrapper();
        var services = Bootstrapper.AppHost.Services;

        var window = services.GetRequiredService<MainWindow>();
        window.Show();

        var router = services.GetRequiredService<MainWindowViewModel>().Router;
        router.Navigate.Execute(services.GetRequiredService<HomeViewModel>());
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Bootstrapper.AppHost.StopAsync();

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
