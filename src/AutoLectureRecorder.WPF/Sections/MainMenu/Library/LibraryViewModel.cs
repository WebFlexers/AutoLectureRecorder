using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public class LibraryViewModel : ReactiveObject, IRoutableViewModel
{
    public string UrlPathSegment => nameof(LibraryViewModel);
    public IScreen HostScreen { get; }

    public LibraryViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
