using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.Services.WebDriver.Interfaces;
using System;
using System.Collections.Generic;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public class WebDriverFactory : IWebDriverFactory
{
    private readonly IEnumerable<IAlrWebDriver> _webDrivers;

    public WebDriverFactory(IEnumerable<IAlrWebDriver> webDrivers)
    {
        _webDrivers = webDrivers;
    }

    public IAlrWebDriver CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222")
    {
        UnipiEdgeWebDriver? edgeWebDriver = null;
        foreach (var webDriver in _webDrivers)
        {
            if (webDriver is not UnipiEdgeWebDriver driver) continue;

            edgeWebDriver = driver;
            break;
        }

        if (edgeWebDriver == null)
        {
            throw new NullReferenceException("No UnipiEdgeWebDriver was found");
        }

        edgeWebDriver.StartDriver(useWebView, implicitWaitTime, debuggerAddress);

        return edgeWebDriver;
    }
}
