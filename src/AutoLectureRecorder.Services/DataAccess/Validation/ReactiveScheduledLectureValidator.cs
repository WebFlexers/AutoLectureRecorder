using AutoLectureRecorder.Data.ReactiveModels;
using System;
using FluentValidation;

namespace AutoLectureRecorder.Services.DataAccess.Validation;

public class ReactiveScheduledLectureValidator : AbstractValidator<ReactiveScheduledLecture>
{
	public ReactiveScheduledLectureValidator(IScheduledLectureData lectureData)
	{
		RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        List<ReactiveScheduledLecture>? existingLectures = null;
		ReactiveScheduledLecture? overlappingLecture = null;

        RuleFor(p => p.SubjectName).NotEmpty().WithMessage("The subject name can't be empty")
								   .MaximumLength(150).WithMessage("The subject name can't be longer than 150 characters")
                                   .Must(x => x.Any(char.IsLetter)).WithMessage("PropertyName must contain at least one letter.");

		RuleFor(p => p.Semester).NotEmpty().WithMessage("The semester can't be empty")
								.GreaterThanOrEqualTo(1).WithMessage("Semesters can only be between 1 and 10")
								.LessThanOrEqualTo(10).WithMessage("Semesters can only be between 1 and 10");

		RuleFor(p => p.MeetingLink)
			.NotEmpty().WithMessage("The meeting link can't be empty")
			.Must(x => x.Contains("teams.microsoft.com"))
				.WithMessage("The provided value couldn't be determined to be a valid Microsoft Teams meeting or team link");

		RuleFor(p => p.Day).IsInEnum().WithMessage("Invalid value for a day");

		RuleFor(p => p.StartTime).NotNull().WithMessage("The start time can't be null");
		RuleFor(p => p.EndTime).NotNull().WithMessage("The end time can't be null");

		RuleFor(p => p.StartTime).LessThan(p => p.EndTime).WithMessage("The start time of a lecture can't be greater than the end time")
								 .MustAsync(async (lecture, startTime, _) =>
								 {
									 if (existingLectures == null)
									 {
										 existingLectures = await lectureData.GetScheduledLecturesByDayAsync(lecture.Day);
										 if (existingLectures.Any() == false)
										 {
											 return true;
										 }
									 }

									 return NotOverlapWithOtherLectures(startTime, lecture.EndTime, existingLectures);

								 }).WithMessage("The specified start time and end time overlap with an already existing lecture.");
    }

	private bool NotOverlapWithOtherLectures(DateTime? startTime, DateTime? endTime, List<ReactiveScheduledLecture> otherLectures)
	{
        foreach (var existingLecture in otherLectures)
        {
            if (startTime <= existingLecture.EndTime && existingLecture.StartTime <= endTime)
            {
				return false;
            }
        }

		return true;
    }
}
