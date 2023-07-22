using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.Common.Abstractions.Persistence;

public interface IRecordingsRepository
{
    Task<bool> AddRecordingDirectory(string path);
    Task<IEnumerable<ReactiveRecordingsStorageDirectory>?> GetAllRecordingDirectories();
}