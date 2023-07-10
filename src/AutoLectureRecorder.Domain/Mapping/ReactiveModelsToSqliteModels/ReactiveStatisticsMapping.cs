using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;

public static class ReactiveStatisticsMapping
{
    public static Statistics MapToSqliteModel(this ReactiveStatistics input)
    {
        return new Statistics(
            input.TotalRecordAttempts,
            input.RecordingSucceededNumber,
            input.RecordingFailedNumber);
    }
}