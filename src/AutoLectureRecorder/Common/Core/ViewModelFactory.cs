using System;
using AutoLectureRecorder.Common.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.Common.Core;

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
    public RoutableViewModel CreateRoutableViewModel(Type viewModelType)
    {
        if (typeof(RoutableViewModel).IsAssignableFrom(viewModelType) == false)
        {
            throw new ArgumentException("The provided ViewModel doesn't implement the IRoutableViewModel interface");
        }

        return (RoutableViewModel)_services.GetRequiredService(viewModelType);
    }
}