using AutoLectureRecorder.Common.Navigation;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Core;

public class NavigationHostViewModel : ReactiveObject, IScreen
{
    public INavigationService NavigationService { get; }

    public RoutingState Router { get; }
    
    public NavigationHostViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        
        var hostName = this.GetType().Name;
        Router = new RoutingState();
        NavigationService.AddNavigationHost(hostName, Router);
    }
}