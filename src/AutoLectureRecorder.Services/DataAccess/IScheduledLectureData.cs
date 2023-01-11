using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;

public interface IScheduledLectureData
{
    /// <summary>
    /// Inserts a new Scheduled Lecture in the database and returns it as a ReactiveScheduledLecture asynchronously
    /// </summary>
    Task<ReactiveScheduledLecture?> InsertScheduledLectureAsync(string subjectName, int semester, string meetingLink, DayOfWeek? day,
                                             DateTime? startTime, DateTime? endTime, bool isScheduled, bool willAutoUpload);
    /// <summary>
    /// Gets all the scheduled lectures from the database asynchronously
    /// </summary>
    Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesAsync();
    /// <summary>
    /// Gets all the scheduled lectures sorted by Day first and then by Start Time from the database asynchronously
    /// </summary>
    Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesSortedAsync();
    /// <summary>
    /// Gets all the scheduled lectures of the given day from the database asynchronously
    /// </summary>
    Task<List<ReactiveScheduledLecture>> GetScheduledLecturesByDayAsync(DayOfWeek? day);
    /// <summary>
    /// Gets the scheduled lecture with the given id from the database asynchronously
    /// </summary>
    Task<ReactiveScheduledLecture?> GetScheduledLectureByIdAsync(int id);
    /// <summary>
    /// Gets the first Scheduled Lecture with the given Subject Name from the database asynchronously
    /// </summary>
    Task<ReactiveScheduledLecture?> GetScheduledLectureBySubjectNameAsync(string subjectName);
    /// <summary>
    /// Gets the distinct subject names from the database asynchronously
    /// </summary>
    Task<List<string>?> GetDistinctSubjectNames();
    /// <summary>
    /// Gets the scheduled lectures with distinct subject names
    /// </summary>
    Task<List<ReactiveScheduledLecture?>> GetScheduledLecturesGroupedByName();
}