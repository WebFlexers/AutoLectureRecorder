namespace AutoLectureRecorder.Data.Models;

public class RecordedLecture
{
    public int Id { get; set; }
    public string StudentRegistrationNumber { get; set; }
    public string? CloudLink { get; set; }
    public string StartedAt { get; set; }
    public string EndedAt { get; set; }
    public int ScheduledLectureId { get; set; }
}
