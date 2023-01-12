using AutoLectureRecorder.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public class ScheduleViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(ScheduleViewModel);
    public IScreen HostScreen { get; }

    public ScheduleViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
