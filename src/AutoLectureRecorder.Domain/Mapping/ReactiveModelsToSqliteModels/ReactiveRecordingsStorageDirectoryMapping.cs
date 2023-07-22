using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;

public static class ReactiveRecordingsStorageDirectoryMapping
{
    public static RecordingsStorageDirectory MapToSqliteModel(this ReactiveRecordingsStorageDirectory input)
    {
        return new RecordingsStorageDirectory(input.Path);
    }
}