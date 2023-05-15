using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveRecordedLecture : ReactiveObject
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public string StudentRegistrationNumber { get; set; }
    [Reactive]
    public string SubjectName { get; set; }
    [Reactive]
    public int Semester { get; set; }
    [Reactive]
    public string? CloudLink { get; set; }
    [Reactive]
    public DateTime? StartedAt { get; set; }
    [Reactive]
    public DateTime? EndedAt { get; set; }
    [Reactive]
    public int ScheduledLectureId { get; set; }

    public TimeSpan LectureDuration => CalculateLectureDuration();

    /// <summary>
    /// Calculates the timespan between StartTime and EndTime
    /// </summary>
    public TimeSpan CalculateLectureDuration()
    {
        if (StartedAt == null || EndedAt == null) return TimeSpan.Zero;

        return EndedAt.Value.Subtract(StartedAt.Value);
    }
}
