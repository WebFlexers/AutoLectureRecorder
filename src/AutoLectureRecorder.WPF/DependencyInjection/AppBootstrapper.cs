using AutoLectureRecorder.DependencyInjection.Factories;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.Services.WebDriver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.DependencyInjection;

public class AppBootstrapper
{
    public IHost AppHost { get; }

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
        services.AddSingleton(sp => new MainWindow { ViewModel = sp.GetRequiredService<MainWindowViewModel>() });

        // Add Factories
        services.AddTransient<IScreenFactory, ScreenFactory>();
        services.AddTransient<IViewModelFactory, ViewModelFactory>();
        services.AddTransient<IWebDriverFactory, WebDriverFactory>();

        // Add Views and ViewModels
        services.AddViews();
        services.AddViewModels();

        // Add Custom Services
        services.AddFluentValidation();
        services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
        services.AddTransient<IWebDriver, UnipiEdgeWebDriver>();
        services.AddTransient<IStudentAccountRepository, StudentAccountRepository>();
        services.AddTransient<IScheduledLectureRepository, ScheduledLectureRepository>();
    }
}
