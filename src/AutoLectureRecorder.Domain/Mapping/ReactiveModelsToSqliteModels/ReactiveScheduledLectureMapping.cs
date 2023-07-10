using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;

public static class ReactiveScheduledLectureMapping
{
    public static ScheduledLecture MapToSqliteModel(this ReactiveScheduledLecture input)
    {
        return new ScheduledLecture(
            input.Id,
            input.SubjectName,
            input.Semester,
            input.MeetingLink,
            (int)input.Day,
            input.StartTime.ToString(),
            input.EndTime.ToString(),
            Convert.ToInt32(input.IsScheduled),
            Convert.ToInt32(input.WillAutoUpload)
        );
    }
}