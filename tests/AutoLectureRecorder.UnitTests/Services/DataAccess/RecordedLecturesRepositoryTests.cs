using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.UnitTests.Fixture;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

[Collection("DatabaseCollection")]
public class RecordedLecturesRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<RecordedLecturesRepository> _logger;

    public RecordedLecturesRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _output = output;
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<RecordedLecturesRepository>(output);
    }

    [Fact]
    public async Task GetRecordedLecturesFromIdAsync_ShouldFetch()
    {
        var recordedLecturesRepository = new RecordedLecturesRepository(_fixture.DataAccess, _logger);

        var recordedLectures = await recordedLecturesRepository.GetRecordedLecturesFromIdAsync(1);
        
        Assert.Equal(4, recordedLectures?.Count);
    }
}
