using AutoLectureRecorder.Data.ReactiveModels;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ScreenRecorderLib;
using System.Text.RegularExpressions;

namespace AutoLectureRecorder.Services.Recording;

public class WindowsRecorder : ReactiveObject, IRecorder, IDisposable
{
    private readonly ILogger<WindowsRecorder> _logger;

    private Recorder? _recorder;
    private Action? _onRecordingComplete;
    private Func<Task>? _onRecordingCompleteAsync;
    private Action? _onRecordingFailed;
    private Func<Task>? _onRecordingFailedAsync;

    private bool _isRecording = false;
    public bool IsRecording
    {
        get => _isRecording;
        set => this.RaiseAndSetIfChanged(ref _isRecording, value);
    }

    public string? RecordingDirectoryPath { get; set; }
    public string? RecordingFileName { get; set; }
    public string? RecordingFilePath => string.IsNullOrWhiteSpace(RecordingDirectoryPath) ||
                                        string.IsNullOrWhiteSpace(RecordingFileName)
        ? null
        : $"{Path.Combine(RecordingDirectoryPath, RecordingFileName)}.mp4";

    // These options must be set before starting the recording, and cannot be modified while recording.
    public RecorderOptions? Options { get; set; }

    public WindowsRecorder(ILogger<WindowsRecorder> logger)
    {
        _logger = logger;
    }

    public void ApplyRecordingSettings(ReactiveRecordingSettings settings)
    {
        RecordingDirectoryPath = settings.RecordingsLocalPath;

        List<AudioDevice> inputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
        List<AudioDevice> outputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);

        // If null or empty string is supplied to the recorder the default device will be used
        AudioDevice? selectedOutputDevice = outputDevices.FirstOrDefault(device => device.DeviceName == settings.OutputDeviceName);
        AudioDevice? selectedInputDevice = inputDevices.FirstOrDefault(device => device.DeviceName == settings.InputDeviceName);

