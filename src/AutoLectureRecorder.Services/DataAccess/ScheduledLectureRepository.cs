using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Services.DataAccess;

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
    public async Task<List<ReactiveScheduledLecture>?> GetAllScheduledLectures()
    {
        try
        {
            string sql = "select * from ScheduledLectures";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

            var reactiveScheduledLectures = new List<ReactiveScheduledLecture>();
            foreach (var scheduledLecture in results)
            {
                var reactiveLecture = CreateReactiveScheduledLecture(scheduledLecture);
                reactiveScheduledLectures.Add(reactiveLecture);
            
            }

            return reactiveScheduledLectures;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lectures sorted by day and start time");
            return null;
        }
    }

    /// <summary>
    /// Gets all the scheduled lectures sorted by Day first and then by Start Time from the database
    /// </summary>
    public async Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedByDayAndStartTime()
    {
        try
        {
            string sql = "select * from ScheduledLectures order by Day, StartTime asc";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

            var reactiveScheduledLectures = new List<ReactiveScheduledLecture>();
            foreach (var scheduledLecture in results)
            {
                var reactiveLecture = CreateReactiveScheduledLecture(scheduledLecture);
                reactiveScheduledLectures.Add(reactiveLecture);
            }

            return reactiveScheduledLectures;
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
    public async Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesByDay(DayOfWeek? day)
    {
        if (day == null) return null;

        try
        {
            string sql = "select * from ScheduledLectures where Day=@Day";

            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Day = (int)day.Value }).ConfigureAwait(false);

            var reactiveScheduledLectures = new List<ReactiveScheduledLecture>();
            foreach (var scheduledLecture in results)
            {
                var reactiveLecture = CreateReactiveScheduledLecture(scheduledLecture);
                reactiveScheduledLectures.Add(reactiveLecture);
            }

            return reactiveScheduledLectures;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture with day {day}", day);
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
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Id = id }).ConfigureAwait(false);
            var scheduledLecture = result.FirstOrDefault();

            return scheduledLecture != null ? CreateReactiveScheduledLecture(scheduledLecture) : null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture with id {id}", id);
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
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { SubjectName = subjectName }).ConfigureAwait(false);
            var scheduledLecture = result.FirstOrDefault();

            return scheduledLecture != null ? CreateReactiveScheduledLecture(scheduledLecture) : null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get scheduled lecture with subject name {name}", subjectName);
            return null;
        }
    }

    /// <summary>
    /// Gets the distinct subject names from the database
    /// </summary>
    public async Task<List<string>?> GetDistinctSubjectNames()
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
    public async Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesGroupedByName()
    {
        try
        {
            string sql = "select * from ScheduledLectures group by SubjectName";
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

            return result.Select(CreateReactiveScheduledLecture).ToList();
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
    public async Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedBySemester()
    {
        try
        {
            string sql = "select * from ScheduledLectures order by Semester";
            var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);
            return result.Select(CreateReactiveScheduledLecture).ToList();
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
        if (reactiveLecture.Day.HasValue == false) return null;

        try
        {
            var scheduledLecture = CreateScheduledLecture(reactiveLecture);

            string sql = "insert into ScheduledLectures (SubjectName, Semester, MeetingLink, Day, StartTime, EndTime, IsScheduled, WillAutoUpload) " +
                         "values (@SubjectName, @Semester, @MeetingLink, @Day, @StartTime, @EndTime, @IsScheduled, @WillAutoUpload)";

            int rowsAffectedNum = await _dataAccess.SaveData(sql, scheduledLecture).ConfigureAwait(false);

            if (rowsAffectedNum == 0)
            {
                _logger.LogWarning("No values were inserted while trying to insert a new scheduled lecture with name {name}", reactiveLecture.SubjectName);
                return null;
            }

            sql = "select * from ScheduledLectures where Id = (select last_insert_rowid())";
            var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

            _logger.LogInformation("Successfully inserted scheduled lecture with name {name}", reactiveLecture.SubjectName);

            return CreateReactiveScheduledLecture(results.First());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to insert scheduled lecture with name {name}", reactiveLecture.SubjectName);
            return null;
        }
    }

    /// <summary>
    /// Updates the scheduled lecture using the Id
    /// </summary>
    public async Task<bool> UpdateScheduledLecture(ReactiveScheduledLecture reactiveLecture)
    {
        if (reactiveLecture.Day.HasValue == false) return false;

        try
        {
            var scheduledLecture = CreateScheduledLecture(reactiveLecture);

            string sql = "update ScheduledLectures " +
                         "set SubjectName = @SubjectName, Semester = @Semester, MeetingLink = @MeetingLink, Day = @Day, StartTime = @StartTime, " +
                         "EndTime = @EndTime, IsScheduled = @IsScheduled, WillAutoUpload = @WillAutoUpload " +
                         "where Id = @Id";

            int rowsAffectedNum = await _dataAccess.SaveData(sql, scheduledLecture).ConfigureAwait(false);

            if (rowsAffectedNum == 0)
            {
                _logger.LogWarning("Failed to update lecture with id {id}, because it doesn't exist", reactiveLecture.Id);
                return false;
            }
            
            _logger.LogInformation("Successfully updated lecture with Id {id}", reactiveLecture.Id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to update scheduled lecture with id {id}", reactiveLecture.Id);
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
            
            _logger.LogInformation("Successfully deleted lecture with Id {id}", id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to delete scheduled lecture with id {id}", id);
            return false;
        }
    }

    private ReactiveScheduledLecture CreateReactiveScheduledLecture(ScheduledLecture scheduledLecture)
    {
        return new ReactiveScheduledLecture
        {
            Id = scheduledLecture.Id,
            SubjectName = scheduledLecture.SubjectName,
            Semester = scheduledLecture.Semester,
            MeetingLink = scheduledLecture.MeetingLink,
            Day = (DayOfWeek)scheduledLecture.Day,
            StartTime = Convert.ToDateTime(scheduledLecture.StartTime),
            EndTime = Convert.ToDateTime(scheduledLecture.EndTime),
            IsScheduled = Convert.ToBoolean(scheduledLecture.IsScheduled),
            WillAutoUpload = Convert.ToBoolean(scheduledLecture.WillAutoUpload),
        };
    }

    private ScheduledLecture CreateScheduledLecture(ReactiveScheduledLecture reactiveScheduledLecture)
    {
        if (reactiveScheduledLecture.Day.HasValue == false)
        {
            throw new ArgumentNullException(nameof(reactiveScheduledLecture.Day), 
                "Day was null while trying to convert from ReactiveScheduledLecture to ScheduledLecture");
        }

        return new ScheduledLecture
        {
            Id = reactiveScheduledLecture.Id,
            SubjectName = reactiveScheduledLecture.SubjectName,
            Semester = reactiveScheduledLecture.Semester,
            MeetingLink = reactiveScheduledLecture.MeetingLink,
            Day = (int)reactiveScheduledLecture.Day,
            StartTime = reactiveScheduledLecture.StartTime?.ToString("HH:mm"),
            EndTime = reactiveScheduledLecture.EndTime?.ToString("HH:mm"),
            IsScheduled = Convert.ToInt32(reactiveScheduledLecture.IsScheduled),
            WillAutoUpload = Convert.ToInt32(reactiveScheduledLecture.WillAutoUpload)
        };
    }
}
