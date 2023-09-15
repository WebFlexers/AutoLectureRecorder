using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;
using AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Infrastructure.Persistence;

public class ScheduledLectureRepository : IScheduledLectureRepository
{
    private readonly ISqliteDataAccess _dataAccess;
    private readonly ILogger<ScheduledLectureRepository> _logger;

    public ScheduledLectureRepository(ISqliteDataAccess dataAccess, ILogger<ScheduledLectureRepository> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
    }

    /// <summary>
    /// Gets all the scheduled lectures from the database
    /// </summary>
    public async Task<IEnumerable<ReactiveScheduledLecture>?> GetAllScheduledLectures()
    {
        try
        {
            string sql = "select * from ScheduledLectures";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { }).ConfigureAwait(false);

            return results.Select(lecture => lecture.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lectures sorted by day and start time");
            return null;
        }
    }

    /// <summary>
    /// Gets all the scheduled lectures sorted by Day first then by Start Time and then by Subject Name from the database
    /// </summary>
    public async Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesOrdered()
    {
        try
        {
            string sql = "select * from ScheduledLectures order by Day, StartTime, SubjectName";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { }).ConfigureAwait(false);

            return results.Select(lecture => lecture.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lectures sorted by day and start time");
            return null;
        }
    }

    /// <summary>
    /// Gets all the scheduled lectures of the given day from the database
    /// </summary>
    public async Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesByDay(DayOfWeek? day)
    {
        if (day == null) return null;

        try
        {
            string sql = "select * from ScheduledLectures where Day=@Day";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { Day = (int)day.Value }).ConfigureAwait(false);

            return results.Select(lecture => lecture.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture with day {Day}", day);
            return null;
        }
    }

    public async Task<IEnumerable<ReactiveScheduledLecture>?> GetActiveScheduledLecturesByDay(DayOfWeek? day)
    {
        if (day == null) return null;

        try
        {
            string sql = "select * from ScheduledLectures where Day=@Day and IsScheduled=1";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { Day = (int)day.Value }).ConfigureAwait(false);

            return results.Select(lecture => lecture.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture with day {Day}", day);
            return null;
        }
    }

