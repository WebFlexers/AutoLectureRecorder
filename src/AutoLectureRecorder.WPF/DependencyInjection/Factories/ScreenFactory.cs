using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Sections.MainMenu;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public class ScreenFactory : IScreenFactory
{
    private readonly IServiceProvider _services;

    public ScreenFactory(IServiceProvider services)
    {
        _services = services;
    }

    public MainWindowViewModel GetMainWindowViewModel()
    {
        return _services.GetRequiredService<MainWindowViewModel>();
    }

    public MainMenuViewModel GetMainMenuViewModel()
    {
        return _services.GetRequiredService<MainMenuViewModel>();
    }
}
