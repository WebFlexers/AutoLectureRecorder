using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.Upload;

public class UploadViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(UploadViewModel);
    public IScreen HostScreen { get; }

    public UploadViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
