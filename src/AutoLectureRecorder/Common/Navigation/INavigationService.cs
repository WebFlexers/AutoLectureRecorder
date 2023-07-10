using System;
using System.Collections.Generic;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Navigation;

public interface INavigationService
{
    void AddNavigationHost(string hostName, RoutingState router);
    Dictionary<string, object>? GetNavigationParameters(Type viewModelType);
    void Navigate(Type viewModelType, string routerHostName);
    void Navigate(Type viewModelType, string routerHostName, Dictionary<string, object> parameters);
    void NavigateAndReset(Type viewModelType, string routerHostName);
    void NavigateAndReset(Type viewModelType, string routerHostName, Dictionary<string, object> parameters);
    void NavigateBack(string routerHostName);
    void NavigateForward(string routerHostName);
}