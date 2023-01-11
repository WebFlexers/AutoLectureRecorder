using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;
public interface IScheduledLectureData
{
    Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesAsync();
    Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesSortedAsync();
    Task<List<ReactiveScheduledLecture?>> GetDistinctScheduledLecturesByName();
    Task<List<string>?> GetScheduledLecturesGroupedBySubjectNames();
    Task<ReactiveScheduledLecture?> GetScheduledLectureByIdAsync(int id);
    Task<ReactiveScheduledLecture?> GetScheduledLectureBySubjectNameAsync(string subjectName);
    Task<List<ReactiveScheduledLecture>> GetScheduledLecturesByDayAsync(DayOfWeek? day);
    Task<ReactiveScheduledLecture?> InsertScheduledLectureAsync(string subjectName, int semester, string meetingLink, DayOfWeek? day, DateTime? startTime, DateTime? endTime, bool isScheduled, bool willAutoUpload);
}