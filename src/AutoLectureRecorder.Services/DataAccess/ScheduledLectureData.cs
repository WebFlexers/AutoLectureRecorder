using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using System.Runtime.InteropServices;

namespace AutoLectureRecorder.Services.DataAccess;

public class ScheduledLectureData : IScheduledLectureData
{
    private readonly ISqliteDataAccess _dataAccess;

    public ScheduledLectureData(ISqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task<ScheduledLecture?> InsertScheduledLectureAsync(string subjectName, int semester, string meetingLink, DayOfWeek day,
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

        await _dataAccess.SaveData(sql, scheduledLecture);

        sql = "select * from ScheduledLectures where Id = (select last_insert_rowid())";
        var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { });

        return results.FirstOrDefault();
    }

    public async Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesAsync()
    {
        string sql = "select * from ScheduledLectures";

        var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { });

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

    public async Task<List<ReactiveScheduledLecture>> GetScheduledLecturesByDayAsync(DayOfWeek day)
    {
        string sql = "select * from ScheduledLectures where Day=@Day";

        var results = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Day = (int)day });

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

    public async Task<ReactiveScheduledLecture?> GetScheduledLectureByIdAsync(int id)
    {
        string sql = "select * from ScheduledLectures where Id=@Id";
        var result = await _dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Id = id });
        var scheduledLecture = result.FirstOrDefault();

        return CreateReactiveScheduledLecture(scheduledLecture);
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
