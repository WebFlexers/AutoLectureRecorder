using ScreenRecorderLib;

namespace AutoLectureRecorder.Services.Recording;

public interface IRecorder
{
    bool IsRecordingDone { get; set; }
    string? RecordingFilePath { get; }
    RecorderOptions Options { get; set; }

    /// <summary>
    /// Starts a new recording. If no windowHandle is specified the main screen is recorded.
    /// Otherwise only the specified window is recorded.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to record</param>
    /// <exception cref="ArgumentException">Thrown when RecordingDirectoryPath or RecordingFileName are not specified</exception>
    IRecorder StartRecording(IntPtr? windowHandle = null);

    /// <summary>
    /// Stops the recording and starts the file saving process.
    /// IMPORTANT: When this method returns the file saving process is not
    /// done yet. If you want to execute code after is has finished use the
    /// OnRecordingComplete and/or OnRecordingFailed methods
    /// </summary>
    IRecorder StopRecording();

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