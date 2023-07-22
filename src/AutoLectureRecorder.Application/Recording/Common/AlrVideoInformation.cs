using ReactiveUI;

namespace AutoLectureRecorder.Application.Recording.Common;

public class AlrVideoInformation : ReactiveObject
{
    public AlrVideoInformation(string name, double sizeInGb, DateTime startedAt, DateTime endedAt, TimeSpan duration,
        string filePath)
    {
        _name = name;
        _sizeInGb = sizeInGb;
        _startedAt = startedAt;
        _endedAt = endedAt;
        _duration = duration;
        _filePath = filePath;
    }
    
    private string _name;
    public string Name 
    { 
        get => _name; 
        set => this.RaiseAndSetIfChanged(ref _name, value); 
    }

    private double _sizeInGb;
    public double SizeInGb 
    { 
        get => _sizeInGb; 
        set => this.RaiseAndSetIfChanged(ref _sizeInGb, value); 
    }

    private DateTime _startedAt;
    public DateTime StartedAt 
    { 
        get => _startedAt; 
        set => this.RaiseAndSetIfChanged(ref _startedAt, value); 
    }

    private DateTime _endedAt;
    public DateTime EndedAt 
    { 
        get => _endedAt; 
        set => this.RaiseAndSetIfChanged(ref _endedAt, value); 
    }

    private TimeSpan _duration;
    public TimeSpan Duration 
    { 
        get => _duration; 
        set => this.RaiseAndSetIfChanged(ref _duration, value); 
    }

    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set => this.RaiseAndSetIfChanged(ref _filePath, value);
    }
}