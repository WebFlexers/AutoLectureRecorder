using ReactiveUI;
using System;

namespace AutoLectureRecorder.DependencyInjection.Factories.Interfaces;

public interface IViewModelFactory
{
    IRoutableViewModel CreateRoutableViewModel(Type viewModelType);
}