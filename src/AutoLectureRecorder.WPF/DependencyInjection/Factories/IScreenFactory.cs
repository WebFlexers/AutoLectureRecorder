using ReactiveUI;

namespace AutoLectureRecorder.WPF.DependencyInjection.Factories;

public interface IScreenFactory
{
    IScreen GetMainMenuViewModel();
    IScreen GetMainWindowViewModel();
}