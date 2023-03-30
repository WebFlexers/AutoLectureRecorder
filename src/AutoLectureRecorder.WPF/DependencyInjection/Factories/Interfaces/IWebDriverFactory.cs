using AutoLectureRecorder.Services.WebDriver;
using System;

namespace AutoLectureRecorder.DependencyInjection.Factories.Interfaces;

public interface IWebDriverFactory
{
    IWebDriver CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222");
}