using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess.Interfaces;

public interface IRecordedLectureRepository
{
    /// <summary>
    /// Fetch the recorded lectures of the scheduled lecture with the
    /// provided id
    /// </summary>
    Task<List<ReactiveRecordedLecture>?> GetRecordedLecturesFromIdAsync(int scheduledLectureId);
}