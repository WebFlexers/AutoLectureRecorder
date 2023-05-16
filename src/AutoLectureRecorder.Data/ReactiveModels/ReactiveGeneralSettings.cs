using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveGeneralSettings : ReactiveObject
{
    [Reactive]
    public bool LaunchAtStartup { get; set; }
    [Reactive]
    public bool OnCloseKeepAlive { get; set; }
    [Reactive]
    public bool ShowSplashScreen { get; set; }
}
