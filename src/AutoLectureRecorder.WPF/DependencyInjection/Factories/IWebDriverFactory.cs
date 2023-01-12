using AutoLectureRecorder.Services.WebDriver;
using System;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public interface IWebDriverFactory
{
    IWebDriver CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222");
}