using AutoLectureRecorder.Application.Recording.Common;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.Recording.Queries.RecordedLecturesInformation;

public record RecordedLecturesInformationQuery(
    string SubjectName,
    int Semester) : IRequest<ErrorOr<List<AlrVideoInformation>>>;