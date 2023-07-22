﻿using AutoLectureRecorder.Common.Navigation;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Core;

public class RoutableViewModelHost : ReactiveObject, IRoutableViewModel, IScreen
{
    public INavigationService NavigationService { get; }
    
    public RoutingState Router { get; }
    public string UrlPathSegment => this.GetType().Name;
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public IScreen HostScreen { get; } = null!;

    protected RoutableViewModelHost(INavigationService navigationService)
    {
        NavigationService = navigationService;
        
        var hostName = this.GetType().Name;
        Router = new RoutingState();
        NavigationService.AddNavigationHost(hostName, Router);
    }
}