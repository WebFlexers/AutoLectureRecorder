using AutoLectureRecorder.Sections.MainMenu.Library;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace AutoLectureRecorder.DependencyInjection.Factories.Interfaces;

public interface IViewModelFactory
{
    IRoutableViewModel CreateRoutableViewModel(Type viewModelType);
    Task<RecordedLecturesViewModel> CreateRecordedLecturesViewModel(int scheduledLectureId);
}