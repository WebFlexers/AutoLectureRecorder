using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture.Mapping;

public static class CreateScheduledLectureCommandMapping
{
    public static CreateScheduledLectureCommand MapToCreateCommand(this ReactiveScheduledLecture lecture, 
        bool ignoreValidationWarnings)
    {
        return new CreateScheduledLectureCommand(
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

    public static ReactiveScheduledLecture MapToReactiveModel(this CreateScheduledLectureCommand lecture)
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