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
        Include(new ScheduledLectureValidator(scheduledLectureRepository, persistentValidationContext));
    }
}