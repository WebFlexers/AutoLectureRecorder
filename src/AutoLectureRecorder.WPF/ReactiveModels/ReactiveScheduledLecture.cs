using ReactiveUI.Fody.Helpers;
using System;

namespace AutoLectureRecorder.WPF.ReactiveModels;

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
    public TimeOnly StartTime { get; set; }
    [Reactive]
    public TimeOnly EndTime { get; set; }
    [Reactive]
    public bool IsScheduled { get; set; }
}
