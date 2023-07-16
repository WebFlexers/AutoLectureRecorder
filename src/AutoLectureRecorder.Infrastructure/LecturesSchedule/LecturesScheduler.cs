using System.Reactive;
using System.Reactive.Linq;
using AutoLectureRecorder.Application.Common.Abstractions.LecturesSchedule;
using AutoLectureRecorder.Domain.ReactiveModels;
using ReactiveUI;

namespace AutoLectureRecorder.Infrastructure.LecturesSchedule;

public class LecturesScheduler : ReactiveObject, ILecturesScheduler
{
    private ReactiveScheduledLecture? _nextScheduledLecture;
    public ReactiveScheduledLecture? NextScheduledLecture
    {
        get => _nextScheduledLecture;
        set => this.RaiseAndSetIfChanged(ref _nextScheduledLecture, value);
    }
    
    private TimeSpan? _nextScheduledLectureTimeDistance;
    public TimeSpan? NextScheduledLectureTimeDistance
    {
        get => _nextScheduledLectureTimeDistance;
        private set => this.RaiseAndSetIfChanged(ref _nextScheduledLectureTimeDistance, value);
    }

    private readonly ReactiveCommand<Unit, bool> _nextScheduledLectureWillBeginCommand;

    public LecturesScheduler()
    {
        _nextScheduledLectureWillBeginCommand = ReactiveCommand.CreateFromTask<Unit, bool>(async _ =>
        {
            NextScheduledLectureTimeDistance = CalculateNextScheduledLectureTimeDiff();

            if (NextScheduledLectureTimeDistance < TimeSpan.FromSeconds(1))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return true;
            }

            return false;
        });
    }

    public IObservable<bool> NextScheduledLectureWillBegin =>
        Observable.Interval(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .SelectMany(_ => _nextScheduledLectureWillBeginCommand.Execute().AsObservable())
            .DistinctUntilChanged();

    private TimeSpan? CalculateNextScheduledLectureTimeDiff()
    {
        if (NextScheduledLecture == null)
        {
            return null;
        }

        DayOfWeek today = DateTime.Today.DayOfWeek;
        TimeOnly currentTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
        TimeOnly scheduledLectureStartTime = NextScheduledLecture.StartTime;

        // If the lecture is today and it's after the current time we just get the difference
        if (NextScheduledLecture.Day == today && scheduledLectureStartTime > currentTime)
        {
            return scheduledLectureStartTime - currentTime;
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
                var remainingTimeToday = TimeSpan.FromDays(1).Subtract(currentTime.ToTimeSpan());
                var finalTime = totalTimeDiff.Add(scheduledLectureStartTime.ToTimeSpan()).Add(remainingTimeToday);

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