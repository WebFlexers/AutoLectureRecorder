using AutoLectureRecorder.Domain.ReactiveModels;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.DisableConflictingLectures;

/// <summary>
/// Disables the lectures that are active and whose timings conflict
/// with the given lecture id. Returns a list of the lectures that were updated
/// </summary>
public record DisableConflictingLecturesCommand(
    int ReferenceScheduledLectureId) : IRequest<List<ReactiveScheduledLecture>?>;