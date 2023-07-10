using System.Reactive;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;

using ErrorOr;

namespace AutoLectureRecorder.Application.UnitTests.Login.TestsUtils;

// Assuming this wrapping class
public class AlrWebDriverWrapper : IAlrWebDriver
{
    private readonly IAlrWebDriver _inner;
    public AlrWebDriverWrapper(IAlrWebDriver inner)
    {
        _inner = inner;
    }

    public ErrorOr<Unit> Login(string academicEmailAddress, string password, bool resolveInitialUrlAutomatically = true, CancellationToken? cancellationToken = null)
    {
        return _inner.Login(academicEmailAddress, password, resolveInitialUrlAutomatically, cancellationToken);
    }

    
    public ErrorOr<Unit> StartDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222")
    {
        return _inner.StartDriver(useWebView, implicitWaitTime, debuggerAddress);
    }
    
    public ErrorOr<Unit> JoinMeeting(string academicEmailAddress, string password, string meetingLink, TimeSpan meetingDuration,
        CancellationToken? cancellationToken = null)
    {
        return _inner.JoinMeeting(academicEmailAddress, password, meetingLink, meetingDuration, cancellationToken);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}