namespace AutoLectureRecorder.Domain.SqliteModels;

public record ScheduledLecture
(
    int Id,
    string SubjectName,
    int Semester,
    string MeetingLink,
    int Day,
    string StartTime,
    string EndTime,
    int IsScheduled,
    int WillAutoUpload
)
{
    public ScheduledLecture() : this(default!, default!, default!, default!, 
        default!, default!, default!, default!, default!)
    { }
};