﻿using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveStatisticSettings
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public bool AreRecordedLecturesTracked { get; set; }
    [Reactive]
    public bool AreSuccessfulRecordingsTracked { get; set; }
    [Reactive]
    public bool AreFailedRecordingsTracked { get; set; }
}