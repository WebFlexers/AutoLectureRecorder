using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
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
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly IWindowFactory _windowFactory;
    private readonly IStudentAccountRepository _studentAccountRepository;

    public string UrlPathSegment => nameof(DashboardViewModel);
    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public ReactiveCommand<Unit, Unit> FindClosestScheduledLectureToNowCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToCreateLectureCommand { get; }

    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture> TodaysLectures { get; private set; } = new();

    private readonly ObservableAsPropertyHelper<bool> _areLecturesScheduledToday;
    public bool AreLecturesScheduledToday => _areLecturesScheduledToday.Value;

    private readonly DispatcherTimer? _nextLectureCalculatorTimer;

    [Reactive]
    public string? RegistrationNumber { get; private set; }
    [Reactive]
    public ReactiveScheduledLecture? NextScheduledLecture { get; private set; }
    [Reactive]
    public TimeSpan? NextScheduledLectureTimeDiff { get; set; }

    public DashboardViewModel(ILogger<DashboardViewModel> logger, IScreenFactory screenFactory, 
        IScheduledLectureRepository scheduledLectureRepository, IWindowFactory windowFactory,
        IStudentAccountRepository studentAccountRepository)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
        _logger = logger;
        _scheduledLectureRepository = scheduledLectureRepository;
        _windowFactory = windowFactory;
        _studentAccountRepository = studentAccountRepository;
        Activator = new ViewModelActivator();

        FindClosestScheduledLectureToNowCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var lecturesSorted = await _scheduledLectureRepository.GetScheduledLecturesOrderedByDayAndStartTime();
            NextScheduledLecture = FindClosestScheduledLectureToNow(lecturesSorted);
            NextScheduledLectureTimeDiff = CalculateNextScheduledLectureTimeDiff();
        });

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
        {
            var mainMenuViewModel = (MainMenuViewModel)screenFactory.GetMainMenuViewModel();
            mainMenuViewModel.Navigate(typeof(CreateLectureViewModel));
        });

        // Get a notification when a scheduled lecture is either added or deleted
        // in order to search again for the closest one
        MessageBus.Current.Listen<bool>(PubSubMessages.CheckClosestScheduledLecture)
            .Select(shouldCheck => shouldCheck)
            .InvokeCommand(ReactiveCommand.CreateFromTask<bool>(async shouldCheck =>
            {
                if (shouldCheck == false) return;

                var lecturesSorted = await _scheduledLectureRepository.GetScheduledLecturesOrderedByDayAndStartTime();
                if (lecturesSorted == null || lecturesSorted.Any() == false) return;

                NextScheduledLecture = FindClosestScheduledLectureToNow(lecturesSorted);
            }));

        // Create a timer that constantly calculates the difference
        // between now and the closest scheduled lecture
        _nextLectureCalculatorTimer = new DispatcherTimer();
        _nextLectureCalculatorTimer.Interval = TimeSpan.FromMilliseconds(500);
        _nextLectureCalculatorTimer.Tick += CalculateClosestLectureTimeDiffTick;
        FindClosestScheduledLectureToNowCommand.Execute()
        .Subscribe(_ =>
        {
            _nextLectureCalculatorTimer.Start();
        });

        _areLecturesScheduledToday =
            this.WhenAnyValue(vm => vm.TodaysLectures)
                .Select(lectures => lectures.Any())
                .ToProperty(this, vm => vm.AreLecturesScheduledToday);

        this.WhenActivated(disposables =>
        {
            Observable.FromAsync(async () =>
                {
                    await FetchTodaysLectures();
                    var studentAccount = await _studentAccountRepository.GetStudentAccountAsync();
                    RegistrationNumber = studentAccount?.RegistrationNumber;
                })
                .Catch((Exception e) =>
                {
                    _logger.LogError(e, "An error occurred while fetching todays lectures");
                    return Observable.Empty<Unit>();
                }).Subscribe()
                .DisposeWith(disposables);
        });
    }

    private async Task FetchTodaysLectures()
    {
        var todaysLecturesUnsorted = await _scheduledLectureRepository
            .GetScheduledLecturesByDay(DateTime.Now.DayOfWeek);

        if (todaysLecturesUnsorted == null || todaysLecturesUnsorted.Any() == false) return;

        var todaysLecturesSortedByStartTime =
            todaysLecturesUnsorted.OrderBy(lecture => lecture.StartTime);

        // TODO: Create a way to detect whether a lecture was successfully recorded and change the icon accordingly
        TodaysLectures = new ObservableCollection<ReactiveScheduledLecture>(todaysLecturesSortedByStartTime);
    }

    private async void CalculateClosestLectureTimeDiffTick(object? sender, EventArgs e)
    {
        NextScheduledLectureTimeDiff = CalculateNextScheduledLectureTimeDiff();

        if (NextScheduledLectureTimeDiff < TimeSpan.FromSeconds(1))
        {
            _nextLectureCalculatorTimer!.Stop();

            await Task.Delay(TimeSpan.FromSeconds(1));

            var recordWindow = _windowFactory.CreateRecordWindow((ReactiveScheduledLecture)NextScheduledLecture!.Clone());
            recordWindow.Show();

            FindClosestScheduledLectureToNowCommand.Execute()
            .Subscribe(_ =>
            {
                _nextLectureCalculatorTimer.Start();
            });
        }
    }

    private static ReactiveScheduledLecture? FindClosestScheduledLectureToNow(IReadOnlyCollection<ReactiveScheduledLecture>? lecturesSorted)
    {
        if (lecturesSorted == null || lecturesSorted.Any() == false)
        {
            return null;
        }

        var activeLectures = lecturesSorted
            .Where(lecture => lecture.IsScheduled)
            .ToList();

        DayOfWeek today = DateTime.Today.DayOfWeek;
        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        bool closestDayIsTodayOrAfterUntilSunday = false;

        // First we find out if there are any lectures in the collection that are
        // scheduled for today or any day after today (until Sunday).
        //  E.g. If today is Friday we check if there is a scheduled lecture at Friday, Saturday or Sunday
        int counter = 0;
        while (counter < activeLectures.Count)
        {
            if (activeLectures[counter].Day >= today)
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
            return activeLectures[0];
        }

        // If the closest day is today we need to check if the start
        // time of the lecture is after the current time of day.
        // So we continue where we left off using the same counter
        while (counter < activeLectures.Count && activeLectures[counter].Day == today)
        {
            if (activeLectures[counter].StartTime!.Value.TimeOfDay >= currentTime)
            {
                return activeLectures[counter];
            }

            counter++;
        }

        // Let's not overstep
        if (counter == activeLectures.Count)
        {
            counter--;
        }

        // If closest scheduled lecture is after today we just return the
        // first lecture that is after today, because they are sorted
        if (activeLectures[counter].Day != today)
        {
            return activeLectures[counter];
        }

        // If we get at this point it means that that there are scheduled lectures only for today
        // and no other day, but they all start before the current time. So we just return the
        // first element of the list, again because it is sorted by day first and then by start time
        return activeLectures[0];
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
        while (dayCounter <= 6)
        {
            // Start counting the days from tomorrow
            dayCounter++;

            // When we surpass Sunday we go to Monday
            if (dayCounter > 6)
            {
                dayCounter = 0;
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
