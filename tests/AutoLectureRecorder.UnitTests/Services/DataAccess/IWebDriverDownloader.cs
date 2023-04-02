namespace AutoLectureRecorder.Services.WebDriver.Interfaces;

public interface IWebDriverDownloader
{
    Task<bool> Download(IProgress<float>? progress = null);
}
