using System;
using System.Collections.Generic;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Navigation;

public interface INavigationService
{
    void AddNavigationHost(string hostName, RoutingState router);
    void RemoveNavigationHost(string hostName);
    Dictionary<string, object>? GetNavigationParameters(Type viewModelType);
    void AddNavigationParameters(Type viewModelType, Dictionary<string, object> parameters);
    void Navigate(Type viewModelType, string routerHostName);
    void Navigate(Type viewModelType, string routerHostName, Dictionary<string, object> parameters);
    void NavigateAndReset(Type viewModelType, string routerHostName);
    void NavigateAndReset(Type viewModelType, string routerHostName, Dictionary<string, object> parameters);
    void NavigateBack(string routerHostName);
    void NavigateForward(string routerHostName);
}