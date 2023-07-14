using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using FluentValidation;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture;

public class UpdateScheduledLectureValidator : AbstractValidator<UpdateScheduledLectureCommand>
{
    public UpdateScheduledLectureValidator(IScheduledLectureRepository scheduledLectureRepository, 
        IPersistentValidationContext persistentValidationContext)
    {
        bool ignoreOverlappingLectures = false;
        
        var ignoreOverlappingLecturesObject = persistentValidationContext.GetValidationParameter(
            typeof(UpdateScheduledLectureCommand),
            ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures);

        if (ignoreOverlappingLecturesObject is not null)
        {
            ignoreOverlappingLectures = (bool)ignoreOverlappingLecturesObject;
        }
        
        Include(new ScheduledLectureValidator(scheduledLectureRepository, ignoreOverlappingLectures));
    }
}