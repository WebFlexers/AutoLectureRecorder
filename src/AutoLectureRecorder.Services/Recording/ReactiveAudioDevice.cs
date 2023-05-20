using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Services.Recording;

public class ReactiveAudioDevice : ReactiveObject
{
    [Reactive]
    public string DeviceName { get; set; }
    [Reactive]
    public string FriendlyName { get; set; }
}
