using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using FluentValidation;

namespace AutoLectureRecorder.Pages.MainMenu.CreateLecture;

public class ValidatableScheduledLectureValidator : AbstractValidator<ValidatableScheduledLecture>
{
    public ValidatableScheduledLectureValidator(IScheduledLectureRepository scheduledLectureRepository,
        IPersistentValidationContext persistentValidationContext)
    {
        Include(new ScheduledLectureValidator(scheduledLectureRepository, persistentValidationContext));
    }
}