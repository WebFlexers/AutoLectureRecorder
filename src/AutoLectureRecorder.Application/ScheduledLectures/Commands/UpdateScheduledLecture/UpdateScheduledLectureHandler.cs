using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Domain.ReactiveModels;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture;

public class UpdateScheduledLectureHandler 
    : IRequestHandler<UpdateScheduledLectureCommand, ErrorOr<ReactiveScheduledLecture>>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    public UpdateScheduledLectureHandler(IScheduledLectureRepository scheduledLectureRepository)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
    }
    
    public Task<ErrorOr<ReactiveScheduledLecture>> Handle(UpdateScheduledLectureCommand request, CancellationToken cancellationToken)
    {
        // TODO: Deactivate conflicting lectures if there are any
        throw new NotImplementedException();
    }
}