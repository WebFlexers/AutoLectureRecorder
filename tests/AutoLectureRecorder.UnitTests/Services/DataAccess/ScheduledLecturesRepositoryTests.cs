using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Repositories;
using AutoLectureRecorder.UnitTests.Fixture;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

[Collection("DatabaseCollection")]
public class ScheduledLecturesRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<ScheduledLectureRepository> _logger;

    public ScheduledLecturesRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _output = output;
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<ScheduledLectureRepository>(output);
    }

    [Fact]
    public async Task GetScheduledLectureById_ShouldFetchSuccessfully()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var fetchedScheduleLecture = await scheduledLectureData.GetScheduledLectureById(5);

        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].SubjectName, fetchedScheduleLecture?.SubjectName);
        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].Semester, fetchedScheduleLecture?.Semester);
        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].MeetingLink, fetchedScheduleLecture?.MeetingLink);
        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].Day, Convert.ToInt32(fetchedScheduleLecture?.Day));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].StartTime, fetchedScheduleLecture?.StartTime?.ToString("HH:mm"));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].EndTime, fetchedScheduleLecture?.EndTime?.ToString("HH:mm"));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].IsScheduled, Convert.ToInt32(fetchedScheduleLecture?.IsScheduled));
        Assert.Equal(_fixture.SampleData.ScheduledLectures[4].WillAutoUpload, Convert.ToInt32(fetchedScheduleLecture?.WillAutoUpload));
    }

    [Fact]
    public async Task GetScheduledLecturesOrderedByDayAndStartTime_ShouldFetchSuccessfully()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var results = await scheduledLectureData.GetScheduledLecturesOrderedByDayAndStartTime();

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
    public async Task GetAllScheduledLectures_ShouldFetchSuccessfully()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var allScheduledLectures = await scheduledLectureData.GetAllScheduledLectures();

        Assert.NotNull(allScheduledLectures);
        Assert.True(allScheduledLectures.Count == _fixture.SampleData.ScheduledLectures.Count);
    }

    [Fact]
    public async Task GetScheduledLecturesByDay_ShouldFetchSuccessfully()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDay(DayOfWeek.Tuesday);
        Assert.NotNull(scheduledLectures);
        Assert.True(scheduledLectures.Count == _fixture.SampleData.ScheduledLectures
            .Where(lecture => lecture.Day == (int)DayOfWeek.Tuesday).ToList().Count);

        scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDay(DayOfWeek.Monday);
        Assert.NotNull(scheduledLectures);
        Assert.True(scheduledLectures.Count == _fixture.SampleData.ScheduledLectures
            .Where(lecture => lecture.Day == (int)DayOfWeek.Monday).ToList().Count);
    }

    [Fact]
    public async Task GetDistinctScheduledLectureSubjectNames_ShouldFetchSuccessfully()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleLecturesDistinctBySubjectName = _fixture.SampleData.ScheduledLectures
            .DistinctBy(lecture => lecture.SubjectName)
            .ToList();

        var results = await scheduledLectureData.GetDistinctSubjectNames();

        Assert.NotNull(results);
        Assert.Equal(sampleLecturesDistinctBySubjectName.Count, results.Count);
    }

    [Fact]
    public async Task GetScheduledLecturesGroupedByName_ShouldFetchSuccessfully()
    {
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleLecturesDistinctBySubjectName = _fixture.SampleData.ScheduledLectures
            .DistinctBy(lecture => lecture.SubjectName)
            .ToList();

        var results = await scheduledLectureData.GetScheduledLecturesGroupedByName();

        Assert.NotNull(results);
        Assert.Equal(sampleLecturesDistinctBySubjectName.Count, results.Count);
    }

    [Fact]
    public async Task GetScheduledLecturesOrderedBySemester_ShouldFetchSuccessfully()
    {
        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sortedLectures = await scheduledLectureRepository.GetScheduledLecturesOrderedBySemester();

        Assert.NotNull(sortedLectures);

        for (int i = 1; i < sortedLectures.Count; i++)
        {
            Assert.True(sortedLectures[i].Semester >= sortedLectures[i-1].Semester);
        }
    }

    [Fact]
    public async Task InsertScheduledLecture_ShouldInsertSuccessfully()
    {
        await _fixture.DataAccess.BeginTransaction();

        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleReactiveScheduledLecture =
            ConvertScheduledLectureToReactive(_fixture.SampleData.ScheduledLectures.First());

        var insertedScheduledLecture = await scheduledLectureRepository
            .InsertScheduledLecture(sampleReactiveScheduledLecture);

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.Equal(sampleReactiveScheduledLecture.SubjectName, insertedScheduledLecture?.SubjectName);
        Assert.Equal(sampleReactiveScheduledLecture.Semester, insertedScheduledLecture?.Semester);
        Assert.Equal(sampleReactiveScheduledLecture.MeetingLink, insertedScheduledLecture?.MeetingLink);
        Assert.Equal(sampleReactiveScheduledLecture.Day, insertedScheduledLecture?.Day);
        Assert.Equal(sampleReactiveScheduledLecture.StartTime, insertedScheduledLecture?.StartTime);
        Assert.Equal(sampleReactiveScheduledLecture.EndTime, insertedScheduledLecture?.EndTime);
        Assert.Equal(sampleReactiveScheduledLecture.IsScheduled, insertedScheduledLecture?.IsScheduled);
        Assert.Equal(sampleReactiveScheduledLecture.WillAutoUpload, insertedScheduledLecture?.WillAutoUpload);
    }

    [Fact]
    public async Task UpdateScheduledLecture_ShouldUpdateSuccessfully()
    {
        var lectureToUpdate = ConvertScheduledLectureToReactive(_fixture.SampleData.ScheduledLectures.First());

        await _fixture.DataAccess.BeginTransaction();
        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        lectureToUpdate.SubjectName = "Updated name";
        lectureToUpdate.MeetingLink = "It's a new link";

        var isSuccessful = await scheduledLectureRepository.UpdateScheduledLecture(lectureToUpdate);

        string sql = "select * from ScheduledLectures where Id=@Id";
        var result = await _fixture.DataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Id = lectureToUpdate.Id });

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.True(isSuccessful);
        var updatedLectureInDb = result.First();
        Assert.NotNull(updatedLectureInDb);
        Assert.Equal("Updated name", updatedLectureInDb.SubjectName);
        Assert.Equal("It's a new link", updatedLectureInDb.MeetingLink);
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
