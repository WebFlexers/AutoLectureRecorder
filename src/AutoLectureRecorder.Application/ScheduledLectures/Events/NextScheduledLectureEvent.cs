using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Events;

public record NextScheduledLectureEvent() : INotification;