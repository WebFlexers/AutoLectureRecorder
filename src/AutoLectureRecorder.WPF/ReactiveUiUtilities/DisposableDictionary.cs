using System;
using System.Collections.Generic;

namespace AutoLectureRecorder.ReactiveUiUtilities;

public class DisposableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable 
    where TKey : notnull
    where TValue : IDisposable
{
    public void Dispose()
    {
        foreach (var value in this.Values)
        {
            value.Dispose();
        }
        this.Clear();
    }
}
