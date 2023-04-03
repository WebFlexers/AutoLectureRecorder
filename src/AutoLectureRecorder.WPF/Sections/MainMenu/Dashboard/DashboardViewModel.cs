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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AutoLectureRecorder.Sections.MainMenu.Dashboard;

public class DashboardViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly ILogger<DashboardViewModel> _logger;
    private readonly IScheduledLectureRepository _lectureData;

    public string UrlPathSegment => nameof(DashboardViewModel);
    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public ReactiveCommand<Unit, Unit> FindClosestScheduledLectureToNowCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToCreateLectureCommand { get; }

    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture> TodaysLectures { get; private set; } = new();

    private readonly ObservableAsPropertyHelper<bool> _areLecturesScheduledToday;
    public bool AreLecturesScheduledToday => _areLecturesScheduledToday.Value;

    [Reactive]
    public ReactiveScheduledLecture? NextScheduledLecture { get; private set; }
    [Reactive]
    public TimeSpan? NextScheduledLectureTimeDiff { get; set; }

    public DashboardViewModel(ILogger<DashboardViewModel> logger, IScreenFactory screenFactory, IScheduledLectureRepository lectureData)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
        _logger = logger;
        _lectureData = lectureData;
        Activator = new ViewModelActivator();

        FindClosestScheduledLectureToNowCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var lecturesSorted = await _lectureData.GetAllScheduledLecturesSortedAsync();
            NextScheduledLecture = FindClosestScheduledLectureToNow(lecturesSorted);
            NextScheduledLectureTimeDiff = CalculateNextScheduledLectureTimeDiff();
        });

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
        {
            var mainMenuViewModel = (MainMenuViewModel)screenFactory.GetMainMenuViewModel();
            mainMenuViewModel.SetRoutedViewHostContent(typeof(CreateLectureViewModel));
        });

        // Get a notification when a scheduled lecture is either added or deleted
        // in order to search again for the closest one
        MessageBus.Current.Listen<bool>(PubSubMessages.CheckClosestScheduledLecture)
            .Subscribe(shouldCheck =>
            {
              if (!shouldCheck) return;

              Observable.FromAsync(async () =>
              {
                  var lecturesSorted = await _lectureData.GetAllScheduledLecturesSortedAsync();
                  if (lecturesSorted == null || lecturesSorted.Any() == false) return;

                  NextScheduledLecture = FindClosestScheduledLectureToNow(lecturesSorted);
              }).Catch((Exception e) =>
              {
                  _logger.LogError(e, "An error occurred while searching for closest scheduled lectures to now");
                  return Observable.Empty<Unit>();
              }).Subscribe();
            });

        Observable.FromAsync(FetchTodaysLectures)
            .Catch((Exception e) =>
            {
                _logger.LogError(e, "An error occurred while fetching todays lectures");
                return Observable.Empty<Unit>();
            }).Subscribe();

        // Create a timer that constantly calculates the difference
        // between now and the closest scheduled lecture
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);
        timer.Tick += CalculateClosestLectureTimeDiffTick;
        FindClosestScheduledLectureToNowCommand.Execute().Subscribe();
        timer.Start();

        _areLecturesScheduledToday = 
            this.WhenAnyValue(vm => vm.TodaysLectures)
                .Select(lectures => lectures.Any())
                .ToProperty(this, vm => vm.AreLecturesScheduledToday);
    }

    private async Task FetchTodaysLectures()
    {
        var todaysLecturesUnsorted = await _lectureData
            .GetScheduledLecturesByDayAsync(DateTime.Now.DayOfWeek);

        if (todaysLecturesUnsorted == null || todaysLecturesUnsorted.Any() == false) return;

        var todaysLecturesSortedByStartTime = 
            todaysLecturesUnsorted.OrderBy(lecture => lecture.StartTime);

        // TODO: Create a way to detect whether a lecture was successfully recorded and change the icon accordingly
        TodaysLectures = new ObservableCollection<ReactiveScheduledLecture>(todaysLecturesSortedByStartTime);
    }

    private void CalculateClosestLectureTimeDiffTick(object? sender, EventArgs e)
    {
        NextScheduledLectureTimeDiff = CalculateNextScheduledLectureTimeDiff();
    }

    private static ReactiveScheduledLecture? FindClosestScheduledLectureToNow(IReadOnlyList<ReactiveScheduledLecture>? lecturesSorted)
    {
        if (lecturesSorted == null || lecturesSorted.Any() == false)
        {
            return default;
        }

        DayOfWeek today = DateTime.Today.DayOfWeek;
        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        bool closestDayIsTodayOrAfterUntilSunday = false;

        // First we find out if there are any lectures in the collection that are
        // scheduled for today or any day after today (until Sunday).
        //  E.g. If today is Friday we check if there is a scheduled lecture at Friday, Saturday or Sunday
        int counter = 0;
        while (counter < lecturesSorted.Count)
        {
            if (lecturesSorted[counter].Day >= today)
            {
                closestDayIsTodayOrAfterUntilSunday = true;
                break;
            }
            counter++;
        }

        // If there isn't a scheduled lecture from today until Sunday
        // then, because the lectures are sorted by day first
        // and start time second, the first one on the list
        // will be the closest
        if (closestDayIsTodayOrAfterUntilSunday == false)
        {
            return lecturesSorted[0];
        }

        // If the closest day is today we need to check if the start
        // time of the lecture is after the current time of day.
        // So we continue where we left off using the same counter
        while (counter < lecturesSorted.Count && lecturesSorted[counter].Day == today)
        {
            if (lecturesSorted[counter].StartTime!.Value.TimeOfDay >= currentTime)
            {
                return lecturesSorted[counter];
            }

            counter++;
        }

        // Let's not overstep
        if (counter == lecturesSorted.Count)
        {
            counter--;
        }

        // If there are scheduled lectures after today we just return the
        // first lecture that is after today, because they are sorted
        if (lecturesSorted[counter].Day != today)
        {
            return lecturesSorted[counter];
        }

        // If we get at this point it means that that there are scheduled lectures only for today
        // and no other day, but they all start before the current time. So we just return the
        // first element of the list, again because it is sorted by day first and then by start time
        return lecturesSorted[0];
    }

    private TimeSpan? CalculateNextScheduledLectureTimeDiff()
    {
        if (NextScheduledLecture == null)
        {
            return null;
        }

        DayOfWeek today = DateTime.Today.DayOfWeek;
        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        TimeSpan scheduledLectureStartTime = NextScheduledLecture.StartTime!.Value.TimeOfDay;

        // If the lecture is today and it's after the current time we just get the difference
        if (NextScheduledLecture.Day == today && scheduledLectureStartTime > currentTime)
        {
            return scheduledLectureStartTime.Subtract(currentTime);
        }

        TimeSpan totalTimeDiff = TimeSpan.Zero;

        // Since the lecture is not today we iterate through the days until we reach
        // the day of the scheduled lecture
        int dayCounter = (int)today;
        while (dayCounter <= 7)
        {
            // Start counting the days from tomorrow
            dayCounter++;

            // When we surpass Sunday we go to Monday
            if (dayCounter > 7)
            {
                dayCounter = 1;
            }

            if ((int)NextScheduledLecture.Day!.Value == dayCounter)
            {
                // Add the remaining time of today and the time of the scheduled lecture to the diff
                var remainingTimeToday = TimeSpan.FromDays(1).Subtract(currentTime);
                var finalTime = totalTimeDiff.Add(scheduledLectureStartTime).Add(remainingTimeToday);

                // Legend says that in some cases the time difference comes up as negative
                // but it's still correct. In that case we just reverse the sign back to positive
                if (TimeSpan.Compare(finalTime, TimeSpan.Zero) == -1)
                {
                    return finalTime.Negate();
                }

                return finalTime;
            }

            // Add each day that is not the scheduled lecture's day to the time diff
            totalTimeDiff = totalTimeDiff.Add(TimeSpan.FromDays(1));
        }

        return null;
    }
}
