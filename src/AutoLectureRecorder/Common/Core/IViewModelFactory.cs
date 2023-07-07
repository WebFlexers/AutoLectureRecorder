using System;

namespace AutoLectureRecorder.Common.Core;

public interface IViewModelFactory
{
    /// <summary>
    /// Creates a new routable ViewModel using the DI system
    /// </summary>
    RoutableViewModel CreateRoutableViewModel(Type viewModelType);
}