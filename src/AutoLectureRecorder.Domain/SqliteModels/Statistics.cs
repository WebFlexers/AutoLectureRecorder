namespace AutoLectureRecorder.Domain.SqliteModels;

public record Statistics
(
    int TotalRecordAttempts,
    int RecordingSucceededNumber,
    int RecordingFailedNumber
)
{
    public Statistics() : this(default!, default!, default!)
    { }
};
