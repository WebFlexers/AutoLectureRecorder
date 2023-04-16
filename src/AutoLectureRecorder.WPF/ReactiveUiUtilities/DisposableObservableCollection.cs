using System;
using System.Collections.ObjectModel;

namespace AutoLectureRecorder.ReactiveUiUtilities;

public class DisposableObservableCollection<T> : ObservableCollection<T>, IDisposable
{
    public void Dispose()
    {
        this.ClearItems();
        this.Clear();
    }
}
