using AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;
using AutoLectureRecorder.Infrastructure.Persistence.Seeding;
using AutoLectureRecorder.Infrastructure.Persistence.UnitTests.TestUtils.Fixture;
using CommonTestsUtils.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.Infrastructure.Persistence.UnitTests.Persistence;

[Collection("DatabaseCollection")]
public class ScheduledLectureRepositoryTests
{
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<ScheduledLectureRepository> _logger;
    private readonly SampleData _sampleData;

    public ScheduledLectureRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<ScheduledLectureRepository>(output);
        _sampleData = _fixture.SampleData;
    }
    
    [Fact]
    public async Task GetScheduledLectureById_WhenIdIs5_ShouldFetchSuccessfully()
    {
        // Arrange
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        // Act
        var fetchedScheduleLecture = await scheduledLectureData.GetScheduledLectureById(5);

        // Assert
        fetchedScheduleLecture.Should().NotBeNull();

        fetchedScheduleLecture!.SubjectName.Should().Be(_sampleData.ScheduledLectures[4].SubjectName);
        fetchedScheduleLecture.Semester.Should().Be(_sampleData.ScheduledLectures[4].Semester);
        fetchedScheduleLecture.Day.Should().Be((DayOfWeek)_sampleData.ScheduledLectures[4].Day);
        fetchedScheduleLecture.StartTime.Should().Be(TimeOnly.Parse(_sampleData.ScheduledLectures[4].StartTime));
        fetchedScheduleLecture.EndTime.Should().Be(TimeOnly.Parse(_sampleData.ScheduledLectures[4].EndTime));
        fetchedScheduleLecture.IsScheduled.Should().Be(Convert.ToBoolean(_sampleData.ScheduledLectures[4].IsScheduled));
        fetchedScheduleLecture.WillAutoUpload.Should().Be(Convert.ToBoolean(_sampleData.ScheduledLectures[4].IsScheduled));
    }
    
    [Fact]
    public async Task GetScheduledLecturesOrderedByDayStartTimeAndSubjectName_ShouldFetchSuccessfully()
    {
        // Arrange
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var actualScheduledLecturesSorted = _fixture.SampleData.ScheduledLectures
            .OrderBy(lecture => lecture.Day)
            .ThenBy(lecture => lecture.StartTime)
            .ThenBy(lecture => lecture.SubjectName)
            .ToList();
        
        // Act
        var results = (await scheduledLectureData
            .GetScheduledLecturesOrdered())?.ToList();

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(_sampleData.ScheduledLectures.Count);

        for (int i = 0; i < actualScheduledLecturesSorted.Count; i++)
        {
            actualScheduledLecturesSorted[i].Id.Should().Be(results![i].Id);
        }
    }
    
    [Fact]
    public async Task GetAllScheduledLectures_ShouldFetchSuccessfully()
    {
        // Arrange
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        // Act
        var allScheduledLectures = 
            (await scheduledLectureData.GetAllScheduledLectures())?.ToList();

        // Assert
        allScheduledLectures.Should().NotBeNull();
        allScheduledLectures.Should().HaveCount(_sampleData.ScheduledLectures.Count);
    }
    
    [Fact]
    public async Task GetDistinctScheduledLectureSubjectNames_ShouldFetchSuccessfully()
    {
        // Arrange
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleLecturesDistinctBySubjectName = _fixture.SampleData.ScheduledLectures
            .DistinctBy(lecture => lecture.SubjectName)
            .ToList();

        // Act
        var results = (await scheduledLectureData.GetDistinctSubjectNames())?.ToList();
        
        // Assert
        results.Should().NotBeNull();
        sampleLecturesDistinctBySubjectName.Should().HaveCount(results!.Count);
    }
    
    [Fact]
    public async Task GetScheduledLecturesGroupedByName_ShouldFetchSuccessfully()
    {
        // Arrange
        var scheduledLectureData = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        var sampleLecturesDistinctBySubjectName = _fixture.SampleData.ScheduledLectures
            .DistinctBy(lecture => lecture.SubjectName)
            .ToList();

        // Act
        var results = 
            (await scheduledLectureData.GetScheduledLecturesGroupedByName())?.ToList();

        // Assert
        results.Should().NotBeNull();
        sampleLecturesDistinctBySubjectName.Should().HaveCount(results!.Count);
    }
    
    [Fact]
    public async Task GetScheduledLecturesOrderedBySemester_ShouldFetchSuccessfully()
    {
        // Arrange
        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        // Act
        var sortedLectures = 
            (await scheduledLectureRepository.GetScheduledLecturesOrderedBySemester())?.ToList();

        // Assert
        sortedLectures.Should().NotBeNull();

        for (int i = 1; i < sortedLectures!.Count; i++)
        {
            sortedLectures[i].Semester.Should().BeGreaterOrEqualTo(sortedLectures[i - 1].Semester);
        }
    }
    
    [Fact]
    public async Task InsertScheduledLecture_ShouldInsertSuccessfully()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        var sampleReactiveScheduledLecture = _fixture.SampleData.ScheduledLectures.First().MapToReactive();
        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);
        
        // Act
        var insertedScheduledLecture = await scheduledLectureRepository
            .InsertScheduledLecture(sampleReactiveScheduledLecture);
        
        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
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
        // Arrange
        var lectureToUpdate = _fixture.SampleData.ScheduledLectures.First().MapToReactive();

        await _fixture.DataAccess.BeginTransaction();
        var scheduledLectureRepository = new ScheduledLectureRepository(_fixture.DataAccess, _logger);

        string newSubjectName = "Updated name";
        string newMeetingLink = "It's a new link";
        lectureToUpdate.SubjectName = newSubjectName;
        lectureToUpdate.MeetingLink = newMeetingLink;

        // Act
        bool isSuccessful = await scheduledLectureRepository.UpdateScheduledLecture(lectureToUpdate);

        string sql = "select * from ScheduledLectures where Id=@Id";
        var result = await _fixture.DataAccess.LoadData<ScheduledLecture, dynamic>(
            sql, new { Id = lectureToUpdate.Id });

        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        isSuccessful.Should().BeTrue();
        
        var updatedLectureInDb = result.First();

        updatedLectureInDb.Should().NotBeNull();
        newSubjectName.Should().Be(updatedLectureInDb.SubjectName);
        newMeetingLink.Should().Be(updatedLectureInDb.MeetingLink);
    }
}