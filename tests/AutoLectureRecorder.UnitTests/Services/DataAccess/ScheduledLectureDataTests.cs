using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.UnitTests.Fixture;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

[Collection("DatabaseCollection")]
public class ScheduledLectureDataTests
{
    private readonly ITestOutputHelper _output;
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<ScheduledLectureRepository> _logger;

    public ScheduledLectureDataTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _output = output;
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<ScheduledLectureRepository>(output);
    }

    [Fact]
    public async Task GetScheduledLectureById_ShouldFetchTheScheduledLectureWithTheId()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var fetchedScheduleLecture = await scheduledLectureData.GetScheduledLectureByIdAsync(5);

        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].SubjectName, fetchedScheduleLecture?.SubjectName);
        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].Semester, fetchedScheduleLecture?.Semester);
        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].MeetingLink, fetchedScheduleLecture?.MeetingLink);
        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].Day, Convert.ToInt32(fetchedScheduleLecture?.Day));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].StartTime, fetchedScheduleLecture?.StartTime?.ToString("HH:mm"));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].EndTime, fetchedScheduleLecture?.EndTime?.ToString("HH:mm"));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].IsScheduled, Convert.ToInt32(fetchedScheduleLecture?.IsScheduled));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[5].WillAutoUpload, Convert.ToInt32(fetchedScheduleLecture?.WillAutoUpload));
    }

    [Fact]
    public async Task GetAllScheduledLecturesSortedAsync_ShouldReturnAListOfSortedLecturesByDayAndStartTime()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var results = await scheduledLectureData.GetAllScheduledLecturesSortedAsync();

        var actualScheduledLecturesSorted = _fixture.SampleData.ScheduledLectures
            .OrderBy(lecture => lecture.Day)
            .ThenBy(lecture => lecture.StartTime)
            .ToList();

        Assert.NotNull(results);
        Assert.Equal(_fixture.SampleData.ScheduledLectures.Count, results.Count);

        for (int i = 0; i < actualScheduledLecturesSorted.Count; i++)
        {
            Assert.Equal(actualScheduledLecturesSorted[i].Id, results[i].Id);
        }
    }

    [Fact]
    public async Task GetAllScheduledLectures_ShouldReturnAListOfTheScheduledLectures()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var allScheduledLectures = await scheduledLectureData.GetAllScheduledLecturesAsync();

        Assert.NotNull(allScheduledLectures);
        Assert.True(allScheduledLectures.Count == _fixture.SampleData.ScheduledLectures.Count);
    }

    [Fact]
    public async Task GetScheduledLecturesByDay_ShouldReturnAListOfTheScheduledLecturesInTheDay()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDayAsync(DayOfWeek.Tuesday);
        Assert.NotNull(scheduledLectures);
        Assert.True(scheduledLectures.Count == _fixture.SampleData.ScheduledLectures
            .Where(lecture => lecture.Day == (int)DayOfWeek.Tuesday).ToList().Count);

        scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDayAsync(DayOfWeek.Monday);
        Assert.NotNull(scheduledLectures);
        Assert.True(scheduledLectures.Count == _fixture.SampleData.ScheduledLectures
            .Where(lecture => lecture.Day == (int)DayOfWeek.Monday).ToList().Count);
    }

    [Fact]
    public async Task GetDistinctScheduledLectureSubjectNames_ShouldFetchDistinctSubjectNames()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleLecturesDistinctBySubjectName = _fixture.SampleData.ScheduledLectures
            .DistinctBy(lecture => lecture.SubjectName)
            .ToList();

        var results = await scheduledLectureData.GetDistinctSubjectNamesAsync();

        Assert.NotNull(results);
        Assert.Equal(sampleLecturesDistinctBySubjectName.Count, results.Count);
    }

    [Fact]
    public async Task GetDistinctScheduledLecturesByName_ShouldFetchAllScheduledLecturesWithDistinctNames()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleLecturesDistinctBySubjectName = _fixture.SampleData.ScheduledLectures
            .DistinctBy(lecture => lecture.SubjectName)
            .ToList();

        var results = await scheduledLectureData.GetScheduledLecturesGroupedByNameAsync();

        Assert.NotNull(results);
        Assert.Equal(sampleLecturesDistinctBySubjectName.Count, results.Count);
    }

    [Fact]
    public async Task InsertScheduledLecture_ShouldInsertAScheduledLecture()
    {
        await _fixture.DataAccess.BeginTransaction();

        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleReactiveScheduledLecture =
            ConvertScheduledLectureToReactive(_fixture.SampleData.ScheduledLectures.First());

        var insertedScheduledLecture = await scheduledLectureRepository
            .InsertScheduledLectureAsync(sampleReactiveScheduledLecture);

        Assert.Equal(sampleReactiveScheduledLecture.SubjectName, insertedScheduledLecture?.SubjectName);
        Assert.Equal(sampleReactiveScheduledLecture.Semester, insertedScheduledLecture?.Semester);
        Assert.Equal(sampleReactiveScheduledLecture.MeetingLink, insertedScheduledLecture?.MeetingLink);
        Assert.Equal(sampleReactiveScheduledLecture.Day, insertedScheduledLecture?.Day);
        Assert.Equal(sampleReactiveScheduledLecture.StartTime, insertedScheduledLecture?.StartTime);
        Assert.Equal(sampleReactiveScheduledLecture.EndTime, insertedScheduledLecture?.EndTime);
        Assert.Equal(sampleReactiveScheduledLecture.IsScheduled, insertedScheduledLecture?.IsScheduled);
        Assert.Equal(sampleReactiveScheduledLecture.WillAutoUpload, insertedScheduledLecture?.WillAutoUpload);

        _fixture.DataAccess.RollbackPendingTransaction();
    }

    [Fact]
    public async Task UpdateScheduledLecture_ShouldUpdateTheScheduledLecture()
    {
        var lectureToUpdate = ConvertScheduledLectureToReactive(_fixture.SampleData.ScheduledLectures.First());

        await _fixture.DataAccess.BeginTransaction();
        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        lectureToUpdate.SubjectName = "Updated name";
        lectureToUpdate.MeetingLink = "It's a new link";

        var isSuccessful = await scheduledLectureRepository.UpdateScheduledLectureAsync(lectureToUpdate);

        string sql = "select * from ScheduledLectures where Id=@Id";
        var result = await _fixture.DataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Id = lectureToUpdate.Id });

        Assert.True(isSuccessful);
        var updatedLectureInDb = result.First();
        Assert.NotNull(updatedLectureInDb);
        Assert.Equal("Updated name", updatedLectureInDb.SubjectName);
        Assert.Equal("It's a new link", updatedLectureInDb.MeetingLink);

        _fixture.DataAccess.RollbackPendingTransaction();
    }

    private static ReactiveScheduledLecture ConvertScheduledLectureToReactive(ScheduledLecture scheduledLecture)
    {
        return new ReactiveScheduledLecture
        {
            Id = scheduledLecture.Id,
            SubjectName = scheduledLecture.SubjectName,
            Semester = scheduledLecture.Semester,
            MeetingLink = scheduledLecture.MeetingLink,
            Day = (DayOfWeek)scheduledLecture.Day,
            StartTime = Convert.ToDateTime(scheduledLecture.StartTime),
            EndTime = Convert.ToDateTime(scheduledLecture.EndTime),
            IsScheduled = Convert.ToBoolean(scheduledLecture.IsScheduled),
            WillAutoUpload = Convert.ToBoolean(scheduledLecture.WillAutoUpload),
        };
    }
}
