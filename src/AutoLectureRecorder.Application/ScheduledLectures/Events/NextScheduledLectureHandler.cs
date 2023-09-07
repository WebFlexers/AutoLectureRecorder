using AutoLectureRecorder.Application.Common.Abstractions.LecturesSchedule;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Events;

public class NextScheduledLectureHandler : INotificationHandler<NextScheduledLectureEvent>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly ILecturesScheduler _lecturesScheduler;

    public NextScheduledLectureHandler(IScheduledLectureRepository scheduledLectureRepository, 
        ILecturesScheduler lecturesScheduler)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
        _lecturesScheduler = lecturesScheduler;
    }
    
    public async Task Handle(NextScheduledLectureEvent notification, CancellationToken cancellationToken)
    {
        var lecturesSorted = (await _scheduledLectureRepository
            .GetScheduledLecturesOrdered())?.ToArray();

        if (lecturesSorted == null || lecturesSorted.Length == 0)
        {
            _lecturesScheduler.NextScheduledLecture = null;
            return;
        }

        var activeLectures = lecturesSorted
            .Where(lecture => lecture.IsScheduled)
            .ToArray();

        if (activeLectures.Length == 0)
        {
            _lecturesScheduler.NextScheduledLecture = null;
            return;
        }

        DayOfWeek today = DateTime.Today.DayOfWeek;
        TimeOnly currentTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);

        bool closestDayIsTodayOrAfterUntilSunday = false;

        // First we find out if there are any lectures in the collection that are
        // scheduled for today or any day after today (until Sunday).
        //  E.g. If today is Friday we check if there is a scheduled lecture at Friday, Saturday or Sunday
        int counter = 0;
        while (counter < activeLectures.Length)
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
            _lecturesScheduler.NextScheduledLecture = activeLectures[0];
            return;
        }

        // If the closest day is today we need to check if the start
        // time of the lecture is after the current time of day.
        // So we continue where we left off using the same counter
        while (counter < activeLectures.Length && activeLectures[counter].Day == today)
        {
            if (activeLectures[counter].StartTime >= currentTime)
            {
                _lecturesScheduler.NextScheduledLecture = (activeLectures[counter]);
                return;
            }

            counter++;
        }

        // Let's not overstep
        if (counter == activeLectures.Length)
        {
            counter--;
        }

        // If closest scheduled lecture is after today we just return the
        // first lecture that is after today, because they are sorted
        if (activeLectures[counter].Day != today)
        {
            _lecturesScheduler.NextScheduledLecture = (activeLectures[counter]);
            return;
        }

        // If we get at this point it means that that there are scheduled lectures only for today
        // and no other day, but they all start before the current time. So we just return the
        // first element of the list, again because it is sorted by day first and then by start time
        _lecturesScheduler.NextScheduledLecture = (activeLectures[0]);
    }
}