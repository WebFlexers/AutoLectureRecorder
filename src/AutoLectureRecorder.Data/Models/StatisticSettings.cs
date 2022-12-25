namespace AutoLectureRecorder.Data.Models;

public class StatisticSettings
{
    public int Id { get; set; }
    public int TrackRecordedLectures { get; set; }
    public int TrackSuccessfulRecordings { get; set; }
    public int TrackFailedRecordings { get; set; }
}
