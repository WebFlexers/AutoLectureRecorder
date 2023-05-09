using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess.Interfaces;

public interface IScheduledLectureRepository
{
    /// <summary>
    /// Gets all the scheduled lectures from the database asynchronously
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetAllScheduledLecturesAsync();

    /// <summary>
    /// Gets all the scheduled lectures sorted by Day first and then by Start Time from the database asynchronously
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetAllScheduledLecturesSortedAsync();

    /// <summary>
    /// Gets all the scheduled lectures of the given day from the database asynchronously
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesByDayAsync(DayOfWeek? day);

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
    Task<List<string>?> GetDistinctSubjectNamesAsync();

    /// <summary>
    /// Gets the scheduled lectures with distinct subject names
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesGroupedByNameAsync();

    /// <summary>
    /// Gets the scheduled lectures grouped by semester
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedBySemesterAsync();

    /// <summary>
    /// Inserts a new Scheduled Lecture in the database and returns it as a ReactiveScheduledLecture asynchronously
    /// </summary>
    Task<ReactiveScheduledLecture?> InsertScheduledLectureAsync(ReactiveScheduledLecture reactiveLecture);

    /// <summary>
    /// Updates the scheduled lecture using the Id
    /// </summary>
    Task<bool> UpdateScheduledLectureAsync(ReactiveScheduledLecture reactiveLecture);

    /// <summary>
    /// Deletes the lecture with the given id if it exists
    /// </summary>
    Task<bool> DeleteScheduledLectureByIdAsync(int id);
}