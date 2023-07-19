using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture.Mapping;

public static class UpdateScheduledLectureCommandMapping
{
    public static UpdateScheduledLectureCommand MapToUpdateCommand(this ReactiveScheduledLecture lecture, 
        bool ignoreOverlappingLecturesWarning)
    {
        return new UpdateScheduledLectureCommand(
            id: lecture.Id,
            subjectName: lecture.SubjectName,
            semester: lecture.Semester,
            meetingLink: lecture.MeetingLink,
            day: lecture.Day,
            startTime: lecture.StartTime,
            endTime: lecture.EndTime,
            isScheduled: lecture.IsScheduled,
            willAutoUpload: lecture.WillAutoUpload,
            ignoreOverlappingLecturesWarning: ignoreOverlappingLecturesWarning);
    }

    public static ReactiveScheduledLecture MapToReactiveModel(this UpdateScheduledLectureCommand lecture)
    {
        return new ReactiveScheduledLecture()
        {
            Id = lecture.Id,
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