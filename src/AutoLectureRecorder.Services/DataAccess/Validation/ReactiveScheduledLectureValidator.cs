using AutoLectureRecorder.Data.ReactiveModels;
using FluentValidation;

namespace AutoLectureRecorder.Services.DataAccess.Validation;

public class ReactiveScheduledLectureValidator : AbstractValidator<ReactiveScheduledLecture>
{
    private readonly IScheduledLectureData _lectureData;
    List<ReactiveScheduledLecture>? _existingLectures;

    public ReactiveScheduledLectureValidator(IScheduledLectureData lectureData)
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

        _lectureData = lectureData;
        List<ReactiveScheduledLecture>? existingLectures = null;

        RuleFor(p => p.SubjectName).NotEmpty().WithMessage("The subject name can't be empty")
								   .MaximumLength(150).WithMessage("The subject name can't be longer than 150 characters")
                                   .Must(x => x.Any(char.IsLetter)).WithMessage("The subject name must contain at least one letter.");

		RuleFor(p => p.Semester).NotEmpty().WithMessage("The semester can't be empty")
								.GreaterThanOrEqualTo(1).WithMessage("Semesters can only be between 1 and 10")
								.LessThanOrEqualTo(10).WithMessage("Semesters can only be between 1 and 10");

		RuleFor(p => p.MeetingLink)
			.NotEmpty().WithMessage("The meeting link can't be empty")
			.Must(x => x.Contains("teams.microsoft.com"))
				.WithMessage("The provided value couldn't be determined to be a valid Microsoft Teams meeting or team link");

		RuleFor(p => p.Day).NotNull().WithMessage("The day can't be empty")
                           .IsInEnum().WithMessage("Invalid value for a day");


		RuleFor(p => p.StartTime).NotNull().WithMessage("Both start time and end time must be filled")
                                 .LessThan(p => p.EndTime).WithMessage("The start time of a lecture can't be greater than the end time")
								 .MustAsync(async (lecture, startTime, _) =>
									  await NotOverlapWithOtherLectures(lecture.Day ,startTime, lecture.EndTime)
								 ).WithMessage("The specified start time and end time overlap with an already existing lecture.");

        RuleFor(p => p.EndTime).NotNull().WithMessage("Both start time and end time must be filled");
    }

	private async Task<bool> NotOverlapWithOtherLectures(DayOfWeek? day, DateTime? startTime, DateTime? endTime)
	{
        if (day == null)
        {
            return false;
        }

        _existingLectures = await _lectureData.GetScheduledLecturesByDayAsync(day);

        if (_existingLectures.Any() == false)
        {
            return true;
        }

        foreach (var existingLecture in _existingLectures)
        {
            if (startTime <= existingLecture.EndTime && existingLecture.StartTime <= endTime)
            {
				return false;
            }
        }

		return true;
    }
}