        Options = new RecorderOptions
        {
            OutputOptions = new OutputOptions
            {
                RecorderMode = RecorderMode.Video,
                OutputFrameSize = new ScreenSize(settings.OutputFrameWidth, settings.OutputFrameHeight),
                Stretch = StretchMode.Uniform,
            },
            VideoEncoderOptions = new VideoEncoderOptions
            {
                Quality = settings.Quality,
                Framerate = settings.Fps
            },
            AudioOptions = new AudioOptions
            {
                Bitrate = AudioBitrate.bitrate_128kbps,
                Channels = AudioChannels.Stereo,
                IsAudioEnabled = true,
                IsInputDeviceEnabled = settings.IsInputDeviceEnabled,
                AudioOutputDevice = selectedOutputDevice?.DeviceName,
                AudioInputDevice = selectedInputDevice?.DeviceName
            },
            MouseOptions = new MouseOptions
            {
                IsMousePointerEnabled = false
            }
        };
    }

    public static ReactiveRecordingSettings GetDefaultSettings(int primaryScreenWidth, int primaryScreenHeight)
    {
        int outputFrameWidth;
        int outputFrameHeight;

        if (primaryScreenWidth > 1920 && primaryScreenHeight > 1080)
        {
            outputFrameWidth = 1920;
            outputFrameHeight = 1080;
        }
        else
        {
            outputFrameWidth = primaryScreenWidth;
            outputFrameHeight = primaryScreenHeight;
        }

        var settings = new ReactiveRecordingSettings
        {
            RecordingsLocalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AutoLectureRecorder"),
            OutputDeviceName = "Default",
            OutputDeviceFriendlyName = "Default",
            InputDeviceName = "Default",
            InputDeviceFriendlyName = "Default",
            IsInputDeviceEnabled = false,
            Quality = 70,
            Fps = 30,
            OutputFrameWidth = outputFrameWidth,
            OutputFrameHeight = outputFrameHeight,
        };

        return settings;
    }

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
    public IRecorder StartRecording(IntPtr? windowHandle = null, bool autoDeleteIdenticalFile = true)
    {
        if (RecordingFilePath == null)
        {
            throw new ArgumentException("RecordingDirectoryPath and RecordingFileName must not be null or empty");
        }

        TryToDeleteIdenticalFile(autoDeleteIdenticalFile);

        if (Options == null)
        {
            ApplyRecordingSettings(GetDefaultSettings(1280, 720));
        }

        if (windowHandle != null)
        {
            var sources = new List<RecordingSourceBase>
            {
                new WindowRecordingSource(windowHandle.Value)
            };

            Options!.SourceOptions ??= new SourceOptions();
            Options.SourceOptions.RecordingSources = sources;
        }

        string videoPath = RecordingFilePath;

        _recorder = Recorder.CreateRecorder(Options);
        _recorder.OnRecordingComplete += RecorderOnRecordingComplete;
        _recorder.OnRecordingFailed += RecorderOnRecordingFailed;

        _recorder.Record(videoPath);

        IsRecording = true;
        _logger.LogInformation("Started Recording...");

        return this;
    }

    private void TryToDeleteIdenticalFile(bool shouldDelete)
    {
        try
        {
            bool fileExists = File.Exists(RecordingFilePath);

            if (shouldDelete && fileExists)
            {
                File.Delete(RecordingFilePath!);
            }
            else if (fileExists)
            {
                RecordingFileName = IncrementNumberInParentheses(RecordingFileName!);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Failed to delete identical recording file");
            RecordingFileName = IncrementNumberInParentheses(RecordingFileName!);
        }
    }

    /// <summary>
    /// Searches if the input string ends with a number surrounded by parentheses.
    /// If it is found it returns the same string, but with the number incremented by 1.
    /// Otherwise the exact same string is returned;
    /// </summary>
    private string IncrementNumberInParentheses(string input)
    {
        var match = Regex.Match(input, @"\((\d+)\)$");
        if (match.Success == false) return $"{input} (1)";

        return $"{input.Substring(0, match.Groups[1].Index)}{int.Parse(match.Groups[1].Value) + 1})";
    }

    /// <summary>
    /// Stops the recording and starts the file saving process.
    /// IMPORTANT: When this method returns the file saving process is not
    /// done yet. If you want to execute code after it has finished use the
    /// OnRecordingComplete and/or OnRecordingFailed methods
    /// </summary>
    public IRecorder StopRecording()
    {
        _recorder?.Stop();
        _logger.LogInformation("Recording stop initiated");
        return this;
    }

    /// <summary>
    /// Executes the specified action when the recording
    /// finishes up and the file is saved successfully
    /// </summary>
    public IRecorder OnRecordingComplete(Action callbackDelegate)
    {
        _onRecordingComplete = callbackDelegate;
        return this;
    }

    public IRecorder OnRecordingComplete(Func<Task> callbackDelegate)
    {
        _onRecordingCompleteAsync = callbackDelegate;
        return this;
    }

    /// <summary>
    /// Executes the specified action when the recording
    /// finishes up and the file is saved successfully
    /// </summary>
    public IRecorder OnRecordingFailed(Action callbackDelegate)
    {
        _onRecordingFailed = callbackDelegate;
        return this;
    }

    public IRecorder OnRecordingFailed(Func<Task> callbackDelegate)
    {
        _onRecordingFailedAsync = callbackDelegate;
        return this;
    }

    private void RecorderOnRecordingComplete(object? sender, RecordingCompleteEventArgs e)
    {
        IsRecording = false;
        _logger.LogInformation("Recording completed successfully!");
        _onRecordingComplete?.Invoke();
        _onRecordingCompleteAsync?.Invoke();
        Dispose();
    }

    private void RecorderOnRecordingFailed(object? sender, RecordingFailedEventArgs e)
    {
        IsRecording = false;
        _logger.LogError("Recording failed to complete with error: {error}", e.Error);
        _onRecordingFailed?.Invoke();
        _onRecordingFailedAsync?.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        if (_recorder == null) return;

        _recorder.OnRecordingComplete -= RecorderOnRecordingComplete;
        _recorder.OnRecordingFailed -= RecorderOnRecordingFailed;
        _recorder.Dispose();
    }
}