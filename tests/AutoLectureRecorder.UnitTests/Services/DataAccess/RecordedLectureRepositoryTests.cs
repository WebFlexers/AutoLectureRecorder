using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.UnitTests.Fixture;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

[Collection("DatabaseCollection")]
public class RecordedLectureRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<RecordedLectureRepository> _logger;

    public RecordedLectureRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _output = output;
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<RecordedLectureRepository>(output);
    }

    [Fact]
    public async Task GetRecordedLecturesFromIdAsync_ShouldFetchSuccessfully()
    {
        var recordedLecturesRepository = new RecordedLectureRepository(_fixture.DataAccess, _logger);

        var recordedLectures = await recordedLecturesRepository.GetRecordedLecturesFromIdAsync(1);
        
        Assert.Equal(4, recordedLectures?.Count);
    }

    [Fact]
    public async Task InsertRecordedLecture_ShouldInsertSuccessfully()
    {
        await _fixture.DataAccess.BeginTransaction();

        var recordedLectureRepository = new RecordedLectureRepository(_fixture.DataAccess, _logger);

        var sampleRecordedLecture = new RecordedLecture
        {
            Id = _fixture.SampleData.RecordedLectures.Last().Id + 1,
            StudentRegistrationNumber = "p19165",
            StartedAt = "04/01/2021 10:14:00 pm",
            EndedAt = "04/01/2021 12:14:00 pm",
            ScheduledLectureId = _fixture.SampleData.ScheduledLectures.Last().Id
        };

        var result = await recordedLectureRepository.InsertRecordedLecture(sampleRecordedLecture);

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.NotNull(result);
        Assert.Equal(sampleRecordedLecture.Id, result.Id);
        Assert.Equal(sampleRecordedLecture.StudentRegistrationNumber, result.StudentRegistrationNumber);
        Assert.Equal(sampleRecordedLecture.StartedAt, result.StartedAt);
        Assert.Equal(sampleRecordedLecture.EndedAt, result.EndedAt);
        Assert.Equal(sampleRecordedLecture.ScheduledLectureId, result.ScheduledLectureId);
    }
}
