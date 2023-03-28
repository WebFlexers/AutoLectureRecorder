namespace AutoLectureRecorder.Services.WebDriver;

public interface IWebDriverDownloader
{
    Task<bool> Download(IProgress<float>? progress = null);
}
