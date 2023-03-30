using System;
using System.Threading.Tasks;
using System.Windows;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Sections.LoginWebView;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public class WindowFactory : IWindowFactory
{
    /// <summary>
    /// Creates a transparent loading window that covers fully
    /// the given window and sits exactly on top of it with Topmost true
    /// </summary>
    public Window CreateTransparentOverlayWindow(Window? backgroundWindowOfOverlay, Func<Task>? cancelDelegate)
    {
        if (backgroundWindowOfOverlay == null)
        {
            throw new ArgumentNullException(nameof(backgroundWindowOfOverlay), 
                "The background window you provided was null");
        }

        var overlayWindow = new TransparentLoadingWindow(cancelDelegate)
        {
            Top = backgroundWindowOfOverlay.Top,
            Left = backgroundWindowOfOverlay.Left,
            Width = backgroundWindowOfOverlay.ActualWidth,
            Height = backgroundWindowOfOverlay.ActualHeight,
            Topmost = true
        };

        return overlayWindow;
    }
}
