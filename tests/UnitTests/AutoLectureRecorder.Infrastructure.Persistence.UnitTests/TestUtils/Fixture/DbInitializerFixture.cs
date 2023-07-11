using AutoLectureRecorder.Infrastructure.Persistence.Seeding;

namespace AutoLectureRecorder.Infrastructure.Persistence.UnitTests.TestUtils.Fixture;

public class DbInitializerFixture
{
    public SqliteDataAccess DataAccess { get; set; }
    public SampleData SampleData { get; set; }

    public DbInitializerFixture()
    {
        DataAccess = new SqliteDataAccess("Data Source=.\\Persistence\\AutoLectureRecorderDB.db;");
        SampleData = new SampleData(DataAccess, true);
        CreateSampleData().Wait();
    }

    private async Task CreateSampleData()
    {
        await SampleData.Seed();
    }
}
