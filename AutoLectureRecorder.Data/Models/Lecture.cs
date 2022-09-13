namespace AutoLectureRecorder.Data.Models;

public class Lecture
{
    public string Name { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string MeetingTeam { get; set; }
    public bool IsLectureActive { get; set; }
    public bool IsAutoUploadActive { get; set; }
    public WeekDay Day { get; set; }
}
