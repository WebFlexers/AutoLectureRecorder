using ReactiveUI;

namespace AutoLectureRecorder.Domain.ReactiveModels;

public class ReactiveGeneralSettings : ReactiveObject
{
    public ReactiveGeneralSettings(bool onCloseKeepAlive)
    {
        _onCloseKeepAlive = onCloseKeepAlive;
    }
    
    private bool _onCloseKeepAlive;
    public bool OnCloseKeepAlive
    {
        get => _onCloseKeepAlive;
        set => this.RaiseAndSetIfChanged(ref _onCloseKeepAlive, value);
    }
}
