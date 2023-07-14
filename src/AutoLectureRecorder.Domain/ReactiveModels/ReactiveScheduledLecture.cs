using ReactiveUI;

namespace AutoLectureRecorder.Domain.ReactiveModels;

public class ReactiveScheduledLecture : ReactiveObject
{
    public ReactiveScheduledLecture() { }
    
    public ReactiveScheduledLecture(int id, string subjectName, int semester, string meetingLink, DayOfWeek? day, 
        TimeOnly startTime, TimeOnly endTime, bool isScheduled, bool willAutoUpload)
    {
        Id = id;
        _subjectName = subjectName;
        _semester = semester;
        _meetingLink = meetingLink;
        _day = day;
        _startTime = startTime;
        _endTime = endTime;
        _isScheduled = isScheduled;
        _willAutoUpload = willAutoUpload;
    }

    public int Id { get; init; }

    private string? _subjectName;
    public string? SubjectName
    {
        get => _subjectName;
        set => this.RaiseAndSetIfChanged(ref _subjectName, value);
    }

    private int _semester;
    public int Semester 
    {
        get => _semester;
        set => this.RaiseAndSetIfChanged(ref _semester, value);
    }

    private string? _meetingLink;
    public string? MeetingLink 
    {
        get => _meetingLink;
        set => this.RaiseAndSetIfChanged(ref _meetingLink, value);
    }

    private DayOfWeek? _day;
    public DayOfWeek? Day 
    {
        get => _day;
        set => this.RaiseAndSetIfChanged(ref _day, value);
    }

    private TimeOnly _startTime;
    public TimeOnly StartTime 
    {
        get => _startTime;
        set => this.RaiseAndSetIfChanged(ref _startTime, value);
    }

    private TimeOnly _endTime;
    public TimeOnly EndTime 
    {
        get => _endTime;
        set => this.RaiseAndSetIfChanged(ref _endTime, value);
    }

    private bool _isScheduled;
    public bool IsScheduled 
    {
        get => _isScheduled;
        set => this.RaiseAndSetIfChanged(ref _isScheduled, value);
    }

    private bool _willAutoUpload;
    public bool WillAutoUpload 
    {
        get => _willAutoUpload;
        set => this.RaiseAndSetIfChanged(ref _willAutoUpload, value);
    }

    /// <summary>
    /// Calculates the time difference between start time and end time
    /// </summary>
    public TimeSpan LectureDuration => EndTime - StartTime;
}
