using AutoLectureRecorder.Application.Recording.Common;
using AutoLectureRecorder.Domain.ReactiveModels;
using ReactiveUI;
using ScreenRecorderLib;

namespace AutoLectureRecorder.Application.Common.Abstractions.Recording;

public interface IRecorder : IDisposable, IReactiveObject
{
    bool IsRecording { get; set; }
    string? RecordingDirectoryPath { get; set; }
    string? RecordingFileName { get; set; }
    string? RecordingFilePath { get; }
    RecorderOptions? Options { get; set; }
    void ApplyRecordingSettings(ReactiveRecordingSettings settings);
    ReactiveRecordingSettings GetDefaultSettings(int primaryScreenWidth, int primaryScreenHeight);
    IEnumerable<ReactiveAudioDevice> GetInputAudioDevices();
    IEnumerable<ReactiveAudioDevice> GetOutputAudioDevices();

    /// <summary>
    /// Starts a new recording. If no windowHandle is specified the main screen is recorded.
    /// Otherwise only the specified window is recorded. If a recording with the exact same
    /// file path already exists it is deleted by default. This behaviour can be modified
    /// using the autoDeleteIdenticalFile argument.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to record</param>
    /// <param name="autoDeleteIdenticalFile">Whether or not to automatically delete an existing file with
    /// the exact same name as the specified Recording File Path</param>
    /// <exception cref="ArgumentException">Thrown when RecordingDirectoryPath or RecordingFileName are not specified</exception>
    IRecorder StartRecording(IntPtr? windowHandle = null, bool autoDeleteIdenticalFile = true);

    /// <summary>
    /// Stops the recording and starts the file saving process.
    /// IMPORTANT: When this method returns the file saving process is not
    /// done yet. If you want to execute code after it has finished use the
    /// OnRecordingComplete and/or OnRecordingFailed methods
    /// </summary>
    IRecorder InitiateStopRecording();

    /// <summary>
    /// Executes the specified action when the recording
    /// finishes up and the file is saved successfully
    /// </summary>
    IRecorder OnRecordingComplete(Action callbackDelegate);

    IRecorder OnRecordingComplete(Func<Task> callbackDelegate);

    /// <summary>
    /// Executes the specified action when the recording
    /// finishes up and the file is saved successfully
    /// </summary>
    IRecorder OnRecordingFailed(Action callbackDelegate);

    IRecorder OnRecordingFailed(Func<Task> callbackDelegate);
}