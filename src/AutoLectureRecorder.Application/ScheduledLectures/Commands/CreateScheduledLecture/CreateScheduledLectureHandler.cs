using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture.Mapping;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.DisableConflictingLectures;
using AutoLectureRecorder.Domain.Errors;
using AutoLectureRecorder.Domain.ReactiveModels;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture;

public class CreateScheduledLectureHandler 
    : IRequestHandler<CreateScheduledLectureCommand, ErrorOr<ReactiveScheduledLecture>>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly ISender _mediatorSender;

    public CreateScheduledLectureHandler(IScheduledLectureRepository scheduledLectureRepository, ISender mediatorSender)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
        _mediatorSender = mediatorSender;
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
        
        if (request.IgnoreValidationWarnings)
        {
            await _mediatorSender.Send(new DisableConflictingLecturesCommand(reactiveScheduledLecture.Id),
                CancellationToken.None).ConfigureAwait(false);
        }

        return reactiveScheduledLecture;
    }
}