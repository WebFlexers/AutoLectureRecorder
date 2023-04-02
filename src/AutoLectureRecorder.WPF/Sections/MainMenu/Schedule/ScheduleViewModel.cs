using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
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

    [Reactive]
    public ReactiveCommand<ReactiveScheduledLecture, Unit> NavigateToCreateLectureCommand { get; set; }

    [Reactive]
    public Dictionary<DayOfWeek, ObservableCollection<ReactiveScheduledLecture>> ScheduledLecturesByDay { get; set; } = new()
    {
        { DayOfWeek.Sunday, new() },
        { DayOfWeek.Monday, new() },
        { DayOfWeek.Tuesday, new() },
        { DayOfWeek.Wednesday, new() },
        { DayOfWeek.Thursday, new() },
        { DayOfWeek.Friday, new() },
        { DayOfWeek.Saturday, new() }
    };

    public bool SundayHasLectures => ScheduledLecturesByDay[DayOfWeek.Sunday].Any();
    public bool MondayHasLectures => ScheduledLecturesByDay[DayOfWeek.Monday].Any();
    public bool TuesdayHasLectures => ScheduledLecturesByDay[DayOfWeek.Tuesday].Any();
    public bool WednesdayHasLectures => ScheduledLecturesByDay[DayOfWeek.Wednesday].Any();
    public bool ThursdayHasLectures => ScheduledLecturesByDay[DayOfWeek.Thursday].Any();
    public bool FridayHasLectures => ScheduledLecturesByDay[DayOfWeek.Friday].Any();
    public bool SaturdayHasLectures => ScheduledLecturesByDay[DayOfWeek.Saturday].Any();

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

        Observable.FromAsync(PopulateAllScheduledLectures)
            .Catch((Exception e) =>
            {
                _logger.LogError(e, "An error occurred while populating scheduled lectures");
                return Observable.Empty<Unit>();
            }).Subscribe();
    }

    public async Task PopulateAllScheduledLectures()
    {
        var allLectures = await _lectureRepository.GetAllScheduledLecturesAsync();

        if (allLectures == null || allLectures.Any() == false) return;

        foreach (var lecture in allLectures)
        {
            if (lecture.Day.HasValue == false)
            {
                continue;
            }

            ScheduledLecturesByDay[lecture.Day.Value].Add(lecture);
        }
    }
}
