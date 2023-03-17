using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;

public class ScheduledLectureRepository : IScheduledLectureRepository
{
    private readonly ISqliteDataAccess _dataAccess;

    public ScheduledLectureRepository(ISqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    /// <summary>
    /// Inserts a new Scheduled Lecture in the database and returns it as a ReactiveScheduledLecture asynchronously
    /// </summary>
    public async Task<ReactiveScheduledLecture?> InsertScheduledLectureAsync(string subjectName, int semester, string meetingLink, DayOfWeek? day,
                                             DateTime? startTime, DateTime? endTime, bool isScheduled, bool willAutoUpload)
    {
        var scheduledLecture = new ScheduledLecture
        {
            SubjectName = subjectName,
            Semester = semester,
            MeetingLink = meetingLink,
            Day = (int)day,
            StartTime = startTime?.ToString("HH:mm"),
            EndTime = endTime?.ToString("HH:mm"),
            IsScheduled = Convert.ToInt32(isScheduled),
            WillAutoUpload = Convert.ToInt32(willAutoUpload)
        };

        string sql = "insert into ScheduledLectures (SubjectName, Semester, MeetingLink, Day, StartTime, EndTime, IsScheduled, WillAutoUpload) " +
                             "values (@SubjectName, @Semester, @MeetingLink, @Day, @StartTime, @EndTime, @IsScheduled, @WillAutoUpload)";

        await _dataAccess.SaveData(sql, scheduledLecture).ConfigureAwait(false);

        sql = "select * from ScheduledLectures where Id = (select last_insert_rowid())";
        var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

        return CreateReactiveScheduledLecture(results.FirstOrDefault());
    }

    /// <summary>
    /// Gets all the scheduled lectures from the database asynchronously
    /// </summary>
    public async Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesAsync()
    {
        string sql = "select * from ScheduledLectures";

        var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

        var reactiveScheduledLectures = new List<ReactiveScheduledLecture>();
        foreach (var scheduledLecture in results)
        {
            var reactiveLecture = CreateReactiveScheduledLecture(scheduledLecture);
            if (reactiveLecture != null)
            {
                reactiveScheduledLectures.Add(reactiveLecture);
            }
        }

        return reactiveScheduledLectures;
    }

    /// <summary>
    /// Gets all the scheduled lectures sorted by Day first and then by Start Time from the database asynchronously
    /// </summary>
    public async Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesSortedAsync()
    {
        string sql = "select * from ScheduledLectures order by Day, StartTime asc";

        var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

        var reactiveScheduledLectures = new List<ReactiveScheduledLecture>();
        foreach (var scheduledLecture in results)
        {
            var reactiveLecture = CreateReactiveScheduledLecture(scheduledLecture);
            if (reactiveLecture != null)
            {
                reactiveScheduledLectures.Add(reactiveLecture);
            }
        }

        return reactiveScheduledLectures;
    }

    /// <summary>
    /// Gets all the scheduled lectures of the given day from the database asynchronously
    /// </summary>
    public async Task<List<ReactiveScheduledLecture>> GetScheduledLecturesByDayAsync(DayOfWeek? day)
    {
        if (day == null) return new List<ReactiveScheduledLecture>();

        string sql = "select * from ScheduledLectures where Day=@Day";

        var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Day = (int)day.Value }).ConfigureAwait(false);

        var reactiveScheduledLectures = new List<ReactiveScheduledLecture>();
        foreach (var scheduledLecture in results)
        {
            var reactiveLecture = CreateReactiveScheduledLecture(scheduledLecture);
            if (reactiveLecture != null)
            {
                reactiveScheduledLectures.Add(reactiveLecture);
            }
        }

        return reactiveScheduledLectures;
    }

    /// <summary>
    /// Gets the scheduled lecture with the given id from the database asynchronously
    /// </summary>
    public async Task<ReactiveScheduledLecture?> GetScheduledLectureByIdAsync(int id)
    {
        string sql = "select * from ScheduledLectures where Id=@Id";
        var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Id = id }).ConfigureAwait(false);
        var scheduledLecture = result.FirstOrDefault();

        return CreateReactiveScheduledLecture(scheduledLecture);
    }

    /// <summary>
    /// Gets the first Scheduled Lecture with the given Subject Name from the database asynchronously
    /// </summary>
    public async Task<ReactiveScheduledLecture?> GetScheduledLectureBySubjectNameAsync(string subjectName)
    {
        string sql = "select * from ScheduledLectures where SubjectName=@SubjectName";
        var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { SubjectName = subjectName }).ConfigureAwait(false);
        var scheduledLecture = result.FirstOrDefault();

        return CreateReactiveScheduledLecture(scheduledLecture);
    }

    /// <summary>
    /// Gets the distinct subject names from the database asynchronously
    /// </summary>
    public async Task<List<string>?> GetDistinctSubjectNames()
    {
        string sql = "select distinct SubjectName from ScheduledLectures";
        return await _dataAccess.LoadData<string, dynamic>(sql, new { }).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the scheduled lectures with distinct subject names
    /// </summary>
    public async Task<List<ReactiveScheduledLecture?>> GetScheduledLecturesGroupedByName()
    {
        string sql = "select * from ScheduledLectures group by SubjectName";
        var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { }).ConfigureAwait(false);

        return result.Select(CreateReactiveScheduledLecture).ToList();
    }

    /// <summary>
    /// Deletes the lecture with the given id if it exists
    /// </summary>
    public async Task DeleteScheduledLectureById(int id)
    {
        string sql = "delete from ScheduledLectures where Id=@Id";
        await _dataAccess.SaveData<dynamic>(sql, new { Id = id });
    }

    private ReactiveScheduledLecture? CreateReactiveScheduledLecture(ScheduledLecture? scheduledLecture)
    {
        if (scheduledLecture == null)
            return null;

        return new ReactiveScheduledLecture
        {
            Id = scheduledLecture!.Id,
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
}
