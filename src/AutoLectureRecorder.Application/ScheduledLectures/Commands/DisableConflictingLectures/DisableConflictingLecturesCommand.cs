using MediatR;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.DisableConflictingLectures;

public record DisableConflictingLecturesCommand(
    int ReferenceScheduledLectureId) : IRequest<Unit>;