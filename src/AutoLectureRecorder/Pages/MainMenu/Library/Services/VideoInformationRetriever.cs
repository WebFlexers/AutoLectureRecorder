using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Application.Recording.Common;

namespace AutoLectureRecorder.Pages.MainMenu.Library.Services;

public class VideoInformationRetriever : IVideoInformationRetriever
{
    private readonly IRecordingsRepository _recordingsRepository;
    
    static string[] _videoExtensions = { "avi", "mp4" };

    public VideoInformationRetriever(IRecordingsRepository recordingsRepository)
    {
        _recordingsRepository = recordingsRepository;
    }
    
    public async Task<List<AlrVideoInformation>?> GetVideoInformationOfLecture(string subjectName, int semester)
    {
        var recordingFilesDirectories = await 
            _recordingsRepository.GetAllRecordingDirectories();

        if (recordingFilesDirectories is null) return null;

        var recordingsInformation = new List<AlrVideoInformation>();
        foreach (var recordingFilesRootDirectory in recordingFilesDirectories)
        {
            var lectureDirectory = Path.Combine(recordingFilesRootDirectory.Path, 
                $"Semester {semester}", subjectName);
            
            if (Directory.Exists(lectureDirectory) == false) return null;
            
            var videosPaths = Directory.GetFiles(lectureDirectory);
            
            foreach (var videoPath in videosPaths)
            {
                var videoStorageFile = await StorageFile.GetFileFromPathAsync(videoPath);

                if (_videoExtensions.FirstOrDefault(videoStorageFile.FileType) is null) continue;
                
                var videoFileInfo = new FileInfo(videoPath);
                var videoProperties = await videoStorageFile.Properties.GetVideoPropertiesAsync();

                DateTime startedAt = videoStorageFile.DateCreated.DateTime;
                TimeSpan videoDuration = videoProperties.Duration;
                DateTime endedAt = startedAt.Add(videoDuration);
                
                var alrVideoInformation = new AlrVideoInformation(
                    videoStorageFile.DisplayName,
                    ConvertBytesToGigabytes(videoFileInfo.Length),
                    startedAt,
                    endedAt, 
                    videoDuration,
                    videoPath);
                
                recordingsInformation.Add(alrVideoInformation);
            }
        }

        return recordingsInformation.OrderBy(info => info.StartedAt).ToList();
    }
    
    static float ConvertBytesToGigabytes(long bytes)
    {
        const float gigabyte = 1024 * 1024 * 1024;
        return bytes / gigabyte;
    }
}