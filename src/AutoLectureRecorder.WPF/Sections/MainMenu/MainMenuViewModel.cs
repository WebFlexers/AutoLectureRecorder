using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.MainMenu;

public class MainMenuViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(MainMenuViewModel);
    public IScreen HostScreen { get; }

    public MainMenuViewModel(IScreenFactory hostScreen)
    {
        
    }
}
