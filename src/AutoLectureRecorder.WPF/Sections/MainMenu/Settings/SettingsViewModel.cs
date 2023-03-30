using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Settings;

public class SettingsViewModel : ReactiveObject, IRoutableViewModel
{
    public string UrlPathSegment => nameof(SettingsViewModel);
    public IScreen HostScreen { get; }

    public SettingsViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
