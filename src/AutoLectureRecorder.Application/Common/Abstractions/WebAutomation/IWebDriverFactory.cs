using AutoLectureRecorder.Application.Common.Options;

namespace AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;

using ErrorOr;

public interface IWebDriverFactory
{
    ErrorOr<IAlrWebDriver> CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, 
        string debuggerAddress = $"localhost:{WebViewOptions.BrowserArguments.DebugPort}");
}