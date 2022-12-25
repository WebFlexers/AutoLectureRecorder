using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveRecordedLecture
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public string StudentRegistrationNumber { get; set; }
    [Reactive]
    public string SubjectName { get; set; }
    [Reactive]
    public int Semester { get; set; }
    [Reactive]
    public string CloudLink { get; set; }
    [Reactive]
    public DateOnly RecordingDate { get; set; }
    [Reactive]
    public TimeOnly StartedAtTime { get; set; }
    [Reactive]
    public TimeOnly EndedAtTime { get; set; }
}
