using AutoLectureRecorder.WPF.Sections.Login;
using AutoLectureRecorder.WPF.Sections.MainMenu;
using AutoLectureRecorder.WPF.Sections.LoginWebView;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        services.AddTransient<IViewFor<LoginWebViewModel>, LoginWebView>();
        services.AddTransient<IViewFor<MainMenuViewModel>, MainMenuView>();

        return services;
    }

    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWebViewModel>();
        services.AddTransient<MainMenuViewModel>();

        return services;
    }
}
