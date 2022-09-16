﻿using AutoLectureRecorder.Services.Factories;
using AutoLectureRecorder.Services.ScreenRecorder;
using AutoLectureRecorder.Services.ScreenRecorder.Interfaces;
using Xunit.Abstractions;

namespace AutoLectureRecorder.Tests.ALR.Services;

public class RecorderTests
{
    private readonly ITestOutputHelper _output;

    public RecorderTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void RecordVideoWithDefaultConfig_ShouldCreateMp4Video()
    {
        IRecorder recorder = RecorderFactory.CreateRecorder();

        recorder.Options.OutputDirectory = "RecorderTests";
        recorder.Options.OutputFileName = "TestDefault";
        recorder.Options.OutputFileExtension = VideoExtensions.mp4;

        recorder.StartRecording();
        Thread.Sleep(3000);
        recorder.StopRecording();
        Thread.Sleep(1000);
    }

    [Fact]
    public void RecordVideoWithCustomAudio_ShouldCreateMp4Video()
    {
        IRecorder recorder = RecorderFactory.CreateRecorder();

        recorder.Options.OutputDirectory = "RecorderTests";
        recorder.Options.OutputFileName = "TestCustomAudio";
        recorder.Options.OutputFileExtension = VideoExtensions.mp4;

        var outputDevices = recorder.GetAudioOutputDevices;
        _output.WriteLine("Devices:");
        foreach (var device in outputDevices)
        {
            _output.WriteLine(device);
        }

        recorder.SelectedAudioOutputDevice = outputDevices[1];
        _output.WriteLine($"\nSelected output device: {recorder.SelectedAudioOutputDevice}");

        recorder.StartRecording();
        Thread.Sleep(3000);
        recorder.StopRecording();
        Thread.Sleep(1000);
    }
}