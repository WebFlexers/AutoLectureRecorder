using ReactiveUI;
using System;

namespace AutoLectureRecorder.WPF.DependencyInjection.Factories;

public interface IViewModelFactory
{
    IRoutableViewModel CreateRoutableViewModel(Type viewModelType);
}