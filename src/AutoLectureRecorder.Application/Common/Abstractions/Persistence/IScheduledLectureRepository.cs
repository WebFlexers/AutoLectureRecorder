using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.Common.Abstractions.Persistence;

public interface IScheduledLectureRepository
{
    /// <summary>
    /// Gets all the scheduled lectures from the database
    /// </summary>
    Task<IEnumerable<ReactiveScheduledLecture>?> GetAllScheduledLectures();

    /// <summary>
    /// Gets all the scheduled lectures sorted by Day first and then by Start Time from the database
    /// </summary>
    Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesOrdered();

    /// <summary>
    /// Gets all the scheduled lectures of the given day from the database
    /// </summary>
    Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesByDay(DayOfWeek? day);
    
    /// <summary>
    /// Gets all the active scheduled lectures of the given day from the database
    /// </summary>
    Task<IEnumerable<ReactiveScheduledLecture>?> GetActiveScheduledLecturesByDay(DayOfWeek? day);

    /// <summary>
    /// Gets all the scheduled lectures of the given day ordered by start time from the database
    /// </summary>
    Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesByDayOrderedByStartTime(DayOfWeek? day);

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
    Task<IEnumerable<string>?> GetDistinctSubjectNames();

    /// <summary>
    /// Gets the scheduled lectures with distinct subject names
    /// </summary>
    Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesGroupedByName();

    /// <summary>
    /// Gets the scheduled lectures grouped by semester
    /// </summary>
    Task<IEnumerable<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedBySemester();

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