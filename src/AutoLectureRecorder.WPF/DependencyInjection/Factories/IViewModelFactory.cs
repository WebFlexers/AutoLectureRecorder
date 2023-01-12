using ReactiveUI;
using System;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public interface IViewModelFactory
{
    IRoutableViewModel CreateRoutableViewModel(Type viewModelType);
}