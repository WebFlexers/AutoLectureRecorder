using AutoLectureRecorder.Data.DTOs;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Services.DataAccess;

public class RecordedLectureRepository : IRecordedLectureRepository
{
    private readonly ISqliteDataAccess _dataAccess;
    private readonly ILogger<RecordedLectureRepository> _logger;

    public RecordedLectureRepository(ISqliteDataAccess dataAccess, ILogger<RecordedLectureRepository> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
    }

    /// <summary>
    /// Fetch the recorded lectures of the scheduled lecture with the
    /// provided id
    /// </summary>
    public async Task<List<ReactiveRecordedLecture>?> GetRecordedLecturesFromIdAsync(int scheduledLectureId)
    {
        try
        {
            string sql =
                @"select RecordedLectures.Id, StudentRegistrationNumber, SubjectName, Semester, CloudLink, StartedAt, EndedAt, ScheduledLectureId
                  from RecordedLectures, ScheduledLectures
                  where ScheduledLectures.Id=ScheduledLectureId and ScheduledLectureId=@ScheduledLectureId";

            var results = await _dataAccess.LoadData<RecordedLectureDTO, dynamic>
                (sql, new { ScheduledLectureId = scheduledLectureId }).ConfigureAwait(false);

            return results.Select(ConvertRecordedLectureToReactive).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to fetch recorded lectures from id");
            return null;
        }
    }

    private ReactiveRecordedLecture ConvertRecordedLectureToReactive(RecordedLectureDTO dto)
    {
        return new ReactiveRecordedLecture
        {
            Id = dto.Id,
            StudentRegistrationNumber = dto.StudentRegistrationNumber,
            SubjectName = dto.SubjectName,
            Semester = dto.Semester,
            CloudLink = dto.CloudLink,
            StartedAt = Convert.ToDateTime(dto.StartedAt),
            EndedAt = Convert.ToDateTime(dto.EndedAt),
            ScheduledLectureId = dto.ScheduledLectureId
        };
    }
}
