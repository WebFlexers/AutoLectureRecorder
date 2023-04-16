using AutoLectureRecorder.Sections.Login;
using AutoLectureRecorder.Sections.LoginWebView;
using AutoLectureRecorder.Sections.MainMenu;
using AutoLectureRecorder.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.Sections.MainMenu.Dashboard;
using AutoLectureRecorder.Sections.MainMenu.Library;
using AutoLectureRecorder.Sections.MainMenu.Schedule;
using AutoLectureRecorder.Sections.MainMenu.Settings;
using AutoLectureRecorder.Sections.MainMenu.Upload;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace AutoLectureRecorder.DependencyInjection;

internal static class MvvmServicesExtensions
{
    internal static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        services.AddTransient<IViewFor<LoginWebViewModel>, LoginWebView>();
        services.AddTransient<IViewFor<MainMenuViewModel>, MainMenuView>();
        services.AddTransient<IViewFor<CreateLectureViewModel>, CreateLectureView>();
        services.AddTransient<IViewFor<DashboardViewModel>, DashboardView>();
        services.AddTransient<IViewFor<LibraryViewModel>, LibraryView>();
        services.AddTransient<IViewFor<ScheduleViewModel>, ScheduleView>();
        services.AddTransient<IViewFor<ScheduledLectureViewModel>, ScheduledLectureView>();
        services.AddTransient<IViewFor<SettingsViewModel>, SettingsView>();
        services.AddTransient<IViewFor<UploadViewModel>, UploadView>();

        return services;
    }

    internal static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWebViewModel>();
        services.AddSingleton<MainMenuViewModel>();
        services.AddTransient<CreateLectureViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddTransient<LibraryViewModel>();
        services.AddTransient<ScheduleViewModel>();
        services.AddTransient<ScheduledLectureViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<UploadViewModel>();

        return services;
    }
}
