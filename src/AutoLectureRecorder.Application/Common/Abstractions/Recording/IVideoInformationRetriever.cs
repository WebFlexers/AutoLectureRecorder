using AutoLectureRecorder.Application.Recording.Common;

namespace AutoLectureRecorder.Application.Common.Abstractions.Recording;

public interface IVideoInformationRetriever
{
    Task<List<AlrVideoInformation>?> GetVideoInformationOfLecture(string subjectName, int semester);
}