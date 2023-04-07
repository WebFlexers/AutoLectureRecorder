using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public class ScheduleViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ILogger<ScheduleViewModel> _logger;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IScheduledLectureRepository _lectureRepository;
    public string UrlPathSegment => nameof(ScheduleViewModel);
    public IScreen HostScreen { get; }

    // Used for pagination
    public const int LoadItemsCount = 5;

    public ReactiveCommand<ReactiveScheduledLecture, Unit> NavigateToCreateLectureCommand { get; }
    public ReactiveCommand<DayOfWeek, bool> LoadNextScheduledLecturesCommand { get; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> UpdateScheduledLectureCommand { get; }
    public Task? PopulateAllScheduledLecturesTask { get; set; }

    [Reactive]
    public Dictionary<DayOfWeek, ObservableCollection<ReactiveScheduledLecture>> VisibleScheduledLecturesByDay { get; set; } = new()
    {
        { DayOfWeek.Sunday, new() },
        { DayOfWeek.Monday, new() },
        { DayOfWeek.Tuesday, new() },
        { DayOfWeek.Wednesday, new() },
        { DayOfWeek.Thursday, new() },
        { DayOfWeek.Friday, new() },
        { DayOfWeek.Saturday, new() }
    };

    public Dictionary<DayOfWeek, int> VisibleLecturesPageIndexByDay { get; set; } = new()
    {
        { DayOfWeek.Sunday, 0 },
        { DayOfWeek.Monday, 0 },
        { DayOfWeek.Tuesday, 0 },
        { DayOfWeek.Wednesday, 0 },
        { DayOfWeek.Thursday, 0 },
        { DayOfWeek.Friday, 0 },
        { DayOfWeek.Saturday, 0 }
    };

    [Reactive]
    public Dictionary<DayOfWeek, List<ReactiveScheduledLecture>> AllScheduledLecturesByDay { get; set; } = new()
    {
        { DayOfWeek.Sunday, new() },
        { DayOfWeek.Monday, new() },
        { DayOfWeek.Tuesday, new() },
        { DayOfWeek.Wednesday, new() },
        { DayOfWeek.Thursday, new() },
        { DayOfWeek.Friday, new() },
        { DayOfWeek.Saturday, new() }
    };

    public bool SundayHasLectures => VisibleScheduledLecturesByDay[DayOfWeek.Sunday].Any();
    public bool MondayHasLectures => VisibleScheduledLecturesByDay[DayOfWeek.Monday].Any();
    public bool TuesdayHasLectures => VisibleScheduledLecturesByDay[DayOfWeek.Tuesday].Any();
    public bool WednesdayHasLectures => VisibleScheduledLecturesByDay[DayOfWeek.Wednesday].Any();
    public bool ThursdayHasLectures => VisibleScheduledLecturesByDay[DayOfWeek.Thursday].Any();
    public bool FridayHasLectures => VisibleScheduledLecturesByDay[DayOfWeek.Friday].Any();
    public bool SaturdayHasLectures => VisibleScheduledLecturesByDay[DayOfWeek.Saturday].Any();

    public ScheduleViewModel(ILogger<ScheduleViewModel> logger, IScreenFactory screenFactory, 
        IViewModelFactory viewModelFactory, IScheduledLectureRepository lectureRepository)
    {
        _logger = logger;
        _viewModelFactory = viewModelFactory;
        _lectureRepository = lectureRepository;
        HostScreen = screenFactory.GetMainMenuViewModel();

        NavigateToCreateLectureCommand = ReactiveCommand.Create<ReactiveScheduledLecture>(lecture =>
        {
            var createLectureVm = _viewModelFactory.CreateRoutableViewModel(typeof(CreateLectureViewModel));
            HostScreen.Router.Navigate.Execute(createLectureVm).Subscribe();
            MessageBus.Current.SendMessage(lecture, PubSubMessages.SetUpdateModeToScheduledLecture);
        });

        LoadNextScheduledLecturesCommand = ReactiveCommand.CreateFromTask<DayOfWeek, bool>(LoadNextScheduledLectures);

        UpdateScheduledLectureCommand = ReactiveCommand.CreateFromTask<ReactiveScheduledLecture>(async lecture =>
        {
            if (await DisableConflictingLectures(lecture) == false) return;

            var result = await _lectureRepository.UpdateScheduledLectureAsync(lecture);

            if (result == false) return;

            // Send message to recalculate the closest scheduled lecture to now,
            // in case the newly updated lecture is closer
            MessageBus.Current.SendMessage(true, PubSubMessages.CheckClosestScheduledLecture);
        });

        PopulateAllScheduledLecturesTask = PopulateAllScheduledLectures();
    }

    public async Task PopulateAllScheduledLectures()
    {
        var allLectures = await _lectureRepository.GetAllScheduledLecturesAsync();

        if (allLectures == null || allLectures.Any() == false) return;

        foreach (var lecture in allLectures)
        {
            if (lecture.Day.HasValue == false) continue;

            AllScheduledLecturesByDay[lecture.Day.Value].Add(lecture);
        }

        foreach (var lecturesByDay in AllScheduledLecturesByDay)
        {
            AllScheduledLecturesByDay[lecturesByDay.Key] = lecturesByDay.Value
                .OrderBy(lecture => lecture.StartTime)
                .ToList();
        }
    }

    /// <summary>
    /// Loads the next scheduled lectures if any are remaining
    /// and returns true if it loaded at least one. Otherwise returns false
    /// </summary>
    public async Task<bool> LoadNextScheduledLectures(DayOfWeek day)
    {
        if (PopulateAllScheduledLecturesTask == null) return false;

        await PopulateAllScheduledLecturesTask;

        int totalDayLecturesCount = AllScheduledLecturesByDay[day].Count;

        if (VisibleScheduledLecturesByDay[day].Count < totalDayLecturesCount)
        {
            VisibleScheduledLecturesByDay[day].AddRange(
                AllScheduledLecturesByDay[day]
                        .Skip(LoadItemsCount * VisibleLecturesPageIndexByDay[day])
                        .Take(LoadItemsCount)
            );

            VisibleLecturesPageIndexByDay[day] += 1;

            return true;
        }

        return false;
    }

    private async Task<bool> DisableConflictingLectures(ReactiveScheduledLecture lectureToKeep)
    {
        if (lectureToKeep.IsScheduled == false || lectureToKeep.Day == null) return true;

        var dayLectures = AllScheduledLecturesByDay[lectureToKeep.Day.Value];

        if (dayLectures.Any() == false) return false;

        var updateLecturesTasks = new List<Task>();
        foreach (var existingLecture in dayLectures)
        {
            if (ReactiveScheduledLecture.AreLecturesOverlapping(lectureToKeep, existingLecture))
            {
                // TODO: Make IsActive update on the UI correctly
                existingLecture.IsScheduled = false;
                var visibleLecture = VisibleScheduledLecturesByDay[lectureToKeep.Day.Value]
                    .FirstOrDefault(lecture => lecture.Id == existingLecture.Id);
                if (visibleLecture != null)
                {
                    visibleLecture = new ReactiveScheduledLecture
                    {
                        Id = existingLecture.Id,
                        SubjectName = existingLecture.SubjectName,
                        Semester = existingLecture.Semester,
                        MeetingLink = existingLecture.MeetingLink,
                        Day = existingLecture.Day,
                        StartTime = existingLecture.StartTime,
                        EndTime = existingLecture.EndTime,
                        IsScheduled = existingLecture.IsScheduled,
                        WillAutoUpload = existingLecture.WillAutoUpload
                    };
                }

                updateLecturesTasks.Add(_lectureRepository.UpdateScheduledLectureAsync(existingLecture));
            }
        }

        await Task.WhenAll(updateLecturesTasks);

        return true;
    }
}
