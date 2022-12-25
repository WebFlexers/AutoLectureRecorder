using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.Settings;
public class SettingsViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(SettingsViewModel);
    public IScreen HostScreen { get; }

    public SettingsViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainWindowViewModel();
    }
}
