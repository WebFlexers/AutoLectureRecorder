using ErrorOr;
using MediatR;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Application.Recording.Commands.RecordMeeting;

/// <summary>
/// Records the specified meeting on the specified window.
/// WARNING: This method must be called on a separate thread since it runs
/// until the lecture is finished
/// </summary>
public record RecordMeetingCommand(
    string LectureSubjectName,
    int LectureSemester,
    TimeOnly LectureEndTime,
    IntPtr? WindowHandle,
    Action? OnRecordingFailed = null) : IRequest<ErrorOr<Unit>>;