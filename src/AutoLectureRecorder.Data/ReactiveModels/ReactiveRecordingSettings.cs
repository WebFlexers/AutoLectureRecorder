using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveRecordingSettings : ReactiveObject
{
    [Reactive]
    public string RecordingsLocalPath { get; set; }
    [Reactive]
    public string OutputDeviceName { get; set; }
    [Reactive]
    public string OutputDeviceFriendlyName { get; set; }
    [Reactive]
    public string InputDeviceName { get; set; }
    [Reactive]
    public string InputDeviceFriendlyName { get; set; }
    [Reactive]
    public bool IsInputDeviceEnabled { get; set; }
    [Reactive]
    public int Quality { get; set; }
    [Reactive]
    public int Fps { get; set; }
    [Reactive]
    public int OutputFrameWidth { get; set; }
    [Reactive]
    public int OutputFrameHeight { get; set; }
}
