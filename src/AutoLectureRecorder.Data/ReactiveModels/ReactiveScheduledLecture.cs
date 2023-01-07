using ReactiveUI.Fody.Helpers;
using System;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveScheduledLecture
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
    public DayOfWeek Day { get; set; }
    [Reactive]
    public DateTime? StartTime { get; set; }
    [Reactive]
    public DateTime? EndTime { get; set; }
    [Reactive]
    public bool IsScheduled { get; set; }
    [Reactive]
    public bool WillAutoUpload { get; set; }
}
