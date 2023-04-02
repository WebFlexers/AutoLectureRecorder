using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveStatistics : ReactiveObject
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public int TotalRecordAttempts { get; set; }
    [Reactive]
    public int RecordingSucceededNumber { get; set; }
    [Reactive]
    public int RecordingFailedNumber { get; set; }
}
