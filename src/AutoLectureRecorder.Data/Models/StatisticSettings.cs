namespace AutoLectureRecorder.Data.Models;

public class StatisticSettings
{
    public int Id { get; set; }
    public int AreRecordedLecturesTracked { get; set; }
    public int AreSuccessfulRecordingsTracked { get; set; }
    public int AreFailedRecordingsTracked { get; set; }
}
