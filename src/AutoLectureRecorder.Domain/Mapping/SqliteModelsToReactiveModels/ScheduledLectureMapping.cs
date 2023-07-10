using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;

public static class ScheduledLectureMapping
{
    public static ReactiveScheduledLecture MapToReactive(this ScheduledLecture input)
    {
        return new ReactiveScheduledLecture(
            input.SubjectName,
            input.Semester,
            input.MeetingLink,
            (DayOfWeek)input.Day,
            TimeOnly.Parse(input.StartTime),
            TimeOnly.Parse(input.EndTime),
            Convert.ToBoolean(input.IsScheduled),
            Convert.ToBoolean(input.WillAutoUpload),
            input.Id);
    }
}