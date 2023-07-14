using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture.Mapping;
using AutoLectureRecorder.Domain.Errors;
using AutoLectureRecorder.Domain.ReactiveModels;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture;

public class CreateScheduledLectureHandler 
    : IRequestHandler<CreateScheduledLectureCommand, ErrorOr<ReactiveScheduledLecture>>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    public CreateScheduledLectureHandler(IScheduledLectureRepository scheduledLectureRepository)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
    }
    
    public async Task<ErrorOr<ReactiveScheduledLecture>> Handle(CreateScheduledLectureCommand request, 
        CancellationToken cancellationToken)
    {
        var reactiveScheduledLecture = await _scheduledLectureRepository
            .InsertScheduledLecture(request.MapToReactiveModel());

        if (reactiveScheduledLecture is null)
        {
            return Errors.ScheduledLectures.FailedToCreate;
        }
        
        // TODO: Deactivate conflicting lectures if there are any

        return reactiveScheduledLecture;
    }
}