using AutoLectureRecorder.Services.WebDriver.SeleniumUtility;
using AutoLectureRecorder.Services.WebDriver.SeleniumUtility.Windows;
using System.Diagnostics;

namespace AutoLectureRecorder.Tests.ALR.Services.WebDriver.SeleniumUtility.Windows;

public class EdgeWebDriverDownloaderTests
{
    [Fact]
    public void DownloadTest()
    {
        IWebDriverDownloader driverDownloader = new EdgeWebDriverDownloader();
        IProgress<float> progress = new Progress<float>(progress =>
        {
            Debug.WriteLine($"{Math.Round(progress*100)}%");
        });
        driverDownloader.Download(progress).Wait();

        bool driverFileExists = false;

        foreach (var fileName in Directory.GetFiles(Directory.GetCurrentDirectory()))
        {
            if (fileName.Contains("msedgedriver"))
            {
                driverFileExists = true;
            }
        }

        Assert.True(driverFileExists);
    }
}