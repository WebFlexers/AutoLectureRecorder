using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Seeding;
using AutoLectureRecorder.UnitTests.Services.DataAccess;

namespace AutoLectureRecorder.UnitTests.Fixture;

public class DbInitializerFixture
{
    public ISqliteDataAccess DataAccess { get; set; }
    public SampleData SampleData { get; set; }

    public DbInitializerFixture()
    {
        DataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        SampleData = new SampleData(DataAccess);
        CreateSampleData().Wait();
    }

    private async Task CreateSampleData()
    {
        await SampleData.Seed();
    }
}
