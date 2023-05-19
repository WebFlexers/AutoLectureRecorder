using AutoLectureRecorder.Services.WebDriver.Interfaces;
using System;

namespace AutoLectureRecorder.DependencyInjection.Factories.Interfaces;

public interface IWebDriverFactory
{
    IAlrWebDriver? CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222");
}