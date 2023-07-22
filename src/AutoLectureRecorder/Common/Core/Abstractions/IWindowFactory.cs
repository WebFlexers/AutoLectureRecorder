using System;
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
}