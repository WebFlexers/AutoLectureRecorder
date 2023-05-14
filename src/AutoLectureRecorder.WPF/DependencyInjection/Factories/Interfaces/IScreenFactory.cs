using AutoLectureRecorder.Sections.MainMenu;

namespace AutoLectureRecorder.DependencyInjection.Factories.Interfaces;

public interface IScreenFactory
{
    MainWindowViewModel GetMainWindowViewModel();
    MainMenuViewModel GetMainMenuViewModel();
}