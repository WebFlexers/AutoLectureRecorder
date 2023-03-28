using System.Text;
using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.WebDriver.UnitTests.WebDriver.Mocks;
using Xunit.Abstractions;

namespace AutoLectureRecorder.WebDriver.UnitTests.WebDriver;

public class EdgeWebDriverDownloaderTests
{
    private readonly ITestOutputHelper _output;

    public EdgeWebDriverDownloaderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task DownloadTest()
    {
        var logger = XUnitLogger.CreateLogger<EdgeWebDriverDownloader>(_output);

        // Delete the directory to simulate a new installation
        var expectedFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "AutoLectureRecorder");

        if (Directory.Exists(expectedFilePath))
        {
            Directory.Delete(expectedFilePath, true);
        }
        
        IWebDriverDownloader driverDownloader = new EdgeWebDriverDownloader(logger, new HttpClientFactoryMock());
        IProgress<float> progress = new Progress<float>(progress =>
        {
            _output.WriteLine($"{Math.Round(progress*100)}%");
        });

        bool isSuccessful = await driverDownloader.Download(progress);

        bool driverFileExists = false;

        foreach (var fileName in Directory.GetFiles(expectedFilePath))
        {
            if (fileName.Contains("msedgedriver"))
            {
                driverFileExists = true;
            }
        }

        Assert.True(isSuccessful);
        Assert.True(driverFileExists);
    }
}
