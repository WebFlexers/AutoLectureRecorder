namespace AutoLectureRecorder.Services.InternetConnectivity;

public interface IWebConnection
{
    Task<bool> IsConnectedToTheInternet(TimeSpan? timeoutMs = null, List<string> urls = null);
}