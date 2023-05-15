using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess.Interfaces;

public interface IScheduledLectureRepository
{
    /// <summary>
    /// Gets all the scheduled lectures from the database
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetAllScheduledLectures();

    /// <summary>
    /// Gets all the scheduled lectures sorted by Day first and then by Start Time from the database
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedByDayAndStartTime();

    /// <summary>
    /// Gets all the scheduled lectures of the given day from the database
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesByDay(DayOfWeek? day);

    /// <summary>
    /// Gets the scheduled lecture with the given id from the database
    /// </summary>
    Task<ReactiveScheduledLecture?> GetScheduledLectureById(int id);

    /// <summary>
    /// Gets the first Scheduled Lecture with the given Subject Name from the database
    /// </summary>
    Task<ReactiveScheduledLecture?> GetScheduledLectureBySubjectName(string subjectName);

    /// <summary>
    /// Gets the distinct subject names from the database
    /// </summary>
    Task<List<string>?> GetDistinctSubjectNames();

    /// <summary>
    /// Gets the scheduled lectures with distinct subject names
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesGroupedByName();

    /// <summary>
    /// Gets the scheduled lectures grouped by semester
    /// </summary>
    Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedBySemester();

    /// <summary>
    /// Inserts a new Scheduled Lecture in the database and returns it as a ReactiveScheduledLecture
    /// </summary>
    Task<ReactiveScheduledLecture?> InsertScheduledLecture(ReactiveScheduledLecture reactiveLecture);

    /// <summary>
    /// Updates the scheduled lecture using the Id
    /// </summary>
    Task<bool> UpdateScheduledLecture(ReactiveScheduledLecture reactiveLecture);

    /// <summary>
    /// Deletes the lecture with the given id if it exists
    /// </summary>
    Task<bool> DeleteScheduledLectureById(int id);
}