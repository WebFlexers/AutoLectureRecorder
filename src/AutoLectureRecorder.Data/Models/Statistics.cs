namespace AutoLectureRecorder.Data.Models;

public class Statistics
{
    public int Id { get; set; }
    public int TotalRecordAttempts { get; set; }
    public int RecordingSucceededNumber { get; set; }
    public int RecordingFailedNumber { get; set; }
}
