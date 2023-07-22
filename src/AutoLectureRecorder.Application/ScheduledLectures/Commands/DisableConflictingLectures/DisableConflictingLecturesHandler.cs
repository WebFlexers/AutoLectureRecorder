using AutoLectureRecorder.Application.Common;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.DisableConflictingLectures;

public class DisableConflictingLecturesHandler 
    : MediatR.IRequestHandler<DisableConflictingLecturesCommand, List<ReactiveScheduledLecture>?>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly ISqliteDataAccess _sqliteDataAccess;

    public DisableConflictingLecturesHandler(IScheduledLectureRepository scheduledLectureRepository,
        ISqliteDataAccess sqliteDataAccess)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
        _sqliteDataAccess = sqliteDataAccess;
    }
    
    public async Task<List<ReactiveScheduledLecture>?> Handle(DisableConflictingLecturesCommand request, 
        CancellationToken cancellationToken)
    {
        var referenceScheduledLecture = await _scheduledLectureRepository
            .GetScheduledLectureById(request.ReferenceScheduledLectureId).ConfigureAwait(false);

        if (referenceScheduledLecture is null) return null;
        
        if (referenceScheduledLecture.IsScheduled == false) return null;

        var dayLectures = await _scheduledLectureRepository
            .GetScheduledLecturesByDay(referenceScheduledLecture.Day).ConfigureAwait(false);

        if (dayLectures is null) return null;

        var updateLecturesTasks = new List<Task>();
        await _sqliteDataAccess.BeginTransaction().ConfigureAwait(false);

        var updatedLectures = new List<ReactiveScheduledLecture>();
        foreach (var lecture in dayLectures)
        {
            if (lecture.Id != referenceScheduledLecture.Id && referenceScheduledLecture.OverlapsWithLecture(lecture))
            {
                lecture.IsScheduled = false;
                updateLecturesTasks.Add(_scheduledLectureRepository.UpdateScheduledLecture(lecture));
                updatedLectures.Add(lecture);
            }
        }

        await Task.WhenAll(updateLecturesTasks).ConfigureAwait(false);
        _sqliteDataAccess.CommitPendingTransaction();

        return updatedLectures;
    }
}