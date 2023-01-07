namespace AutoLectureRecorder.Data.Models;

public class ScheduledLecture
{
    public int Id { get; set; }
    public string SubjectName { get; set; }
    public int Semester { get; set; }
    public string MeetingLink { get; set; }
    public int Day { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int IsScheduled { get; set; }
    public int WillAutoUpload { get; set; }
}
