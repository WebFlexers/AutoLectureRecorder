using ReactiveUI;

namespace AutoLectureRecorder.DependencyInjection.Factories.Interfaces;

public interface IScreenFactory
{
    IScreen GetMainMenuViewModel();
    IScreen GetMainWindowViewModel();
}