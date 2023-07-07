using System;
using System.Collections.Generic;
using System.Linq;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Logging;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Navigation;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly Dictionary<string, RoutingState> _routers;

    /// <summary>
    /// This is a dictionary that has a type as a key and a dictionary of string and object as value.
    /// The string key of the inner dictionary is the name of the parameter and the object is the parameter value
    /// The reason we are categorizing parameters by type and not deleting them between subsequent navigation
    /// is because when navigating back in the navigation stack we need a way to pass the parameters again if needed.
    /// </summary>
    public Dictionary<Type, Dictionary<string, object>> Parameters { get; }

    public NavigationService(ILogger<NavigationService> logger, IViewModelFactory viewModelFactory)
    {
        _logger = logger;
        _viewModelFactory = viewModelFactory;
        _routers = new Dictionary<string, RoutingState>();
        Parameters = new Dictionary<Type, Dictionary<string, object>>();
    }

    public void AddNavigationHost(string hostName, RoutingState router)
    {
        _routers[hostName] = router;
    }
    
    // An extra navigation stack that handles forward and back navigation
    // since it doesn't already exist in ReactiveUI.
    // IMPORTANT: It also avoids memory leaks caused by ViewModel instances
    // being directly referenced by the ReactiveUI Router NavigationStack.
    // For that reason NavigateAndReset must always be used instead of Navigate
    // in order for the NavigationStack of the Router to only contain the current ViewModel
    private int _currentNavigationIndex = 0;
    private readonly List<Type> _navigationStack = new();
    
    private void SetRoutedViewHostContent(RoutableViewModel viewModel, string routerHostName)
    {
        var vmType = viewModel.GetType();
        var router = _routers[routerHostName];
        if (router.NavigationStack.LastOrDefault()?.GetType() == vmType) return;

        // Navigate and reset to avoid memory leaks with ViewModels being retained in memory
        router.NavigateAndReset.Execute(viewModel);
        _logger.LogSuccessfulNavigation(vmType.Name);
    }

    public void Navigate(Type viewModelType, string routerHostName)
    {
        if (_currentNavigationIndex < _navigationStack.Count - 1)
        {
            for (int i = _navigationStack.Count - 1; i > _currentNavigationIndex; i--)
            {
                _navigationStack.RemoveAt(i);
            }
        }

        _navigationStack.Add(viewModelType);
        _currentNavigationIndex = _navigationStack.Count - 1;

        SetRoutedViewHostContent(_viewModelFactory.CreateRoutableViewModel(viewModelType), routerHostName);
    }

    public void Navigate(Type vmType, string routerHostName, Dictionary<string, object> parameters)
    {
        Parameters[vmType] = parameters;
        Navigate(vmType, routerHostName);
    }

    public void NavigateBack(string routerHostName)
    {
        var backIndex = _currentNavigationIndex - 1;
        var navigationStackCount = _navigationStack.Count;

        if (navigationStackCount > 1 && backIndex >= 0)
        {
            _currentNavigationIndex = backIndex;
            var viewModelType = _navigationStack.ElementAt(backIndex);
            SetRoutedViewHostContent(_viewModelFactory.CreateRoutableViewModel(viewModelType), routerHostName);
        }
        else
        {
            _logger.LogFailedBackNavigation();
        }
    }

    public void NavigateForward(string routerHostName)
    {
        var forwardIndex = _currentNavigationIndex + 1;
        var navigationStackCount = _navigationStack.Count;

        if (navigationStackCount > 1 && forwardIndex < navigationStackCount)
        {
            _currentNavigationIndex = forwardIndex;
            var viewModelType = _navigationStack.ElementAt(forwardIndex);
            SetRoutedViewHostContent(_viewModelFactory.CreateRoutableViewModel(viewModelType), routerHostName);
        }
        else
        {
            _logger.LogFailedForwardNavigation();
        }
    }
}