using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.MainMenu.Schedule;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;

public class DashboardViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ILogger<DashboardViewModel> _logger;
    private readonly IScheduledLectureData _lectureData;

    public string? UrlPathSegment => nameof(DashboardViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> FindClosestScheduledLectureToNowCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> LoadAllScheduledLecturesCommand { get; private set; }

    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture> AllScheduledLectures { get; private set; }
    [Reactive]
    public ReactiveScheduledLecture? NextScheduledLecture { get; private set; }

    [Reactive]
    public TimeSpan? NextScheduledLectureTimeDiff { get; set; } 

    public DashboardViewModel(ILogger<DashboardViewModel> logger, IScreenFactory screenFactory, IScheduledLectureData lectureData)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
        _logger = logger;
        _lectureData = lectureData;

        FindClosestScheduledLectureToNowCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var lecturesSorted = await _lectureData.GetAllScheduledLecturesSortedAsync();
            NextScheduledLecture = FindClosestScheduledLectureToNow(lecturesSorted);
            NextScheduledLectureTimeDiff = CalculateNextScheduledLectureTimeDiff();
        });

        LoadAllScheduledLecturesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AllScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>(
                await _lectureData.GetAllScheduledLecturesAsync()
            );
        });

        MessageBus.Current.Listen<bool>(PubSubMessages.CheckClosestScheduledLecture)
                          .Subscribe(async (shouldCheck) =>
                          {
                              if (shouldCheck)
                              {
                                  var lecturesSorted = await _lectureData.GetAllScheduledLecturesSortedAsync();
                                  NextScheduledLecture = FindClosestScheduledLectureToNow(lecturesSorted);
                              }
                          });

        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);
        timer.Tick += CalculateClosestLectureTimeDiffTick;
        FindClosestScheduledLectureToNowCommand.Execute().Subscribe();
        timer.Start();
    }

    private void CalculateClosestLectureTimeDiffTick(object? sender, EventArgs e)
    {
        NextScheduledLectureTimeDiff = CalculateNextScheduledLectureTimeDiff();
    }

    private ReactiveScheduledLecture? FindClosestScheduledLectureToNow(List<ReactiveScheduledLecture>? lecturesSorted)
    {
        if (lecturesSorted == null || lecturesSorted.Any() == false) 
        {
            return null;
        }

        DayOfWeek today = DateTime.Today.DayOfWeek;
        var currentTime = DateTime.Now.TimeOfDay;

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
        var currentTime = DateTime.Now.TimeOfDay;
        var scheduledLectureStartTime = NextScheduledLecture.StartTime!.Value.TimeOfDay;

        if (NextScheduledLecture.Day == today && scheduledLectureStartTime > currentTime)
        {
            return scheduledLectureStartTime.Subtract(currentTime);
        }

        TimeSpan totalTimeDiff = TimeSpan.Zero;

        int dayCounter = (int)today;
        while (dayCounter <= 7)
        {
            dayCounter++;

            if (dayCounter > 7)
            {
                dayCounter = 1;
            }

            if ((int)NextScheduledLecture.Day!.Value == dayCounter)
            {
                var remainingTimeToday = TimeSpan.FromDays(1).Subtract(currentTime);
                var finalTime = totalTimeDiff.Add(scheduledLectureStartTime).Add(remainingTimeToday);

                if (TimeSpan.Compare(finalTime, TimeSpan.Zero) == -1)
                {
                    return finalTime.Negate();
                }

                return finalTime;
            }

            totalTimeDiff = totalTimeDiff.Add(TimeSpan.FromDays(1));
        }

        return null;
    }
}
