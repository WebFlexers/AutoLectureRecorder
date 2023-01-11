using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess.Validation.DataAccessMocks;

internal class ScheduledLectureDataMock : IScheduledLectureData
{
    private readonly List<ReactiveScheduledLecture> _lectures;

    public ScheduledLectureDataMock(List<ReactiveScheduledLecture> lectures)
    {
        _lectures = lectures;
    }

    public Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<ReactiveScheduledLecture>?> GetDistinctScheduledLecturesByName()
    {
        throw new NotImplementedException();
    }

    public Task<List<string>?> GetScheduledLecturesGroupedBySubjectNames()
    {
        throw new NotImplementedException();
    }

    public Task<ReactiveScheduledLecture?> GetScheduledLectureByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ReactiveScheduledLecture?> GetScheduledLectureBySubjectNameAsync(string subjectName)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReactiveScheduledLecture>> GetScheduledLecturesByDayAsync(DayOfWeek? day)
    {
        return Task.FromResult(_lectures);
    }

    public Task<ReactiveScheduledLecture?> InsertScheduledLectureAsync(string subjectName, int semester, string meetingLink, DayOfWeek? day, DateTime? startTime, DateTime? endTime, bool isScheduled, bool willAutoUpload)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReactiveScheduledLecture>> GetAllScheduledLecturesSortedAsync()
    {
        throw new NotImplementedException();
    }
}
