using AutoLectureRecorder.Application.ScheduledLectures.Common;
using ErrorOr;
using MediatR;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture;

public class UpdateScheduledLectureCommand : IRequest<ErrorOr<Unit>>, IValidatableScheduledLecture
{
    public UpdateScheduledLectureCommand(int id, string? subjectName, int semester, string? meetingLink, DayOfWeek? day, 
        TimeOnly startTime, TimeOnly endTime, bool isScheduled, bool willAutoUpload, bool ignoreOverlappingLecturesWarning)
    {
        Id = id;
        SubjectName = subjectName;
        Semester = semester;
        MeetingLink = meetingLink;
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
        IsScheduled = isScheduled;
        WillAutoUpload = willAutoUpload;
        IgnoreOverlappingLecturesWarning = ignoreOverlappingLecturesWarning;
    }

    public int Id { get; set; }
    public string? SubjectName { get; set; }
    public int Semester { get; set; }
    public string? MeetingLink { get; set; }
    public DayOfWeek? Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsScheduled { get; set; }
    public bool WillAutoUpload { get; set; }

    public bool IgnoreOverlappingLecturesWarning { get; set; }
}