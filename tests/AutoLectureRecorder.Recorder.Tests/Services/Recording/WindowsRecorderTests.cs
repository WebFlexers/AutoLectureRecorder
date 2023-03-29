using System.Runtime.InteropServices;
using AutoLectureRecorder.Services.Recording;
using Xunit.Abstractions;

namespace AutoLectureRecorder.Recorder.Tests.Services.Recording;

public class WindowsRecorderTests
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    private readonly ITestOutputHelper _output;

    public WindowsRecorderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task StartRecording_ShouldRecordScreenFor3Seconds()
    {
        await TestSuccessfulRecording("RecordScreenTest");
    }

    [Fact]
    public async Task StartRecording_ShouldRecordWindowFor3Seconds()
    {
        IntPtr thisWindowHandle = GetForegroundWindow();
        await TestSuccessfulRecording("RecordWindowTest", thisWindowHandle);
    }

    private async Task TestSuccessfulRecording(string videoFileName, IntPtr? windowHandle = null)
    {
        var logger = XUnitLogger.CreateLogger<WindowsRecorder>(_output);
        var recorder = new WindowsRecorder(logger)
        {
            RecordingDirectoryPath = Directory.GetCurrentDirectory(),
            RecordingFileName = videoFileName
        };

        recorder.StartRecording(windowHandle);

        await Task.Delay(3000);

        bool finishedSuccessfully = false;
        bool recordingFailed = false;

        recorder.StopRecording()
            .OnRecordingComplete(() =>
            {
                finishedSuccessfully = true;
            })
            .OnRecordingFailed(() =>
            {
                recordingFailed = true;
            });

        while (recorder.IsRecordingDone == false)
        {
            await Task.Delay(100);
        }

        var currentDirectory = Directory.GetCurrentDirectory();
        Assert.True(File.Exists(Path.Combine(currentDirectory, $"{videoFileName}.mp4")));
        Assert.True(finishedSuccessfully);
        Assert.True(recorder.IsRecordingDone);
        Assert.False(recordingFailed);
    }
}