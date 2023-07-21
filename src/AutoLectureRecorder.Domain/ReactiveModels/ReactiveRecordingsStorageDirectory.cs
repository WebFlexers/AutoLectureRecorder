using ReactiveUI;

namespace AutoLectureRecorder.Domain.ReactiveModels;

public class ReactiveRecordingsStorageDirectory : ReactiveObject
{
    private string _path;
    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public ReactiveRecordingsStorageDirectory(string path)
    {
        _path = path;
    }
}