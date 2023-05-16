using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess.Validation.DataAccessMocks;

internal class ScheduledLectureDataMock : IScheduledLectureRepository
{
    private readonly List<ReactiveScheduledLecture>? _lectures;

    public ScheduledLectureDataMock(List<ReactiveScheduledLecture>? lectures)
    {
        _lectures = lectures;
    }

    public Task<List<ReactiveScheduledLecture>?> GetAllScheduledLectures()
    {
        throw new NotImplementedException();
    }

    public Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesGroupedByName()
    {
        throw new NotImplementedException();
    }

    public Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedBySemester()
    {
        throw new NotImplementedException();
    }

    public Task<ReactiveScheduledLecture?> InsertScheduledLecture(ReactiveScheduledLecture lecture)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateScheduledLecture(ReactiveScheduledLecture lecture)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteScheduledLectureById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>?> GetDistinctSubjectNames()
    {
        throw new NotImplementedException();
    }

    public Task<ReactiveScheduledLecture?> GetScheduledLectureById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ReactiveScheduledLecture?> GetScheduledLectureBySubjectName(string subjectName)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesByDay(DayOfWeek? day)
    {
        return Task.FromResult(_lectures);
    }

    public Task<ReactiveScheduledLecture?> InsertScheduledLectureAsync(string subjectName, int semester, string meetingLink, DayOfWeek? day, DateTime? startTime, DateTime? endTime, bool isScheduled, bool willAutoUpload)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReactiveScheduledLecture>?> GetScheduledLecturesOrderedByDayAndStartTime()
    {
        throw new NotImplementedException();
    }
}
