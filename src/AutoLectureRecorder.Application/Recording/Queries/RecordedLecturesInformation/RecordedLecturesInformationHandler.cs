using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Application.Recording.Common;
using AutoLectureRecorder.Domain.Errors;
using ErrorOr;
using MediatR;

namespace AutoLectureRecorder.Application.Recording.Queries.RecordedLecturesInformation;

public class RecordedLecturesInformationHandler 
    : IRequestHandler<RecordedLecturesInformationQuery, ErrorOr<List<AlrVideoInformation>>>
{
    private readonly IVideoInformationRetriever _videoInformationRetriever;

    public RecordedLecturesInformationHandler(IVideoInformationRetriever videoInformationRetriever)
    {
        _videoInformationRetriever = videoInformationRetriever;
    }
    
    public async Task<ErrorOr<List<AlrVideoInformation>>> Handle(RecordedLecturesInformationQuery request, 
        CancellationToken cancellationToken)
    {
        List<AlrVideoInformation>? videosInfo = await _videoInformationRetriever
            .GetVideoInformationOfLecture(request.SubjectName, request.Semester);

        if (videosInfo is null)
        {
            return Errors.Recorder.NoVideosFound;
        }
        
        return videosInfo;
    }
}