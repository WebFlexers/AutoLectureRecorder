using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.DisableConflictingLectures;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture.Mapping;
using AutoLectureRecorder.Application.ScheduledLectures.Events;
using AutoLectureRecorder.Domain.Errors;
using AutoLectureRecorder.Domain.ReactiveModels;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture;

public class UpdateScheduledLectureHandler 
    : IRequestHandler<UpdateScheduledLectureCommand, ErrorOr<List<ReactiveScheduledLecture>?>>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly IMediator _mediator;

    public UpdateScheduledLectureHandler(IScheduledLectureRepository scheduledLectureRepository, IMediator mediator)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
        _mediator = mediator;
    }
    
    public async Task<ErrorOr<List<ReactiveScheduledLecture>?>> Handle(UpdateScheduledLectureCommand command, 
        CancellationToken cancellationToken)
    {
        var updatedSuccessfully = await _scheduledLectureRepository
            .UpdateScheduledLecture(command.MapToReactiveModel()).ConfigureAwait(false);

        if (updatedSuccessfully is false)
        {
            return Errors.ScheduledLectures.FailedToUpdate;
        }
        
        var conflictingLectures = await _mediator.Send(new DisableConflictingLecturesCommand(command.Id),
            CancellationToken.None).ConfigureAwait(false);

        // Now we need to recalculate the next scheduled lecture in case the newly
        // added one is earlier that the currently scheduled one
        await _mediator.Publish(new NextScheduledLectureEvent(), cancellationToken).ConfigureAwait(false);

        return conflictingLectures;
    }
}