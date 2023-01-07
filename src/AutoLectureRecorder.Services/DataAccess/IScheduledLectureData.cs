using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;
public interface IScheduledLectureData
{
    Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesAsync();
    Task<ReactiveScheduledLecture?> GetScheduledLectureByIdAsync(int id);
    Task<List<ReactiveScheduledLecture>> GetScheduledLecturesByDayAsync(DayOfWeek day);
    Task<ScheduledLecture?> InsertScheduledLectureAsync(string subjectName, int semester, string meetingLink, DayOfWeek day, DateTime? startTime, DateTime? endTime, bool isScheduled, bool willAutoUpload);
}