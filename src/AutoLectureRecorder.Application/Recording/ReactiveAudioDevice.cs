using ReactiveUI;

namespace AutoLectureRecorder.Application.Recording;

public class ReactiveAudioDevice : ReactiveObject
{
    private string _deviceName;
    public string DeviceName
    {
        get => _deviceName;
        set => this.RaiseAndSetIfChanged(ref _deviceName, value);
    }

    private string _friendlyName;
    public string FriendlyName
    {
        get => _friendlyName;
        set => this.RaiseAndSetIfChanged(ref _friendlyName, value);
    }
}
