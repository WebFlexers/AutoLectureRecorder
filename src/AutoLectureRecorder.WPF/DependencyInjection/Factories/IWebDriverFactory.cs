using AutoLectureRecorder.Services.WebDriver;
using System;

namespace AutoLectureRecorder.WPF.DependencyInjection.Factories;

public interface IWebDriverFactory
{
    IWebDriver CreateUnipiEdgeWebDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222");
}