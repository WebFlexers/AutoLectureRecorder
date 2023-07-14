using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using FluentValidation;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture;

public class CreateScheduledLectureValidator : AbstractValidator<CreateScheduledLectureCommand>
{
    public CreateScheduledLectureValidator(IScheduledLectureRepository scheduledLectureRepository,
        IPersistentValidationContext persistentValidationContext)
    {
        bool ignoreOverlappingLectures = false;
        
        var ignoreOverlappingLecturesObject = persistentValidationContext.GetValidationParameter(
            typeof(CreateScheduledLectureCommand),
            ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures);

        if (ignoreOverlappingLecturesObject is not null)
        {
            ignoreOverlappingLectures = (bool)ignoreOverlappingLecturesObject;
        }
        
        Include(new ScheduledLectureValidator(scheduledLectureRepository, ignoreOverlappingLectures));
    }
}