namespace AutoLectureRecorder.Services.ScreenRecorder.Interfaces;

public interface IAudioDevices
{
    List<string> GetAudioInputDevices { get; }
    List<string> GetAudioOutputDevices { get; }
    string SelectedAudioInputDevice { get; set; }
    string SelectedAudioOutputDevice { get; set; }
}