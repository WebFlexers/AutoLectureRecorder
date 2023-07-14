using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture.Mapping;

public static class UpdateScheduledLectureCommandMapping
{
    public static UpdateScheduledLectureCommand MapToUpdateCommand(this ReactiveScheduledLecture lecture, 
        bool ignoreValidationWarnings)
    {
        return new UpdateScheduledLectureCommand(
            lecture.SubjectName,
            lecture.Semester,
            lecture.MeetingLink,
            lecture.Day,
            lecture.StartTime,
            lecture.EndTime,
            lecture.IsScheduled,
            lecture.WillAutoUpload,
            ignoreValidationWarnings);
    }

    public static ReactiveScheduledLecture MapToReactiveModel(this UpdateScheduledLectureCommand lecture)
    {
        return new ReactiveScheduledLecture()
        {
            SubjectName = lecture.SubjectName,
            Semester = lecture.Semester,
            MeetingLink = lecture.MeetingLink,
            Day = lecture.Day,
            StartTime = lecture.StartTime,
            EndTime = lecture.EndTime,
            IsScheduled = lecture.IsScheduled,
            WillAutoUpload = lecture.WillAutoUpload
        };
    }
}