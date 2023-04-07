﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveScheduledLecture : ReactiveObject
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

    public static bool AreLecturesOverlapping(ReactiveScheduledLecture lecture1, ReactiveScheduledLecture lecture2)
    {
        return lecture1.StartTime <= lecture2.EndTime && lecture2.StartTime <= lecture1.EndTime;
    }
}
