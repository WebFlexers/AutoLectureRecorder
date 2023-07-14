namespace AutoLectureRecorder.Application.ScheduledLectures.Common;

public interface IValidatableScheduledLecture
{
    string? SubjectName { get; set; }
    int Semester { get; set; }
    string? MeetingLink { get; set; }
    DayOfWeek? Day { get; set; }
    TimeOnly StartTime { get; set; }
    TimeOnly EndTime { get; set; }
    bool IsScheduled { get; set; }
    bool WillAutoUpload { get; set; }
}