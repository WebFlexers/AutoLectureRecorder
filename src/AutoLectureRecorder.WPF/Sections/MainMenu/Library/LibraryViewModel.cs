using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.Library;

public class LibraryViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(LibraryViewModel);
    public IScreen HostScreen { get; }

    public LibraryViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
