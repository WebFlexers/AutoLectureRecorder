namespace AutoLectureRecorder.Services.WebDriver.SeleniumUtility;

public interface IWebDriverDownloader
{
    Task<string> Download(IProgress<float> progress);
}
