using AutoLectureRecorder.Data.ReactiveModels;
using FluentValidation;

namespace AutoLectureRecorder.Services.DataAccess.Validation;

public class ReactiveScheduledLectureValidator : AbstractValidator<ReactiveScheduledLecture>
{
    private readonly List<ReactiveScheduledLecture>? _existingLectures;

    public ReactiveScheduledLectureValidator(List<ReactiveScheduledLecture>? existingLectures = null)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        _existingLectures = existingLectures;

        RuleFor(p => p.SubjectName)
            .NotEmpty()
                .WithMessage("The subject name can't be empty")
           .MaximumLength(150)
                .WithMessage("The subject name can't be longer than 150 characters")
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
            .NotNull()
                .WithMessage("The day can't be empty")
            .IsInEnum()
                .WithMessage("Invalid value for a day");


        RuleFor(p => p.StartTime)
            .NotNull()
                .WithMessage("Both start time and end time must be filled")
            .LessThan(p => p.EndTime)
                .WithMessage("The start time of a lecture can't be greater than the end time")
            .Must((lecture, startTime, context) => NotOverlapWithOtherLectures(lecture.Day, startTime, lecture.EndTime, _existingLectures, context))
                .WithMessage("The specified timespan overlaps with {conflictingSubjectName}, " +
                             "which is scheduled at {conflictingDay} at " +
                             "{conflictingStartTime} - {conflictingEndTime}")
                .WithErrorCode(ScheduledLectureErrorCodes.OverlappingLecture);

        RuleFor(p => p.EndTime)
            .NotNull()
                .WithMessage("Both start time and end time must be filled");
    }

    public static bool NotOverlapWithOtherLectures(DayOfWeek? day, DateTime? startTime, DateTime? endTime, 
        List<ReactiveScheduledLecture>? existingLectures, ValidationContext<ReactiveScheduledLecture>? context)
    {
        if (day == null) return true;

        if (existingLectures == null || existingLectures.Any() == false) return true;

        foreach (var existingLecture in existingLectures)
        {
            if (startTime <= existingLecture.EndTime && existingLecture.StartTime <= endTime)
            {
                if (context == null) return false;

                context.MessageFormatter.AppendArgument("conflictingSubjectName", 
                    existingLecture.SubjectName);
                context.MessageFormatter.AppendArgument("conflictingDay", 
                    existingLecture.Day!.Value.ToString());
                context.MessageFormatter.AppendArgument("conflictingStartTime",
                    existingLecture.StartTime!.Value.ToString("hh:mm:ss"));
                context.MessageFormatter.AppendArgument("conflictingEndTime",
                    existingLecture.EndTime!.Value.ToString("hh:mm:ss"));

                return false;
            }
        }

        return true;
    }
}