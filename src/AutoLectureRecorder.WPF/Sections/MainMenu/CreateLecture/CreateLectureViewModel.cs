using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;

public class CreateLectureViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(CreateLectureViewModel);
    public IScreen HostScreen { get; }

    public CreateLectureViewModel(IScreenFactory hostScreen)
    {
        HostScreen = hostScreen.GetMainMenuViewModel();
    }
}
