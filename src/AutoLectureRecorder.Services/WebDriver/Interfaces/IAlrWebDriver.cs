namespace AutoLectureRecorder.Services.WebDriver.Interfaces;

public interface IAlrWebDriver : IDisposable
{
    /// <summary>
    /// Creates a web driver backed by the Edge browser.
    /// </summary>
    /// <param name="useWebView">Whether to use the integrated microsoft EdgeWebView2 or not</param>
    /// <param name="implicitWaitTime">The time to wait when searching for elements before throwing exception</param>
    /// <param name="debuggerAddress">The debugger address that will host the WebView2</param>
    void StartDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222");

    /// <summary>
    /// Login to Microsoft Teams through the web driver using the provided credentials
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown when the web driver is null</exception>
    (bool result, string resultMessage) Login(string academicEmailAddress, string password,
        CancellationToken? cancellationToken = null,
        bool resolveInitialUrlAutomatically = true);

    /// <summary>
    /// Join the specified microsoft teams meeting after logging in if the user is not already logged in
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown when the web driver is null</exception>
    (bool result, string resultMessage) JoinMeeting(string academicEmailAddress, string password,
        string meetingLink, TimeSpan meetingDuration, CancellationToken? cancellationToken = null);
}