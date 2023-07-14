using AutoLectureRecorder.Application.ScheduledLectures.Common;
using AutoLectureRecorder.Domain.ReactiveModels;
using MediatR;

using ErrorOr;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture;

public class UpdateScheduledLectureCommand : IRequest<ErrorOr<ReactiveScheduledLecture>>, IValidatableScheduledLecture
{
    public UpdateScheduledLectureCommand(string? subjectName, int semester, string? meetingLink, DayOfWeek? day, 
        TimeOnly startTime, TimeOnly endTime, bool isScheduled, bool willAutoUpload, bool ignoreValidationWarnings)
    {
        SubjectName = subjectName;
        Semester = semester;
        MeetingLink = meetingLink;
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
        IsScheduled = isScheduled;
        WillAutoUpload = willAutoUpload;
        IgnoreValidationWarnings = ignoreValidationWarnings;
    }

    public string? SubjectName { get; set; }
    public int Semester { get; set; }
    public string? MeetingLink { get; set; }
    public DayOfWeek? Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsScheduled { get; set; }
    public bool WillAutoUpload { get; set; }

    public bool IgnoreValidationWarnings { get; set; }
}