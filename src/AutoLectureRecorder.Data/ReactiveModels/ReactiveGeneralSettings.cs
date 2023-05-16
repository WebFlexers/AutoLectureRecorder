using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Data.ReactiveModels;

public class ReactiveGeneralSettings : ReactiveObject
{
    [Reactive]
    public int LaunchAtStartup { get; set; }
    [Reactive]
    public int OnCloseKeepAlive { get; set; }
    [Reactive]
    public int ShowSplashScreen { get; set; }
}
