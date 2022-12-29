using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;

public class DashboardViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(DashboardViewModel);
    public IScreen HostScreen { get; }

    public DashboardViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
