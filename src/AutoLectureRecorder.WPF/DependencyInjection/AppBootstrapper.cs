using AutoLectureRecorder.DependencyInjection.Factories;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Sections.MainMenu.RecordLectures;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using AutoLectureRecorder.Services.Recording;
using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.Services.WebDriver.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Serilog;

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
            .ConfigureLogging((hostContext, builder) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .CreateLogger();

                builder.ClearProviders();
                builder.AddSerilog();
                Locator.CurrentMutable.UseSerilogFullLogger();
            })
            .Build();

        AppHost.Services.UseMicrosoftDependencyResolver();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Add Microsoft Services
        services.AddHttpClient();

        // Add Windows
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton(sp => new MainWindow { ViewModel = sp.GetRequiredService<MainWindowViewModel>() });
        services.AddTransient<RecordWindowViewModel>();
        services.AddTransient(sp => new RecordWindow { ViewModel = sp.GetRequiredService<RecordWindowViewModel>() });

        // Add Factories
        services.AddTransient<IScreenFactory, ScreenFactory>();
        services.AddTransient<IWindowFactory, WindowFactory>();
        services.AddTransient<IViewModelFactory, ViewModelFactory>();
        services.AddTransient<IValidationFactory, ValidationFactory>();
        services.AddTransient<IWebDriverFactory, WebDriverFactory>();

        // Add Views and ViewModels
        services.AddViews();
        services.AddViewModels();

        // Add Custom Services
        services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
        services.AddTransient<IAlrWebDriver, UnipiEdgeWebDriver>();
        services.AddTransient<IWebDriverDownloader, EdgeWebDriverDownloader>();
        //services.AddTransient<IRecorder, WindowsRecorder>();
        services.AddTransient<IStudentAccountRepository, StudentAccountRepository>();
        services.AddTransient<IScheduledLectureRepository, ScheduledLectureRepository>();
    }
}
