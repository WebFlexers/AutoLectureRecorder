using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;

public static class StatisticsMapping
{
    public static ReactiveStatistics MapToReactive(this Statistics input)
    {
        return new ReactiveStatistics(
            input.TotalRecordAttempts,
            input.RecordingSucceededNumber,
            input.RecordingFailedNumber);
    }
}