using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.Services.DataAccess.Validation;
using AutoLectureRecorder.UnitTests.Services.DataAccess.Validation.DataAccessMocks;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess.Validation;

public class ReactiveScheduledLectureValidatorTests
{
    private readonly ITestOutputHelper _output;
    private ReactiveScheduledLectureValidator _validator;

	public ReactiveScheduledLectureValidatorTests(ITestOutputHelper testOutputHelper)
	{
        _validator = new ReactiveScheduledLectureValidator(
						new ScheduledLectureData(
							new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration())));

        _output = testOutputHelper;
    }

	[Fact]
	public async Task ShouldNotHaveAnyErrors()
	{
        var model = new ReactiveScheduledLecture
        {
            Id = 1,
            SubjectName = "Αλληλεπίδραση Ανθρώπου - Υπολογιστή",
            Semester = 5,
            MeetingLink = "https://teams.microsoft.com/l/team/19%3a4f80471db6464f18a64f47be0dcd660d%40thread.tacv2/conversations?groupId=29a0e98c-f210-4df8-afe9-a9f9f6d02264&tenantId=d9c8dee3-558b-483d-b502-d31fa0cb24de",
            Day = DayOfWeek.Sunday,
            StartTime = default(DateTime).AddHours(1).AddMinutes(15),
            EndTime = default(DateTime).AddHours(3).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = true,
        };
        var result = await _validator.ValidateAsync(model);

        if (result.IsValid == false)
        {
            _output.WriteLine($"Error message: {result.Errors.First().ErrorMessage}");
        }

        Assert.True(result.IsValid);
    }

	[Fact]
	public async Task ShouldHaveErrorWhenNameIsNull()
	{
		var model = new ReactiveScheduledLecture { SubjectName = null };
		var result = await _validator.ValidateAsync(model);

		Assert.Single(result.Errors);

		_output.WriteLine($"Error message: {result.Errors.First().ErrorMessage}");
    }

    [Fact]
    public async Task ShouldHaveErrorWhenStartTimeIsGreaterThanEndTime()
    {
        var model = new ReactiveScheduledLecture
        {
            SubjectName = "Αλληλεπίδραση Ανθρώπου - Υπολογιστή",
            Semester = 5,
            MeetingLink = "https://teams.microsoft.com/l/team/19%3a4f80471db6464f18a64f47be0dcd660d%40thread.tacv2/conversations?groupId=29a0e98c-f210-4df8-afe9-a9f9f6d02264&tenantId=d9c8dee3-558b-483d-b502-d31fa0cb24de",
            Day = DayOfWeek.Sunday,
            StartTime = default(DateTime).AddHours(3).AddMinutes(15),
            EndTime = default(DateTime).AddHours(2).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = true,
        };

        var result = await _validator.ValidateAsync(model);

        if (result.IsValid == false)
        {
            _output.WriteLine($"Error message: {result.Errors.First().ErrorMessage}");
        }

        Assert.False(result.IsValid);
        Assert.Contains("greater", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task ShouldHaveErrorWhenTimeIsNull()
    {
        var model = new ReactiveScheduledLecture
        {
            SubjectName = "Αλληλεπίδραση Ανθρώπου - Υπολογιστή",
            Semester = 5,
            MeetingLink = "https://teams.microsoft.com/l/team/19%3a4f80471db6464f18a64f47be0dcd660d%40thread.tacv2/conversations?groupId=29a0e98c-f210-4df8-afe9-a9f9f6d02264&tenantId=d9c8dee3-558b-483d-b502-d31fa0cb24de",
            Day = DayOfWeek.Sunday,
            IsScheduled = true,
            WillAutoUpload = true,
        };

        var result = await _validator.ValidateAsync(model);

        if (result.IsValid == false)
        {
            _output.WriteLine($"Error message: {result.Errors.First().ErrorMessage}");
        }

        Assert.False(result.IsValid);
        Assert.Contains("null", result.Errors.First().ErrorMessage);
    }

    [Theory]
    [InlineData(13, 40, 15, 40,
            14, 40, 16, 40)]

    [InlineData(14, 40, 16, 40,
            13, 40, 15, 40)]

    [InlineData(14, 40, 15, 40,
            13, 40, 16, 40)]

    [InlineData(13, 40, 16, 40,
            14, 40, 15, 40)]

    [InlineData(13, 40, 15, 40,
            15, 40, 17, 40)]
    public async Task ShouldHaveErrorWhenTimesAreConflicting(int startTime1Hours, int startTime1Minutes, int endTime1Hours, int endTime1Minutes,
                                                             int startTime2Hours, int startTime2Minutes, int endTime2Hours, int endTime2Minutes)
    {
        var lectures = new List<ReactiveScheduledLecture>
        {
            new ReactiveScheduledLecture
            {
                SubjectName = "Αλληλεπίδραση Ανθρώπου - Υπολογιστή",
                Semester = 5,
                MeetingLink = "https://teams.microsoft.com/l/team/19%3a4f80471db6464f18a64f47be0dcd660d%40thread.tacv2/conversations?groupId=29a0e98c-f210-4df8-afe9-a9f9f6d02264&tenantId=d9c8dee3-558b-483d-b502-d31fa0cb24de",
                Day = DayOfWeek.Sunday,
                StartTime = default(DateTime).AddHours(startTime1Hours).AddMinutes(startTime1Minutes),
                EndTime = default(DateTime).AddHours(endTime1Hours).AddMinutes(endTime1Minutes),
                IsScheduled = true,
                WillAutoUpload = false,
            },
        };

        var conflictingLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Κατι άλλοοο",
            Semester = 3,
            MeetingLink = "https://teams.microsoft.com/l/team/19%3a4f80471db6464f18a64f47be0dcd660d%40thread.tacv2/conversations?groupId=29a0e98c-f210-4df8-afe9-a9f9f6d02264&tenantId=d9c8dee3-558b-483d-b502-d31fa0cb24de",
            Day = DayOfWeek.Sunday,
            StartTime = default(DateTime).AddHours(startTime2Hours).AddMinutes(startTime2Minutes),
            EndTime = default(DateTime).AddHours(endTime2Hours).AddMinutes(endTime2Minutes),
            IsScheduled = true,
            WillAutoUpload = false,
        };

        var newValidator = new ReactiveScheduledLectureValidator(new ScheduledLectureDataMock(lectures));

        var result = await newValidator.ValidateAsync(conflictingLecture);

        var errorMessage = result.Errors.First().ErrorMessage;
        Assert.False(result.IsValid);
        Assert.Contains("overlap", errorMessage);
    }
}
