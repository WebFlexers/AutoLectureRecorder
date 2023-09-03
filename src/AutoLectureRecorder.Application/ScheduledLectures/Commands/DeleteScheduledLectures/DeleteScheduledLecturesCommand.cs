using AutoLectureRecorder.Domain.ReactiveModels;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.DeleteScheduledLectures;

/// <summary>
/// Deletes the given lectures and returns a list of the ids of the deleted lectures
/// </summary>
public record DeleteScheduledLecturesCommand(
    List<ReactiveScheduledLecture> LecturesToDelete) : IRequest<List<int>>;