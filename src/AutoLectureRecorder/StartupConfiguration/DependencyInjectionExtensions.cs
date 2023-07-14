using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Core.Abstractions;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.Pages.MainMenu;
using AutoLectureRecorder.Pages.MainMenu.CreateLecture;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using FluentValidation;
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

        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
        
        services.AddSingleton<IThemeManager, ThemeManager>();
        
        services.AddHttpClient();
        
        return services;
    }

    private static IServiceCollection AddWindows(this IServiceCollection services)
    {
        services.AddSingleton(sp => new MainWindow 
            { ViewModel = sp.GetRequiredService<MainWindowViewModel>() });

        services.AddTransient<IWindowFactory, WindowFactory>();
        
        return services;
    }
    
    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        services.AddTransient<IViewFor<LoginWebViewModel>, LoginWebView>();
        services.AddTransient<IViewFor<MainMenuViewModel>, MainMenuView>();
        services.AddTransient<IViewFor<CreateLectureViewModel>, CreateLectureView>();
        return services;
    }
    
    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<IViewModelFactory, ViewModelFactory>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWebViewModel>();
        services.AddTransient<MainMenuViewModel>();
        services.AddTransient<CreateLectureViewModel>();
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
            .WriteTo.File(Common.Options.Serilog.Serilog.Sinks.FileLocation, 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        builder.ClearProviders();
        builder.AddSerilog();
        builder.AddDebug();

        return builder;
    }
}