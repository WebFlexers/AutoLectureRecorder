using AutoLectureRecorder.WPF.Sections.Login;
using AutoLectureRecorder.WPF.Sections.LoginWebView;
using AutoLectureRecorder.WPF.Sections.MainMenu;
using AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;
using AutoLectureRecorder.WPF.Sections.MainMenu.Library;
using AutoLectureRecorder.WPF.Sections.MainMenu.Schedule;
using AutoLectureRecorder.WPF.Sections.MainMenu.Settings;
using AutoLectureRecorder.WPF.Sections.MainMenu.Upload;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.DependencyInjection;

public static class MvvmIServiceCollectionExtensions
{
    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        services.AddTransient<IViewFor<LoginWebViewModel>, LoginWebView>();

        services.AddTransient<IViewFor<MainMenuViewModel>, MainMenuView>();

        services.AddTransient<IViewFor<CreateLectureViewModel>, CreateLectureView>();
        services.AddTransient<IViewFor<DashboardViewModel>, DashboardView>();
        services.AddTransient<IViewFor<LibraryViewModel>, LibraryView>();
        services.AddTransient<IViewFor<ScheduleViewModel>, ScheduleView>();
        services.AddTransient<IViewFor<SettingsViewModel>, SettingsView>();
        services.AddTransient<IViewFor<UploadViewModel>, UploadView>();

        return services;
    }

    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWebViewModel>();

        services.AddScoped<MainMenuViewModel>();

        services.AddTransient<CreateLectureViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<LibraryViewModel>();
        services.AddTransient<ScheduleViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<UploadViewModel>();

        return services;
    }
}
