using AutoLectureRecorder.WPF.Sections.Home;
using AutoLectureRecorder.WPF.Sections.Login;
using AutoLectureRecorder.WPF.Sections.MainMenu;
using AutoLectureRecorder.WPF.Sections.Settings;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<HomeViewModel>, HomeView>();
        services.AddTransient<IViewFor<SettingsViewModel>, SettingsView>();
        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        services.AddTransient<IViewFor<MainMenuViewModel>, MainMenuView>();

        return services;
    }

    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainMenuViewModel>();

        return services;
    }
}
