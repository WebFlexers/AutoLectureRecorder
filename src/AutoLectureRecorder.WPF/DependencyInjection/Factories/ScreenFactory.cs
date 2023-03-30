using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Sections.MainMenu;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public class ScreenFactory : IScreenFactory
{
    private readonly IServiceProvider _services;

    public ScreenFactory(IServiceProvider services)
    {
        _services = services;
    }

    public IScreen GetMainWindowViewModel()
    {
        return _services.GetRequiredService<MainWindowViewModel>();
    }

    public IScreen GetMainMenuViewModel()
    {
        return _services.GetRequiredService<MainMenuViewModel>();
    }
}
