using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.ScheduledLectures.Events;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.DeleteScheduledLectures;

public class DeleteScheduledLecturesHandler : IRequestHandler<DeleteScheduledLecturesCommand, List<int>>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly ISqliteDataAccess _dataAccess;
    private readonly IPublisher _mediatorPublisher;

    public DeleteScheduledLecturesHandler(IScheduledLectureRepository scheduledLectureRepository, 
        ISqliteDataAccess dataAccess, IPublisher mediatorPublisher)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
        _dataAccess = dataAccess;
        _mediatorPublisher = mediatorPublisher;
    }
    
    public async Task<List<int>> Handle(DeleteScheduledLecturesCommand command, CancellationToken cancellationToken)
    {
        if (command.LecturesToDelete.Count == 1)
        {
            var lectureId = command.LecturesToDelete[0].Id;
            await _scheduledLectureRepository.DeleteScheduledLectureById(lectureId)
                .ConfigureAwait(false);
            await _mediatorPublisher.Publish(new NextScheduledLectureEvent(), cancellationToken)
                .ConfigureAwait(false);
            return new List<int> { lectureId };
        }
        
        await _dataAccess.BeginTransaction();

        var deletedLecturesIds = new List<int>();
        var deleteLecturesTasks = new List<Task>();

        foreach (var lectureToDelete in command.LecturesToDelete)
        {
            deletedLecturesIds.Add(lectureToDelete.Id);
            deleteLecturesTasks.Add(_scheduledLectureRepository.DeleteScheduledLectureById(lectureToDelete.Id));
        }

        await Task.WhenAll(deleteLecturesTasks);
        
        _dataAccess.CommitPendingTransaction();
        
        await _mediatorPublisher.Publish(new NextScheduledLectureEvent(), cancellationToken)
            .ConfigureAwait(false);

        return deletedLecturesIds;
    }
}