using AutoLectureRecorder.Application.ScheduledLectures.Common;
using AutoLectureRecorder.Domain.ReactiveModels;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture;

/// <summary>
/// Updates the scheduled lecture of the given id with the given information.
/// Returns a list of the lectures that had to be deactivated in
/// case of a time conflict (if any)
/// </summary>
public class UpdateScheduledLectureCommand : IRequest<ErrorOr<List<ReactiveScheduledLecture>?>>, IValidatableScheduledLecture
{
    public UpdateScheduledLectureCommand(int id, string? subjectName, int semester, string? meetingLink, DayOfWeek? day, 
        TimeOnly startTime, TimeOnly endTime, bool isScheduled, bool willAutoUpload)
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
}