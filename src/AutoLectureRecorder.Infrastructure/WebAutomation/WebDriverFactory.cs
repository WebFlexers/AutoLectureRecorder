using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using ErrorOr;

namespace AutoLectureRecorder.Infrastructure.WebAutomation;

public class WebDriverFactory : IWebDriverFactory
{
    private readonly IAlrWebDriver _alrWebDriver;

    public WebDriverFactory(IAlrWebDriver alrWebDriver)
    {
        _alrWebDriver = alrWebDriver;
    }
    
    public ErrorOr<IAlrWebDriver> CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, 
        string debuggerAddress)
    {
        var errorOrDriverStarted = _alrWebDriver.StartDriver(useWebView, implicitWaitTime, debuggerAddress);
        if (errorOrDriverStarted.IsError)
        {
            return errorOrDriverStarted.Errors;
        }
        
        return (UnipiEdgeWebDriver)_alrWebDriver;
    }
}