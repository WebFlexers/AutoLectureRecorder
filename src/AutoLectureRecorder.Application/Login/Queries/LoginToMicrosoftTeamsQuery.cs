using ErrorOr;
using MediatR;
using Unit = System.Reactive.Unit;


namespace AutoLectureRecorder.Application.Login.Queries;

public record LoginToMicrosoftTeamsQuery(
    string AcademicEmailAddress,
    string Password) : IRequest<ErrorOr<Unit>>;