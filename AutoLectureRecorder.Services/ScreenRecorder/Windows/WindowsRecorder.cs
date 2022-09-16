namespace AutoLectureRecorder.Services.ScreenRecorder.Windows;

using AutoLectureRecorder.Services.ScreenRecorder.Interfaces;
using ScreenRecorderLib;
using System.Collections.Generic;
using System.Diagnostics;
public class WindowsRecorder : IRecorder
{
    // Get Audio Devices
    public List<string> GetAudioInputDevices => GetAudioDevicesFriendlyNameBySource(AudioDeviceSource.InputDevices);
    public List<string> GetAudioOutputDevices => GetAudioDevicesFriendlyNameBySource(AudioDeviceSource.OutputDevices);

    private List<string> GetAudioDevicesFriendlyNameBySource(AudioDeviceSource audioDeviceSource)
    {
        var output = new List<string>();

        var audioDevices = Recorder.GetSystemAudioDevices(audioDeviceSource);
        foreach (var audioDevice in audioDevices)
        {
            output.Add(audioDevice.FriendlyName);
        }

        return output;
    }

    // Select Audio Devices
    public string SelectedAudioInputDevice
    {
        get
        {
            return WindowsRecorderOptions.AudioOptions.AudioInputDevice;
        }
        set 
        {
            var audioDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
            var selectedDevice = audioDevices.Find(x => x.FriendlyName == value);
            WindowsRecorderOptions.AudioOptions.AudioInputDevice = selectedDevice?.DeviceName;
        }
    }
    public string SelectedAudioOutputDevice 
    {
        get
        {
            return WindowsRecorderOptions.AudioOptions.AudioOutputDevice;
        }
        set
        {
            var audioDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);
            var selectedDevice = audioDevices.Find(x => x.FriendlyName == value);
            WindowsRecorderOptions.AudioOptions.AudioOutputDevice = selectedDevice?.DeviceName;
        }
    }

    // Select specific window for recording
    public bool IsSpecificWindowSelected { get; set; }

    // Options
    public IRecorderOptions Options { get; }
    private WindowsRecorderOptions WindowsRecorderOptions { get => (WindowsRecorderOptions)Options; }

    public WindowsRecorder(WindowsRecorderOptions options)
    {
        Options = options;
    }

    Recorder _rec;
    public void StartRecording()
    {
        string videoPath = Options.OutputFilePath;
        if (File.Exists(videoPath))
        {
            File.Delete(videoPath);
            Debug.WriteLine("A file with the same name already exists. It has been replaced");
        }

        if (WindowsRecorderOptions.OutputOptions.RecorderMode != RecorderMode.Video)
        {
            WindowsRecorderOptions.OutputOptions.RecorderMode = RecorderMode.Video;
        }

        _rec = Recorder.CreateRecorder(WindowsRecorderOptions);
        _rec.OnRecordingComplete += Rec_OnRecordingComplete;
        _rec.OnRecordingFailed += Rec_OnRecordingFailed;
        _rec.OnStatusChanged += Rec_OnStatusChanged;

        // Record to a file
        _rec.Record(videoPath);
    }

    public void StopRecording()
    {
        _rec.Stop();
    }

    public void TakeScreenshot()
    {
        if (WindowsRecorderOptions.OutputOptions.RecorderMode != RecorderMode.Screenshot)
        {
            WindowsRecorderOptions.OutputOptions.RecorderMode = RecorderMode.Screenshot;
        }

        string screenshotPath = Options.OutputFilePath;
        if (File.Exists(screenshotPath))
        {
            File.Delete(screenshotPath);
            Debug.WriteLine("A file with the same name already exists. It has been replaced");
        }

        _rec = Recorder.CreateRecorder(WindowsRecorderOptions);

        _rec.Record(screenshotPath);

        _rec.Stop();
    }

    private void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
    {
        Debug.WriteLine($"Succesfully wrote file: {Options.OutputFilePath}");
    }
    private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
    {
        string error = e.Error;
        Debug.WriteLine(error);
    }
    private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
    {
        RecorderStatus status = e.Status;
        Debug.WriteLine(status.ToString());
    }
}
