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
        Include(new ScheduledLectureValidator(scheduledLectureRepository, persistentValidationContext));
    }
}