    /// <summary>
    /// Gets all the scheduled lectures of the given day ordered by start time from the database
    /// </summary>
    public async Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesByDayOrderedByStartTime(DayOfWeek? day)
    {
        if (day == null) return null;

        try
        {
            string sql = "select * from ScheduledLectures where Day=@Day order by StartTime, SubjectName";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { Day = (int)day.Value }).ConfigureAwait(false);

            return results.Select(lecture => lecture.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get ordered by start time " +
                                "scheduled lectures with day {Day}", day);
            return null;
        }
    }

    /// <summary>
    /// Gets the scheduled lecture with the given id from the database
    /// </summary>
    public async Task<ReactiveScheduledLecture?> GetScheduledLectureById(int id)
    {
        try
        {
            string sql = "select * from ScheduledLectures where Id=@Id";
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { Id = id }).ConfigureAwait(false);
            var scheduledLecture = result.FirstOrDefault();

            return scheduledLecture?.MapToReactive();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture with id {Id}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets the first Scheduled Lecture with the given Subject Name from the database
    /// </summary>
    public async Task<ReactiveScheduledLecture?> GetScheduledLectureBySubjectName(string subjectName)
    {
        try
        {
            string sql = "select * from ScheduledLectures where SubjectName=@SubjectName";
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { SubjectName = subjectName }).ConfigureAwait(false);
            var scheduledLecture = result.FirstOrDefault();

            return scheduledLecture?.MapToReactive();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture with subject name {Name}", 
                subjectName);
            return null;
        }
    }

    /// <summary>
    /// Gets the distinct subject names from the database
    /// </summary>
    public async Task<IEnumerable<string>?> GetDistinctSubjectNames()
    {
        try
        {
            string sql = "select distinct SubjectName from ScheduledLectures";
            return await _dataAccess.LoadData<string, dynamic>(sql, new { }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get distinct lecture subject names");
            return null;
        }
    }

    /// <summary>
    /// Gets the scheduled lectures with distinct subject names
    /// </summary>
    public async Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesGroupedByName()
    {
        try
        {
            string sql = "select * from ScheduledLectures group by SubjectName";
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { }).ConfigureAwait(false);

            return result.Select(scheduledLecture => scheduledLecture.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture grouped by name");
            return null;
        }
    }

    /// <summary>
    /// Gets the scheduled lectures grouped by semester
    /// </summary>
    public async Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedBySemester()
    {
        try
        {
            string sql = "select * from ScheduledLectures order by Semester";
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { }).ConfigureAwait(false);
            return result.Select(scheduledLecture => scheduledLecture.MapToReactive());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture grouped by semester");
            return null;
        }
    }

    /// <summary>
    /// Inserts a new Scheduled Lecture in the database and returns it as a ReactiveScheduledLecture
    /// </summary>
    public async Task<ReactiveScheduledLecture?> InsertScheduledLecture(ReactiveScheduledLecture reactiveLecture)
    {
        try
        {
            string sql = "insert into ScheduledLectures (SubjectName, Semester, MeetingLink, Day, StartTime, EndTime, " +
                         "IsScheduled, WillAutoUpload) " +
                         "values (@SubjectName, @Semester, @MeetingLink, @Day, @StartTime, @EndTime, " +
                         "@IsScheduled, @WillAutoUpload)";

            int rowsAffectedNum = await _dataAccess.SaveData(sql, reactiveLecture.MapToSqliteModel())
                .ConfigureAwait(false);

            if (rowsAffectedNum == 0)
            {
                _logger.LogWarning("No values were inserted while trying to insert a new scheduled lecture " +
                                   "with name {Name}", reactiveLecture.SubjectName);
                return null;
            }

            sql = "select * from ScheduledLectures where Id = (select last_insert_rowid())";
            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(
                sql, new { }).ConfigureAwait(false);

            _logger.LogInformation("Successfully inserted scheduled lecture with name {Name}", 
                reactiveLecture.SubjectName);

            return results.First().MapToReactive();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to insert scheduled lecture with name {Name}", 
                reactiveLecture.SubjectName);
            return null;
        }
    }

    /// <summary>
    /// Updates the scheduled lecture using the Id
    /// </summary>
    public async Task<bool> UpdateScheduledLecture(ReactiveScheduledLecture reactiveLecture)
    {
        try
        {
            string sql = "update ScheduledLectures " +
                         "set SubjectName = @SubjectName, Semester = @Semester, MeetingLink = @MeetingLink, Day = @Day, " +
                         "StartTime = @StartTime, EndTime = @EndTime, IsScheduled = @IsScheduled, " +
                         "WillAutoUpload = @WillAutoUpload where Id = @Id";

            int rowsAffectedNum = await _dataAccess.SaveData(sql, reactiveLecture.MapToSqliteModel())
                .ConfigureAwait(false);

            if (rowsAffectedNum == 0)
            {
                _logger.LogWarning("Failed to update lecture with id {Id}, because it doesn't exist", 
                    reactiveLecture.Id);
                return false;
            }

            _logger.LogInformation("Successfully updated lecture with Id {Id}", reactiveLecture.Id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to update scheduled lecture with id {Id}", reactiveLecture.Id);
            return false;
        }
    }

    /// <summary>
    /// Deletes the lecture with the given id if it exists
    /// </summary>
    public async Task<bool> DeleteScheduledLectureById(int id)
    {
        try
        {
            string sql = "delete from ScheduledLectures where Id=@Id";
            int rowsAffectedNum = await _dataAccess.SaveData<dynamic>(sql, new { Id = id });

            if (rowsAffectedNum == 0)
            {
                _logger.LogWarning("Failed to delete lecture with the given id, because it doesn't exist");
                return false;
            }

            _logger.LogInformation("Successfully deleted lecture with Id {Id}", id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to delete scheduled lecture with id {Id}", id);
            return false;
        }
    }
}
