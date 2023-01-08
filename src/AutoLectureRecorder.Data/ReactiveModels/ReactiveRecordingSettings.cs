﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveRecordingSettings : ReactiveObject
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public string RecordingsLocalPath { get; set; }
    [Reactive]
    public string OutputDevice { get; set; }
    [Reactive]
    public string InputDevice { get; set; }
    [Reactive]
    public bool IsInputDeviceEnabled { get; set; }
    [Reactive]
    public int Quality { get; set; }
    [Reactive]
    public double Fps { get; set; }
}
