using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Core.Abstractions;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.Pages.MainMenu;
using AutoLectureRecorder.Pages.MainMenu.CreateLecture;
using AutoLectureRecorder.Pages.MainMenu.Dashboard;
using AutoLectureRecorder.Pages.MainMenu.Library;
using AutoLectureRecorder.Pages.MainMenu.Library.Services;
using AutoLectureRecorder.Pages.MainMenu.Schedule;
using AutoLectureRecorder.Pages.MainMenu.Settings;
using AutoLectureRecorder.Pages.MainMenu.Upload;
using AutoLectureRecorder.Pages.RecordLecture;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Splat;
using Splat.Serilog;


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

        // This is here instead of the infrastructure layer, because it has dependencies that
        // are not available to class libraries
        services.AddTransient<IVideoInformationRetriever, VideoInformationRetriever>();
        
        services.AddSingleton<IThemeManager, ThemeManager>();

        return services;
    }

    private static IServiceCollection AddWindows(this IServiceCollection services)
    {
        services.AddSingleton(sp => new MainWindow 
            { ViewModel = sp.GetRequiredService<MainWindowViewModel>() });
        
        services.AddTransient(sp => new RecordWindow
            { ViewModel = sp.GetRequiredService<RecordWindowViewModel>() });

        services.AddTransient<IWindowFactory, WindowFactory>();
        
        return services;
    }
    
    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        services.AddTransient<IViewFor<LoginWebViewModel>, LoginWebView>();
        services.AddTransient<IViewFor<MainMenuViewModel>, MainMenuView>();
        services.AddTransient<IViewFor<DashboardViewModel>, DashboardView>();
        services.AddTransient<IViewFor<ScheduleViewModel>, ScheduleView>();
        services.AddTransient<IViewFor<CreateLectureViewModel>, CreateLectureView>();
        services.AddTransient<IViewFor<UploadViewModel>, UploadView>();
        services.AddTransient<IViewFor<LibraryViewModel>, LibraryView>();
        services.AddTransient<IViewFor<RecordedLecturesViewModel>, RecordedLecturesView>();
        services.AddTransient<IViewFor<SettingsViewModel>, SettingsView>();
        return services;
    }
    
    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        
        services.AddTransient<RecordWindowViewModel>();
        services.AddTransient<IViewModelFactory, ViewModelFactory>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWebViewModel>();
        services.AddTransient<MainMenuViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ScheduleViewModel>();
        services.AddTransient<CreateLectureViewModel>();
        services.AddTransient<UploadViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<LibraryViewModel>();
        services.AddTransient<RecordedLecturesViewModel>();
        return services;
    }

    private static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        return services;
    }

    public static ILoggingBuilder AddLogging(this ILoggingBuilder builder)
    {
        builder.ClearProviders();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .WriteTo.Console(levelSwitch: new LoggingLevelSwitch(LogEventLevel.Debug))
            .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Debug)
            .WriteTo.File(Common.Options.Serilog.Serilog.Sinks.FileLocation, 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                restrictedToMinimumLevel: LogEventLevel.Information)
            .Filter.ByExcluding(Matching.FromSource("ReactiveUI.POCOObservableForProperty"))
            .CreateLogger();

        builder.AddSerilog();

        Locator.CurrentMutable.UseSerilogFullLogger();

        return builder;
    }
}