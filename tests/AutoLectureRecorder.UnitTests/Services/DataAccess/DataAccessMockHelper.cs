using Microsoft.Extensions.Configuration;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

public static class DataAccessMockHelper
{
    public static IConfiguration CreateConfiguration()
    {
        Dictionary<string, string?>? inMemorySettings = new()
        {
            { "Main", "Dictionary Main Value" },
            { "ConnectionStrings:Default", "Data Source=.\\AutoLectureRecorderDB.db;" },
            { "DefaultGeneralSettings:LaunchAtStartup", "true" },
            { "DefaultGeneralSettings:OnCloseKeepAlive", "true" },
            { "DefaultGeneralSettings:ShowSplashScreen", "true" },
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
}
