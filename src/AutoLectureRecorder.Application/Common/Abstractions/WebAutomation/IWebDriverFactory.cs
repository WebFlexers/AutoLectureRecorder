using AutoLectureRecorder.Application.Options;

namespace AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;

using ErrorOr;

public interface IWebDriverFactory
{
    ErrorOr<IAlrWebDriver> CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, 
        string debuggerAddress = $"localhost:{WebView.BrowserArguments.DebugPort}");
}