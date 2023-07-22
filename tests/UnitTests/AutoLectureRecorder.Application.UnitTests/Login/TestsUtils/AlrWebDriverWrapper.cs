using System.Reactive;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;

using ErrorOr;

namespace AutoLectureRecorder.Application.UnitTests.Login.TestsUtils;

/// <summary>
/// This wrapper is used for the following reason: <br/><br/> The IAlrWebDriver methods return ErrorOr for the most part.
/// An instance of ErrorOr can only be created using implicit operators (aka by returning directly either an Error or
/// the actual type that ErrorOr is defined with. However when the type that is defined in
/// ErrorOr is an interface it cannot be returned directly, because in C# it is forbidden to return interface instances
/// through implicit operators. So we have to create a concrete implementation that is a wrapper
/// of the IAlrWebDriver interface)
/// </summary>
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
        _inner.Dispose();
    }
}