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
    
    public async Task<List<ReactiveScheduledLecture>?> Handle(DisableConflictingLecturesCommand command, 
        CancellationToken cancellationToken)
    {
        var referenceScheduledLecture = await _scheduledLectureRepository
            .GetScheduledLectureById(command.ReferenceScheduledLectureId).ConfigureAwait(false);

        if (referenceScheduledLecture is null) return null;
        
        if (referenceScheduledLecture.IsScheduled == false) return null;

        var dayLectures = await _scheduledLectureRepository
            .GetScheduledLecturesByDay(referenceScheduledLecture.Day).ConfigureAwait(false);

        if (dayLectures is null) return null;

        var updateLecturesTasks = new List<Task>();
        await _sqliteDataAccess.BeginTransaction().ConfigureAwait(false);

        var deactivatedLectures = new List<ReactiveScheduledLecture>();
        foreach (var existingLecture in dayLectures)
        {
            if (existingLecture.Id != referenceScheduledLecture.Id && existingLecture.IsScheduled &&
                referenceScheduledLecture.OverlapsWithLecture(existingLecture))
            {
                existingLecture.IsScheduled = false;
                updateLecturesTasks.Add(_scheduledLectureRepository.UpdateScheduledLecture(existingLecture));
                deactivatedLectures.Add(existingLecture);
            }
        }

        await Task.WhenAll(updateLecturesTasks).ConfigureAwait(false);
        _sqliteDataAccess.CommitPendingTransaction();

        return deactivatedLectures.Count > 0 ? deactivatedLectures : null;
    }
}