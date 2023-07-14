using AutoLectureRecorder.Application.Common;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Domain.ReactiveModels;
using FluentValidation;

namespace AutoLectureRecorder.Application.ScheduledLectures.Common;

public class ScheduledLectureValidator : AbstractValidator<IValidatableScheduledLecture>
{
    public ScheduledLectureValidator(IScheduledLectureRepository scheduledLectureRepository, 
        bool ignoreOverlappingLectures)
    {
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
            .Must(x => x.Contains("teams.microsoft.com"))
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
            .MustAsync(async (lecture, _, context, cancellationToken) =>
            {
                if (ignoreOverlappingLectures) return true;

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

    private static bool NotOverlapWithOtherLectures(IValidatableScheduledLecture lecture, 
        List<ReactiveScheduledLecture>? existingLectures, ValidationContext<IValidatableScheduledLecture>? context)
    {
        if (existingLectures is null || existingLectures.Any() == false) return true;

        foreach (var existingLecture in existingLectures)
        {
            if (lecture.OverlapsWithLecture(existingLecture))
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
}