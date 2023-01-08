﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveStudentAccount : ReactiveObject
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public string RegistrationNumber { get; set; }
    [Reactive]
    public string EmailAddress { get; set; }
    [Reactive]
    public string Password { get; set; }
}
