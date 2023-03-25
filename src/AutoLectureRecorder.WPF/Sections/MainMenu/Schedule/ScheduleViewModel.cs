using System;
using System.Collections.Generic;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public class ScheduleViewModel : ReactiveObject, IRoutableViewModel
{
    public Dictionary<DayOfWeek, List<ReactiveScheduledLecture>> ScheduledLecturesByDay { get; set; } = new()
    {
        { DayOfWeek.Monday, new() },
        { DayOfWeek.Tuesday, new() },
        { DayOfWeek.Wednesday, new() },
        { DayOfWeek.Thursday, new() },
        { DayOfWeek.Friday, new() },
        { DayOfWeek.Saturday, new() },
        { DayOfWeek.Sunday, new() },
    };

    public string UrlPathSegment => nameof(ScheduleViewModel);
    public IScreen HostScreen { get; }

    public ScheduleViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
