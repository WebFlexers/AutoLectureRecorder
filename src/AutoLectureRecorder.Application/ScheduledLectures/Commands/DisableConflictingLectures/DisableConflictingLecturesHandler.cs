using System.Reactive;
using AutoLectureRecorder.Application.Common;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.DisableConflictingLectures;

public class DisableConflictingLecturesHandler : MediatR.IRequestHandler<DisableConflictingLecturesCommand, Unit>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly ISqliteDataAccess _sqliteDataAccess;

    public DisableConflictingLecturesHandler(IScheduledLectureRepository scheduledLectureRepository,
        ISqliteDataAccess sqliteDataAccess)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
        _sqliteDataAccess = sqliteDataAccess;
    }
    
    public async Task<Unit> Handle(DisableConflictingLecturesCommand request, CancellationToken cancellationToken)
    {
        var referenceScheduledLecture = await _scheduledLectureRepository
            .GetScheduledLectureById(request.ReferenceScheduledLectureId).ConfigureAwait(false);

        if (referenceScheduledLecture is null) return Unit.Default;
        
        if (referenceScheduledLecture.IsScheduled == false) return Unit.Default;

        var dayLectures = await _scheduledLectureRepository
            .GetScheduledLecturesByDay(referenceScheduledLecture.Day).ConfigureAwait(false);

        if (dayLectures == null || dayLectures.Any() == false) return Unit.Default;

        var updateLecturesTasks = new List<Task>();
        await _sqliteDataAccess.BeginTransaction().ConfigureAwait(false);
        
        foreach (var lecture in dayLectures)
        {
            if (referenceScheduledLecture.OverlapsWithLecture(lecture))
            {
                lecture.IsScheduled = false;
                updateLecturesTasks.Add(_scheduledLectureRepository.UpdateScheduledLecture(lecture));
            }
        }

        await Task.WhenAll(updateLecturesTasks).ConfigureAwait(false);
        _sqliteDataAccess.CommitPendingTransaction();

        return Unit.Default;
    }
}