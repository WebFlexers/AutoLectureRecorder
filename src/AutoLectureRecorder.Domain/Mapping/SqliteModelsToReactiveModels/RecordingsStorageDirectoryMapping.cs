using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;

public static class RecordingsStorageDirectoryMapping
{
    public static ReactiveRecordingsStorageDirectory MapToReactive(this RecordingsStorageDirectory input)
    {
        return new ReactiveRecordingsStorageDirectory(input.Path);
    }
}