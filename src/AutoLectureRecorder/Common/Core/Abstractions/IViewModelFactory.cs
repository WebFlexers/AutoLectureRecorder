using System;
using ReactiveUI;

namespace AutoLectureRecorder.Common.Core.Abstractions;

public interface IViewModelFactory
{
    /// <summary>
    /// Creates a new routable ViewModel using the DI system
    /// </summary>
    IRoutableViewModel CreateRoutableViewModel(Type viewModelType);
}