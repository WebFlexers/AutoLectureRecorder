using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using AutoLectureRecorder.Common.Core.Abstractions;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.Pages.RecordLecture;
using Microsoft.Extensions.DependencyInjection;

namespace AutoLectureRecorder.Common.Core;

public class WindowFactory : IWindowFactory
{
    private readonly IServiceProvider _services;

    public WindowFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <inheritdoc/>
    public Window CreateTransparentOverlayWindow(Window? backgroundWindowOfOverlay, 
        bool isWindowFullscreen, Func<Task>? cancelDelegate)
    {
        if (backgroundWindowOfOverlay == null)
        {
            throw new ArgumentNullException(nameof(backgroundWindowOfOverlay), 
                "The background window you provided was null");
        }

        var overlayWindow = new TransparentLoadingWindow(cancelDelegate)
        {
            Width = backgroundWindowOfOverlay.ActualWidth,
            Height = backgroundWindowOfOverlay.ActualHeight,
        };

        if (isWindowFullscreen)
        {
            var hostScreen = System.Windows.Forms.Screen.FromHandle(
                new WindowInteropHelper(backgroundWindowOfOverlay).Handle);
            overlayWindow.Left = hostScreen.WorkingArea.Left;
            overlayWindow.Top = hostScreen.WorkingArea.Top;
        }
        else
        {
            overlayWindow.Top = backgroundWindowOfOverlay.Top;
            overlayWindow.Left = backgroundWindowOfOverlay.Left;
        }

        return overlayWindow;
    }
    
    /// <inheritdoc/>
    public Window CreateRecordWindow()
    {
        var recordWindow = _services.GetRequiredService<RecordWindow>();
        recordWindow.ViewModel!.RecordWindowState = WindowState.Maximized;
        
        return recordWindow;
    }
}