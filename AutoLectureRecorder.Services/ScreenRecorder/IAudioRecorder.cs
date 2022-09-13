namespace AutoLectureRecorder.Services.ScreenRecorder;

public interface IAudioDevices
{
    IReadOnlyCollection<KeyValuePair<string, string>> AudioInputDevices { get; }
    IReadOnlyCollection<KeyValuePair<string, string>> AudioOutputDevices { get; }
}