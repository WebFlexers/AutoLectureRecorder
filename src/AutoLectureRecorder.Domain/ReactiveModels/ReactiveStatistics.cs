using ReactiveUI;

namespace AutoLectureRecorder.Domain.ReactiveModels;

public class ReactiveStatistics : ReactiveObject
{
    public ReactiveStatistics(int totalRecordAttempts, int recordingSucceededNumber, int recordingFailedNumber)
    {
        _totalRecordAttempts = totalRecordAttempts;
        _recordingSucceededNumber = recordingSucceededNumber;
        _recordingFailedNumber = recordingFailedNumber;
    }
    
    private int _totalRecordAttempts;
    public int TotalRecordAttempts 
    {
        get => _totalRecordAttempts;
        set => this.RaiseAndSetIfChanged(ref _totalRecordAttempts, value);
    }
    
    private int _recordingSucceededNumber;
    public int RecordingSucceededNumber 
    {
        get => _recordingSucceededNumber;
        set => this.RaiseAndSetIfChanged(ref _recordingSucceededNumber, value);
    }
    
    private int _recordingFailedNumber;
    public int RecordingFailedNumber 
    {
        get => _recordingFailedNumber;
        set => this.RaiseAndSetIfChanged(ref _recordingFailedNumber, value);
    }
}
