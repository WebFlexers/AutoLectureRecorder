using ReactiveUI;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public interface IScreenFactory
{
    IScreen GetMainMenuViewModel();
    IScreen GetMainWindowViewModel();
}