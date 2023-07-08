using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Splat;


namespace AutoLectureRecorder.StartupConfiguration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddViews()
            .AddViewModels()
            .AddWindows()
            .AddNavigation();

        services.AddSingleton<IThemeManager, ThemeManager>();
        
        services.AddHttpClient();
        
        return services;
    }

    private static IServiceCollection AddWindows(this IServiceCollection services)
    {
        services.AddSingleton(sp => new MainWindow 
            { ViewModel = sp.GetRequiredService<MainWindowViewModel>() });
        return services;
    }
    
    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        return services;
    }
    
    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddTransient<LoginViewModel>();
        return services;
    }

    private static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        return services;
    }

    public static ILoggingBuilder AddLogging(this ILoggingBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.File(Common.Options.Serilog.Serilog.Sinks.FileLocation, 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        builder.ClearProviders();
        builder.AddSerilog();

        return builder;
    }
}