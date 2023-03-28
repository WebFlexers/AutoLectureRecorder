using System;
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
    public async Task Download_ShouldSuccessfullyDownloadDriver()
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

    [Fact]
    public async Task Download_ShouldNotDownloadTwice()
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
        
        // Download normally the first time
        IWebDriverDownloader driverDownloader = new EdgeWebDriverDownloader(logger, new HttpClientFactoryMock());
        IProgress<float> progress1 = new Progress<float>();
        await driverDownloader.Download(progress1);

        // This must remain 0
        int numberOfDownloadProgressReports = 0;
        IProgress<float> progress2 = new Progress<float>(progress =>
        {
            numberOfDownloadProgressReports++;
        });

        
        bool isSuccessful = await driverDownloader.Download(progress2);

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
        Assert.Equal(0, numberOfDownloadProgressReports);
    }
}
