﻿namespace AutoLectureRecorder.Services.WebDriver;

public interface IWebDriver : IDisposable
{
    (bool result, string resultMessage) LoginToMicrosoftTeams(string academicEmailAddress, string password);
    void StartDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222");
}