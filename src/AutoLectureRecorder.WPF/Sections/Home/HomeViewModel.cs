using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.WPF.Sections.Home;
public class HomeViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(HomeViewModel);
    public IScreen HostScreen { get; }

    [Reactive]
    public string? MessageText { get; set; }

    public HomeViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainWindowViewModel();
    }
}
