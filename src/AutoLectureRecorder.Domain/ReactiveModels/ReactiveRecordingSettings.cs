using ReactiveUI;

namespace AutoLectureRecorder.Domain.ReactiveModels;

public class ReactiveRecordingSettings : ReactiveObject
{
    public ReactiveRecordingSettings(string recordingsLocalPath, string outputDeviceName, 
        string outputDeviceFriendlyName, string inputDeviceName, string inputDeviceFriendlyName, 
        bool isInputDeviceEnabled, int quality, int fps, int outputFrameWidth, int outputFrameHeight)
    {
        _recordingsLocalPath = recordingsLocalPath;
        _outputDeviceName = outputDeviceName;
        _outputDeviceFriendlyName = outputDeviceFriendlyName;
        _inputDeviceName = inputDeviceName;
        _inputDeviceFriendlyName = inputDeviceFriendlyName;
        _isInputDeviceEnabled = isInputDeviceEnabled;
        _quality = quality;
        _fps = fps;
        _outputFrameWidth = outputFrameWidth;
        _outputFrameHeight = outputFrameHeight;
    }

    private string _recordingsLocalPath;
    public string RecordingsLocalPath {
        get => _recordingsLocalPath;
        set => this.RaiseAndSetIfChanged(ref _recordingsLocalPath, value);
    }
    
    private string _outputDeviceName;
    public string OutputDeviceName {
        get => _outputDeviceName;
        set => this.RaiseAndSetIfChanged(ref _outputDeviceName, value);
    }

    private string _outputDeviceFriendlyName;
    public string OutputDeviceFriendlyName {
        get => _outputDeviceFriendlyName;
        set => this.RaiseAndSetIfChanged(ref _outputDeviceFriendlyName, value);
    }

    private string _inputDeviceName;
    public string InputDeviceName {
        get => _inputDeviceName;
        set => this.RaiseAndSetIfChanged(ref _inputDeviceName, value);
    }

    private string _inputDeviceFriendlyName;
    public string InputDeviceFriendlyName {
        get => _inputDeviceFriendlyName;
        set => this.RaiseAndSetIfChanged(ref _inputDeviceFriendlyName, value);
    }

    private bool _isInputDeviceEnabled;
    public bool IsInputDeviceEnabled {
        get => _isInputDeviceEnabled;
        set => this.RaiseAndSetIfChanged(ref _isInputDeviceEnabled, value);
    }

    private int _quality;
    public int Quality {
        get => _quality;
        set => this.RaiseAndSetIfChanged(ref _quality, value);
    }

    private int _fps;
    public int Fps {
        get => _fps;
        set => this.RaiseAndSetIfChanged(ref _fps, value);
    }

    private int _outputFrameWidth;
    public int OutputFrameWidth {
        get => _outputFrameWidth;
        set => this.RaiseAndSetIfChanged(ref _outputFrameWidth, value);
    }

    private int _outputFrameHeight;
    public int OutputFrameHeight {
        get => _outputFrameHeight;
        set => this.RaiseAndSetIfChanged(ref _outputFrameHeight, value);
    }
}
