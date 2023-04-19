using Microsoft.Extensions.Logging;
//using ScreenRecorderLib;

namespace AutoLectureRecorder.Services.Recording;

public class WindowsRecorder //: IRecorder, IDisposable
{
    //private readonly ILogger<WindowsRecorder> _logger;

    //private Recorder? _recorder;
    //private Action? _onRecordingComplete;
    //private Func<Task>? _onRecordingCompleteAsync;
    //private Action? _onRecordingFailed;
    //private Func<Task>? _onRecordingFailedAsync;

    //public bool IsRecording { get; set; } = false;

    //public string? RecordingDirectoryPath { get; set; }
    //public string? RecordingFileName { get; set; }
    //public string? RecordingFilePath => string.IsNullOrWhiteSpace(RecordingDirectoryPath) ||
    //                                    string.IsNullOrWhiteSpace(RecordingFileName) 
    //    ? null 
    //    : $"{Path.Combine(RecordingDirectoryPath, RecordingFileName)}.mp4";

    //// These options must be set before starting the recording, and cannot be modified while recording.
    //public RecorderOptions Options { get; set; } = new()
    //{
    //    OutputOptions = new OutputOptions
    //    {
    //        RecorderMode = RecorderMode.Video,
    //        OutputFrameSize = new ScreenSize(1280, 720),
    //        Stretch = StretchMode.UniformToFill,
    //    },
    //    AudioOptions = new AudioOptions
    //    {
    //        Bitrate = AudioBitrate.bitrate_128kbps,
    //        Channels = AudioChannels.Stereo,
    //        IsAudioEnabled = true,
    //    },
    //    MouseOptions = new MouseOptions
    //    {
    //        IsMousePointerEnabled = false
    //    }
    //};

    //public WindowsRecorder(ILogger<WindowsRecorder> logger)
    //{
    //    _logger = logger;
    //}

    ///// <summary>
    ///// Starts a new recording. If no windowHandle is specified the main screen is recorded.
    ///// Otherwise only the specified window is recorded.
    ///// </summary>
    ///// <param name="windowHandle">The handle of the window to record</param>
    ///// <exception cref="ArgumentException">Thrown when RecordingDirectoryPath or RecordingFileName are not specified</exception>
    //public IRecorder StartRecording(IntPtr? windowHandle = null)
    //{
    //    if (RecordingFilePath == null)
    //    {
    //        throw new ArgumentException("RecordingDirectoryPath and RecordingFileName must not be null or empty");
    //    }

    //    if (File.Exists(RecordingFilePath))
    //    {
    //        File.Delete(RecordingFilePath);
    //    }

    //    if (windowHandle != null)
    //    {
    //        var sources = new List<RecordingSourceBase>
    //        {
    //            new WindowRecordingSource(windowHandle.Value)
    //        };

    //        Options.SourceOptions = new SourceOptions
    //        {
    //            RecordingSources = sources
    //        };
    //    }

    //    string videoPath = RecordingFilePath;

    //    _recorder = Recorder.CreateRecorder(Options);
    //    _recorder.OnRecordingComplete += RecorderOnRecordingComplete;
    //    _recorder.OnRecordingFailed += RecorderOnRecordingFailed;

    //    _recorder.Record(videoPath);

    //    IsRecording = true;
    //    _logger.LogInformation("Started Recording...");

    //    return this;
    //}

    ///// <summary>
    ///// Stops the recording and starts the file saving process.
    ///// IMPORTANT: When this method returns the file saving process is not
    ///// done yet. If you want to execute code after it has finished use the
    ///// OnRecordingComplete and/or OnRecordingFailed methods
    ///// </summary>
    //public IRecorder StopRecording()
    //{
    //    _recorder?.Stop();
    //    _logger.LogInformation("Recording stop initiated");
    //    return this;
    //}

    ///// <summary>
    ///// Executes the specified action when the recording
    ///// finishes up and the file is saved successfully
    ///// </summary>
    //public IRecorder OnRecordingComplete(Action callbackDelegate)
    //{
    //    _onRecordingComplete = callbackDelegate;
    //    return this;
    //}

    //public IRecorder OnRecordingComplete(Func<Task> callbackDelegate)
    //{
    //    _onRecordingCompleteAsync = callbackDelegate;
    //    return this;
    //}

    ///// <summary>
    ///// Executes the specified action when the recording
    ///// finishes up and the file is saved successfully
    ///// </summary>
    //public IRecorder OnRecordingFailed(Action callbackDelegate)
    //{
    //    _onRecordingFailed = callbackDelegate;
    //    return this;
    //}

    //public IRecorder OnRecordingFailed(Func<Task> callbackDelegate)
    //{
    //    _onRecordingFailedAsync = callbackDelegate;
    //    return this;
    //}

    //private void RecorderOnRecordingComplete(object? sender, RecordingCompleteEventArgs e)
    //{
    //    IsRecording = false;
    //    _logger.LogInformation("Recording completed successfully!");
    //    _onRecordingComplete?.Invoke();
    //    _onRecordingCompleteAsync?.Invoke();
    //    _recorder?.Dispose();
    //}

    //private void RecorderOnRecordingFailed(object? sender, RecordingFailedEventArgs e)
    //{
    //    IsRecording = false;
    //    _logger.LogError("Recording failed to complete with error: {error}", e.Error);
    //    _onRecordingFailed?.Invoke();
    //    _onRecordingFailedAsync?.Invoke();
    //    _recorder?.Dispose();
    //}

    //public void Dispose()
    //{
    //    _recorder?.Dispose();
    //}
}