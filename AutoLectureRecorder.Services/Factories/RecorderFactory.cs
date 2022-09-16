using AutoLectureRecorder.Services.ScreenRecorder.Interfaces;
using AutoLectureRecorder.Services.ScreenRecorder.Windows;
using System.Runtime.InteropServices;

namespace AutoLectureRecorder.Services.Factories;

public class RecorderFactory
{
    /// <summary>
    /// Creates a recorder according to the OS that the application runs in
    /// </summary>
    /// <returns> IRecorder implementation </returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IRecorder CreateRecorder()
    {
        // var value when value == something is used
        // to include OSPlatform readonly struct, because it's resolved during runtime
        // and switch only accepts constant values
        switch (true)
        {
            case var value when value == RuntimeInformation.IsOSPlatform(OSPlatform.Windows):
                var options = new WindowsRecorderOptions();
                return new WindowsRecorder(options);
            case var value when value == RuntimeInformation.IsOSPlatform(OSPlatform.Linux):
                throw new NotImplementedException();
            case var value when value == RuntimeInformation.IsOSPlatform(OSPlatform.OSX):
                throw new NotImplementedException();
            default:
                throw new NotImplementedException("Only windows, linux and macOS are supported by the recorder");
        }
    }
}