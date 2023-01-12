using AutoLectureRecorder.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Upload;

public class UploadViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(UploadViewModel);
    public IScreen HostScreen { get; }

    public UploadViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
