using System;
using System.Collections.Generic;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Navigation;

public interface INavigationService
{
    /// <summary>
    /// This is a dictionary that has a type as a key and a dictionary of string and object as value.
    /// The string key of the inner dictionary is the name of the parameter and the object is the parameter value
    /// The reason we are categorizing parameters by type and not deleting them between subsequent navigation
    /// is because when navigating back in the navigation stack we need a way to pass the parameters again if needed.
    /// </summary>
    Dictionary<Type, Dictionary<string, object>> Parameters { get; }

    void AddNavigationHost(string hostName, RoutingState router);
    void Navigate(Type viewModelType, string routerHostName);
    void Navigate(Type vmType, string routerHostName, Dictionary<string, object> parameters);
    void NavigateBack(string routerHostName);
    void NavigateForward(string routerHostName);
}