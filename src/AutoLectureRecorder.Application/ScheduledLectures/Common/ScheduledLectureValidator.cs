using AutoLectureRecorder.Application.Common;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Domain.ReactiveModels;
using FluentValidation;

namespace AutoLectureRecorder.Application.ScheduledLectures.Common;

public class ScheduledLectureValidator : AbstractValidator<IValidatableScheduledLecture>
{
    private readonly IPersistentValidationContext _persistentValidationContext;

    public ScheduledLectureValidator(IScheduledLectureRepository scheduledLectureRepository, 
        IPersistentValidationContext persistentValidationContext)
    {
        _persistentValidationContext = persistentValidationContext;
        
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(p => p.SubjectName)
           .NotEmpty()
                .WithMessage("The subject name can't be empty")
           .MaximumLength(500)
                .WithMessage("The subject name can't be longer than 500 characters")
           .Must(x => x.Any(char.IsLetter))
                .WithMessage("The subject name must contain at least one letter.");

        RuleFor(p => p.Semester)
            .NotEmpty()
                .WithMessage("The semester can't be empty")
            .GreaterThanOrEqualTo(1)
                .WithMessage("Semesters can only be between 1 and 10")
            .LessThanOrEqualTo(10)
                .WithMessage("Semesters can only be between 1 and 10");

        RuleFor(p => p.MeetingLink)
            .NotEmpty()
                .WithMessage("The meeting link can't be empty")
            .Must(x => x!.Contains("teams.microsoft.com"))
                .WithMessage("The provided value couldn't be determined to be a valid Microsoft Teams meeting or team link");

        RuleFor(p => p.Day)
            .NotEmpty()
                .WithMessage("The day can't be empty")
            .IsInEnum()
                .WithMessage("Invalid value for a day");


        RuleFor(p => p.StartTime)
            .NotNull()
                .WithMessage("Both start time and end time must be filled")
            .LessThan(p => p.EndTime)
                .WithMessage("The start time of a lecture can't be greater than the end time")
            .MustAsync(async (lecture, _, context, _) =>
            {
                if (ShouldIgnoreOverlappingLectures()) return true;

                var existingLectures = await
                    scheduledLectureRepository.GetScheduledLecturesByDay(lecture.Day);
                return NotOverlapWithOtherLectures(lecture, existingLectures, context);
            })
                .WithMessage("The specified timespan overlaps with {conflictingSubjectName}, " +
                         "which is scheduled at {conflictingDay} at " +
                         "{conflictingStartTime} - {conflictingEndTime}")
                .WithErrorCode("OverlappingLecture");

        RuleFor(p => p.EndTime)
            .NotEmpty()
                .WithMessage("Both start time and end time must be filled");
    }

    private bool NotOverlapWithOtherLectures(IValidatableScheduledLecture lecture, 
        IEnumerable<ReactiveScheduledLecture>? existingLectures, ValidationContext<IValidatableScheduledLecture>? context)
    {
        if (existingLectures is null) return true;
        if (lecture.IsScheduled == false) return true;

        var isOnUpdateModeObject = _persistentValidationContext.GetValidationParameter(
            ValidationParameters.ScheduledLectures.IsOnUpdateMode);
        
        bool isOnUpdateMode = false;
        int lectureId = -1;
        if (isOnUpdateModeObject is not null)
        {
            isOnUpdateMode = (bool)isOnUpdateModeObject;
            if (isOnUpdateMode)
            {
                var lectureIdObject = _persistentValidationContext.GetValidationParameter(
                    ValidationParameters.ScheduledLectures.ScheduledLectureId);
                if (lectureIdObject is not null)
                {
                    lectureId = (int)lectureIdObject;
                }
            }
        }
        
        foreach (var existingLecture in existingLectures)
        {
            // Here we can safely cast to UpdateScheduledLectureCommand because when isOnUpdateMode is true
            // the validating object is an UpdateScheduledLectureCommand
            if (isOnUpdateMode && existingLecture.Id == lectureId) continue;
            
            if (existingLecture.IsScheduled && lecture.OverlapsWithLecture(existingLecture))
            {
                if (context == null) return false;

                context.MessageFormatter.AppendArgument("conflictingSubjectName", 
                    existingLecture.SubjectName);
                context.MessageFormatter.AppendArgument("conflictingDay", 
                    existingLecture.Day.ToString());
                context.MessageFormatter.AppendArgument("conflictingStartTime",
                    existingLecture.StartTime.ToString("hh:mm:ss"));
                context.MessageFormatter.AppendArgument("conflictingEndTime",
                    existingLecture.EndTime.ToString("hh:mm:ss"));

                return false;
            }
        }

        return true;
    }

    private bool ShouldIgnoreOverlappingLectures()
    {
        var ignoreOverlappingLecturesObject = _persistentValidationContext.GetValidationParameter(
            ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures);

        if (ignoreOverlappingLecturesObject is not null)
        {
            return (bool)ignoreOverlappingLecturesObject;
        }

        return false;
    }
}