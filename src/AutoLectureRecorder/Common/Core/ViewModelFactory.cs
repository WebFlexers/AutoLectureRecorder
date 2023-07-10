using System;
using AutoLectureRecorder.Common.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

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
    public IRoutableViewModel CreateRoutableViewModel(Type viewModelType)
    {
        if (typeof(IRoutableViewModel).IsAssignableFrom(viewModelType) == false)
        {
            throw new ArgumentException("The provided ViewModel doesn't implement the IRoutableViewModel interface");
        }

        return (IRoutableViewModel)_services.GetRequiredService(viewModelType);
    }
}