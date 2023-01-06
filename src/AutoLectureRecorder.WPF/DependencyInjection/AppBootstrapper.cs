using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.WPF.DependencyInjection;

public class AppBootstrapper
{
    public IHost AppHost { get; private set; }

    public AppBootstrapper()
    {
        AppHost = Host.CreateDefaultBuilder()
           .ConfigureServices((hostContext, services) =>
           {
               // Override splat internal DI with Microsoft built-in dependency injection
               services.UseMicrosoftDependencyResolver();
               Locator.CurrentMutable.InitializeSplat();
               Locator.CurrentMutable.InitializeReactiveUI();

               // Configure the rest of the Dependency Injection
               ConfigureServices(services);
           })
           .Build();

        AppHost.Services.UseMicrosoftDependencyResolver();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Add Main Window and Routing
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>(sp => new MainWindow { ViewModel = sp.GetRequiredService<MainWindowViewModel>() });

        // Add Factories
        services.AddTransient<IScreenFactory, ScreenFactory>();
        services.AddTransient<IViewModelFactory, ViewModelFactory>();
        services.AddTransient<IWebDriverFactory, WebDriverFactory>();

        // Add Views and ViewModels
        services.AddViews();
        services.AddViewModels();

        // Add Custom Services
        services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
        services.AddTransient<IWebDriver, UnipiEdgeWebDriver>();
        services.AddTransient<IStudentAccountData, StudentAccountData>();
    }
}
