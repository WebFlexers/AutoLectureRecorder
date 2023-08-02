using ErrorOr;
using MediatR;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Application.Recording.Queries.JoinMeeting;

public record JoinMeetingQuery(
    string MeetingLink,
    TimeSpan LectureDuration) : IRequest<ErrorOr<Unit>>;