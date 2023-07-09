using System;
using System.Collections.Generic;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Navigation;

public interface INavigationService
{
    void AddNavigationHost(string hostName, RoutingState router);
    Dictionary<string, object>? GetNavigationParameters(Type vmType);
    void Navigate(Type viewModelType, string routerHostName);
    void Navigate(Type vmType, string routerHostName, Dictionary<string, object> parameters);
    void NavigateAndReset(Type vmType, string routerHostName);
    void NavigateAndReset(Type vmType, string routerHostName, Dictionary<string, object> parameters);
    void NavigateBack(string routerHostName);
    void NavigateForward(string routerHostName);
}