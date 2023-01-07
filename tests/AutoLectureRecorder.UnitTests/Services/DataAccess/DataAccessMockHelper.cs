using Microsoft.Extensions.Configuration;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

public static class DataAccessMockHelper
{
    public static IConfiguration CreateConfiguration()
    {
        Dictionary<string, string?>? inMemorySettings = new Dictionary<string, string?>
        {
            {"Main", "Dictionary Main Value"},
            {"ConnectionStrings:Default", "Data Source=.\\AutoLectureRecorderDB.db;"},
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
}
