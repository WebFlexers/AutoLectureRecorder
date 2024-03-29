﻿using System;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.Common.Core.Abstractions;

public interface IWindowFactory
{
    /// <summary>
    /// Creates a transparent loading window that covers fully
    /// the given window and sits exactly on top of it
    /// </summary>
    Window CreateTransparentOverlayWindow(Window? backgroundWindowOfOverlay, 
        bool isWindowFullscreen, Func<Task>? cancelDelegate);

    /// <summary>
    /// Creates a Record Window that joins the meeting 
    /// and starts recording the given scheduled lecture
    /// </summary>
    Window CreateRecordWindow();
}