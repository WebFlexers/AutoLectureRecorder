using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture.Mapping;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.DisableConflictingLectures;
using AutoLectureRecorder.Application.ScheduledLectures.Events;
using AutoLectureRecorder.Domain.Errors;
using AutoLectureRecorder.Domain.ReactiveModels;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture;

public class CreateScheduledLectureHandler 
    : IRequestHandler<CreateScheduledLectureCommand, ErrorOr<ReactiveScheduledLecture>>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly IMediator _mediator;

    public CreateScheduledLectureHandler(IScheduledLectureRepository scheduledLectureRepository, IMediator mediator)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
        _mediator = mediator;
    }
    
    public async Task<ErrorOr<ReactiveScheduledLecture>> Handle(CreateScheduledLectureCommand request, 
        CancellationToken cancellationToken)
    {
        var reactiveScheduledLecture = await _scheduledLectureRepository
            .InsertScheduledLecture(request.MapToReactiveModel()).ConfigureAwait(false);

        if (reactiveScheduledLecture is null)
        {
            return Errors.ScheduledLectures.FailedToCreate;
        }

        await _mediator.Send(new DisableConflictingLecturesCommand(reactiveScheduledLecture.Id),
            CancellationToken.None).ConfigureAwait(false);
        
        // Now we need to recalculate the next scheduled lecture in case the newly
        // added one is earlier that the currently scheduled one
        await _mediator.Publish(new NextScheduledLectureEvent(), cancellationToken).ConfigureAwait(false);

        return reactiveScheduledLecture;
    }
}