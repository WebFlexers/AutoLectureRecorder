using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories;
using AutoLectureRecorder.Services.DataAccess;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public class ScheduleViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly ILogger<ScheduleViewModel> _logger;
    private readonly IScheduledLectureRepository _lectureRepository;
    public ViewModelActivator Activator { get; }
    public string UrlPathSegment => nameof(ScheduleViewModel);
    public IScreen HostScreen { get; }

    [Reactive]
    public ReactiveScheduledLecture? Lecture { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> NavigateToCreateLectureCommand { get; set; }

    [Reactive]
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

    public ScheduleViewModel(ILogger<ScheduleViewModel> logger, IScreenFactory screenFactory, IScheduledLectureRepository lectureRepository)
    {
        _logger = logger;
        _lectureRepository = lectureRepository;
        HostScreen = screenFactory.GetMainMenuViewModel();
        Activator = new ViewModelActivator();

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
        {
            Debug.WriteLine("It was clicked in ViewModel");
        });

        this.WhenActivated(async disposables =>
        {
            await FetchAllScheduledLectures().DisposeWith(disposables);
        });
    }

    public async Task FetchAllScheduledLectures()
    {
        var allLectures = await _lectureRepository.GetAllScheduledLecturesAsync();

        if (allLectures.Any() == false) return;

        foreach (var lecture in allLectures)
        {
            if (lecture.Day.HasValue == false) continue;

            ScheduledLecturesByDay[lecture.Day.Value].Add(lecture);
        }
    }
}
