using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;

namespace AutoLectureRecorder.WPF.DependencyInjection.Factories;

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _services;

    public ViewModelFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Creates a new routable ViewModel using the DI system
    /// </summary>
    /// <returns> An IRoutableViewModel </returns>
    public IRoutableViewModel CreateRoutableViewModel(Type viewModelType)
    {
        if (typeof(IRoutableViewModel).IsAssignableFrom(viewModelType) == false)
        {
            throw new ArgumentException("The provided ViewModel doesn't implement the IRoutableViewModel interface");
        }

        return (IRoutableViewModel)_services.GetRequiredService(viewModelType);
    }
}
