using System.Text.RegularExpressions;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Application.Recording;
using AutoLectureRecorder.Application.Recording.Common;
using AutoLectureRecorder.Domain.ReactiveModels;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ScreenRecorderLib;

namespace AutoLectureRecorder.Infrastructure.Recording;

public class WindowsRecorder : ReactiveObject, IRecorder
{
    private readonly ILogger<WindowsRecorder>? _logger;

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

    public WindowsRecorder(ILogger<WindowsRecorder>? logger)
    {
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public void ApplyRecordingSettings(ReactiveRecordingSettings settings)
    {
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

    /// <inheritdoc/>
    public ReactiveRecordingSettings GetDefaultSettings(int primaryScreenWidth, int primaryScreenHeight)
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
        (
            recordingsLocalPath: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "AutoLectureRecorder"),
            outputDeviceName: "Default",
            outputDeviceFriendlyName: "Default",
            inputDeviceName: "Default",
            inputDeviceFriendlyName: "Default",
            isInputDeviceEnabled: false,
            quality: 70,
            fps: 30,
            outputFrameWidth: outputFrameWidth,
            outputFrameHeight: outputFrameHeight
        );

        return settings;
    }

    /// <inheritdoc/>
    public IEnumerable<ReactiveAudioDevice> GetInputAudioDevices()
    {
        return Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices)
            .Select(audioDevice => new ReactiveAudioDevice()
            {
                DeviceName = audioDevice.DeviceName,
                FriendlyName = audioDevice.FriendlyName,
            });
    }

    /// <inheritdoc/>
    public IEnumerable<ReactiveAudioDevice> GetOutputAudioDevices()
    {
        return Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices)
            .Select(audioDevice => new ReactiveAudioDevice()
            {
                DeviceName = audioDevice.DeviceName,
                FriendlyName = audioDevice.FriendlyName,
            });
    }
    
    // TODO: Before recording check if the selected dimension exceeds the one of the screen and modify it accordingly
    /// <inheritdoc/>
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
        _logger?.LogInformation("Started Recording...");

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
            _logger?.LogWarning(ex, "Failed to delete identical recording file");
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

    /// <inheritdoc/>
    public IRecorder StopRecording()
    {
        _recorder?.Stop();
        _logger?.LogInformation("Recording stop initiated");
        return this;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
        _logger?.LogInformation("Recording completed successfully!");
        _onRecordingComplete?.Invoke();
        _onRecordingCompleteAsync?.Invoke();
        Dispose();
    }

    private void RecorderOnRecordingFailed(object? sender, RecordingFailedEventArgs e)
    {
        IsRecording = false;
        _logger?.LogError("Recording failed to complete with error: {Error}", e.Error);
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