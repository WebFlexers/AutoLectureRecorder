using AutoLectureRecorder.Services.WebDriver;
using System;
using System.Collections.Generic;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public class WebDriverFactory : IWebDriverFactory
{
    private readonly IEnumerable<IWebDriver> _webDrivers;

    public WebDriverFactory(IEnumerable<IWebDriver> webDrivers)
    {
        _webDrivers = webDrivers;
    }

    public IWebDriver CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222")
    {
        UnipiEdgeWebDriver? edgeWebDriver = null;
        foreach (var webDriver in _webDrivers)
        {
            if (webDriver is UnipiEdgeWebDriver)
            {
                edgeWebDriver = (UnipiEdgeWebDriver)webDriver;
                break;
            }
        }

        if (edgeWebDriver == null)
        {
            throw new NullReferenceException("No UnipiEdgeWebDriver was found");
        }

        edgeWebDriver.StartDriver(useWebView, implicitWaitTime, debuggerAddress);

        return edgeWebDriver;
    }
}
