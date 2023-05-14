using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveScheduledLecture : ReactiveObject, ICloneable
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public string SubjectName { get; set; }
    [Reactive]
    public int Semester { get; set; }
    [Reactive]
    public string MeetingLink { get; set; }
    [Reactive]
    public DayOfWeek? Day { get; set; }
    [Reactive]
    public DateTime? StartTime { get; set; }
    [Reactive]
    public DateTime? EndTime { get; set; }
    [Reactive]
    public bool IsScheduled { get; set; }
    [Reactive]
    public bool WillAutoUpload { get; set; }

    /// <summary>
    /// Calculates the timespan between StartTime and EndTime
    /// </summary>
    public TimeSpan CalculateLectureDuration()
    {
        if (StartTime == null || EndTime == null) return TimeSpan.Zero;

        return EndTime.Value.Subtract(StartTime.Value);
    }

    public static bool AreLecturesOverlapping(ReactiveScheduledLecture lecture1, ReactiveScheduledLecture lecture2)
    {
        return lecture1.StartTime <= lecture2.EndTime && lecture2.StartTime <= lecture1.EndTime;
    }

    public object Clone()
    {
        return new ReactiveScheduledLecture()
        {
            Id = this.Id,
            SubjectName = this.SubjectName,
            Semester = this.Semester,
            MeetingLink = this.MeetingLink,
            Day = this.Day,
            StartTime = this.StartTime,
            EndTime = this.EndTime,
            IsScheduled = this.IsScheduled,
            WillAutoUpload = this.WillAutoUpload
        };
    }
}
