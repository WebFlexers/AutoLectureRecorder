using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveRecordedLecture : ReactiveObject
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
    public DateTime? RecordingDate { get; set; }
    [Reactive]
    public DateTime? StartedAtTime { get; set; }
    [Reactive]
    public DateTime? EndedAtTime { get; set; }
}
