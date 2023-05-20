using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using AutoLectureRecorder.Sections.MainMenu.Library;

namespace AutoLectureRecorder.DependencyInjection.Factories;

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

    /// <summary>
    /// Creates the view model that corresponds to the Recorded Lectures View from
    /// the provided ReactiveScheduledLecture
    /// </summary>
    public async Task<RecordedLecturesViewModel> CreateRecordedLecturesViewModel(int scheduledLectureId)
    {
        var viewModel = _services.GetRequiredService<RecordedLecturesViewModel>();
        await viewModel.Initialize(scheduledLectureId);
        return viewModel;
    }
}
