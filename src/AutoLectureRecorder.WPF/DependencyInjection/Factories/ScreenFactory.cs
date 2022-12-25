using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;

namespace AutoLectureRecorder.WPF.DependencyInjection.Factories;
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
        throw new NotImplementedException();
    }
}
