using AutoLectureRecorder.Common.Navigation;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Core;

public abstract class RoutableViewModel : ReactiveObject, IRoutableViewModel
{
    public INavigationService NavigationService { get; }
    
    protected RoutableViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public string UrlPathSegment => this.GetType().Name;
    
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public IScreen HostScreen { get; } = null!;
}