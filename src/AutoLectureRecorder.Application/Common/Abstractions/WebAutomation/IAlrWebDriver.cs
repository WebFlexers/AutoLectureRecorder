using System.Reactive;
using AutoLectureRecorder.Application.Options;
using ErrorOr;

namespace AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;

public interface IAlrWebDriver
{
    /// <summary>
    /// Creates a web driver backed by the Edge browser.
    /// </summary>
    /// <param name="useWebView">Whether to use the integrated microsoft EdgeWebView2 or not</param>
    /// <param name="implicitWaitTime">The time to wait when searching for elements before throwing exception</param>
    /// <param name="debuggerAddress">The debugger address that will host the WebView2</param>
    ErrorOr<Unit> StartDriver(bool useWebView, TimeSpan implicitWaitTime, 
        string debuggerAddress = $"localhost:{WebView.BrowserArguments.DebugPort}");

    /// <summary>
    /// Login to Microsoft Teams through the web driver using the provided credentials
    /// </summary>
    ErrorOr<Unit> Login(string academicEmailAddress, string password, bool resolveInitialUrlAutomatically = true,
        CancellationToken? cancellationToken = null);

    /// <summary>
    /// Join the specified microsoft teams meeting after logging in if the user is not already logged in
    /// </summary>
    ErrorOr<Unit> JoinMeeting(string academicEmailAddress, string password,
        string meetingLink, TimeSpan meetingDuration, CancellationToken? cancellationToken = null);

    void Dispose();
}