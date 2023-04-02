using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

public class ScheduledLectureDataTests
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger<ScheduledLectureRepository> _logger;

    public ScheduledLectureDataTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = XUnitLogger.CreateLogger<ScheduledLectureRepository>(output);
    }

    [Fact]
    public async Task GetScheduledLectureById_ShouldFetchTheScheduledLectureWithTheId()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureRepository(dataAccess, _logger);

        var fetchedScheduleLecture = await scheduledLectureData.GetScheduledLectureByIdAsync(1);

        Assert.Equal("Αντικειμενοστρεφής ανάπτυξη εφαρμογών", fetchedScheduleLecture.SubjectName);
        Assert.Equal(4, fetchedScheduleLecture.Semester);
        Assert.Equal("https://teams.microsoft.com", fetchedScheduleLecture.MeetingLink);
        Assert.Equal(1, Convert.ToInt32(fetchedScheduleLecture.Day));
        Assert.Equal("08:15", fetchedScheduleLecture.StartTime?.ToString("HH:mm"));
        Assert.Equal("10:14", fetchedScheduleLecture.EndTime?.ToString("HH:mm"));
        Assert.Equal(1, Convert.ToInt32(fetchedScheduleLecture.IsScheduled));
        Assert.Equal(0, Convert.ToInt32(fetchedScheduleLecture.WillAutoUpload));
    }

    [Fact]
    public async Task GetAllScheduledLecturesSortedAsync_ShouldReturnAListOfSortedLecturesByDayAndStartTime()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureRepository(dataAccess, _logger);

        var results = await scheduledLectureData.GetAllScheduledLecturesSortedAsync();

        foreach (var lecture in results)
        {
            _output.WriteLine($"Day: {lecture.Day} - Start time: {lecture.StartTime}");
        }

        Assert.True(results.Count == 5);

        Assert.True(results[0].Day!.Value == DayOfWeek.Monday);
        Assert.True(results[0].StartTime!.Value.Hour == 8 && results[0].StartTime!.Value.Minute == 15);
        Assert.True(results[1].Day!.Value == DayOfWeek.Monday);
        Assert.True(results[1].StartTime!.Value.Hour == 10 && results[1].StartTime!.Value.Minute == 15);
        Assert.True(results[2].Day!.Value == DayOfWeek.Tuesday);
        Assert.True(results[2].StartTime!.Value.Hour == 6 && results[2].StartTime!.Value.Minute == 15);
        Assert.True(results[3].Day!.Value == DayOfWeek.Tuesday);
        Assert.True(results[3].StartTime!.Value.Hour == 8 && results[3].StartTime!.Value.Minute == 15);
        Assert.True(results[4].Day!.Value == DayOfWeek.Tuesday);
        Assert.True(results[4].StartTime!.Value.Hour == 10 && results[4].StartTime!.Value.Minute == 15);
    }

    [Fact]
    public async Task GetAllScheduledLectures_ShouldReturnAListOfTheScheduledLectures()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureRepository(dataAccess, _logger);

        var allScheduledLectures = await scheduledLectureData.GetAllScheduledLecturesAsync();

        Assert.NotNull(allScheduledLectures);
        Assert.True(allScheduledLectures.Count == 5);
    }

    [Fact]
    public async Task GetScheduledLecturesByDay_ShouldReturnAListOfTheScheduledLecturesInTheDay()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureRepository(dataAccess, _logger);

        var scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDayAsync(DayOfWeek.Tuesday);
        Assert.NotNull(scheduledLectures);
        Assert.True(scheduledLectures.Count == 3);

        scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDayAsync(DayOfWeek.Monday);
        Assert.NotNull(scheduledLectures);
        Assert.True(scheduledLectures.Count == 2);
    }

    [Fact]
    public async Task GetDistinctScheduledLectureSubjectNames_ShouldFetchDistinctSubjectNames()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureRepository(dataAccess, _logger);

        var results = await scheduledLectureData.GetDistinctSubjectNamesAsync();

        Assert.NotNull(results);
        Assert.True(results.Count == 4);
        Assert.Equal("Αντικειμενοστρεφής ανάπτυξη εφαρμογών", results[0]);
        Assert.Equal("Τεχνολογία Λογισμικού", results[1]);
        Assert.Equal("Αλληλεπίδραση Ανθρώπου Υπολογιστή", results[2]);
        Assert.Equal("Πιθανότητες και Στατιστική", results[3]);
    }

    [Fact]
    public async Task GetDistinctScheduledLecturesByName_ShouldFetchAllScheduledLecturesWithDistinctNames()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureRepository(dataAccess, _logger);

        var results = await scheduledLectureData.GetScheduledLecturesGroupedByNameAsync();

        foreach (ReactiveScheduledLecture? lecture in results)
        {
            _output.WriteLine(lecture?.SubjectName);
        }

        Assert.True(results.Count == 4);
        Assert.NotNull(results.FirstOrDefault(lecture => lecture.SubjectName == "Αντικειμενοστρεφής ανάπτυξη εφαρμογών"));
        Assert.NotNull(results.FirstOrDefault(lecture => lecture.SubjectName == "Τεχνολογία Λογισμικού"));
        Assert.NotNull(results.FirstOrDefault(lecture => lecture.SubjectName == "Αλληλεπίδραση Ανθρώπου Υπολογιστή"));
        Assert.NotNull(results.FirstOrDefault(lecture => lecture.SubjectName == "Πιθανότητες και Στατιστική"));
    }

    [Fact]
    public async Task InsertScheduledLecture_ShouldInsertAScheduledLecture()
    {
        var lectureToInsert = CreateSampleReactiveScheduledLecture();

        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureRepository = new ScheduledLectureRepository(dataAccess, _logger);

        var insertedScheduledLecture = await scheduledLectureRepository.InsertScheduledLectureAsync(lectureToInsert);

        Assert.Equal("Ανάλυση 1", insertedScheduledLecture!.SubjectName);
        Assert.Equal(5, insertedScheduledLecture.Semester);
        Assert.Equal("https://teams.microsoft.com", insertedScheduledLecture.MeetingLink);
        Assert.Equal(DayOfWeek.Monday, insertedScheduledLecture.Day);
        Assert.Equal("08:15", insertedScheduledLecture.StartTime!.Value.ToString("HH:mm"));
        Assert.Equal("10:15", insertedScheduledLecture.EndTime!.Value.ToString("HH:mm"));
        Assert.True(insertedScheduledLecture.IsScheduled);
        Assert.False(insertedScheduledLecture.WillAutoUpload);

        // Delete it so it will not affect other tests
        await scheduledLectureRepository.DeleteScheduledLectureByIdAsync(insertedScheduledLecture.Id);
    }

    [Fact]
    public async Task UpdateScheduledLecture_ShouldUpdateTheScheduledLecture()
    {
        var lectureToUpdate = CreateSampleReactiveScheduledLecture();

        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureRepository = new ScheduledLectureRepository(dataAccess, _logger);

        lectureToUpdate.SubjectName = "Updated name";
        lectureToUpdate.MeetingLink = "It's a new link";

        var isSuccessful = await scheduledLectureRepository.UpdateScheduledLectureAsync(lectureToUpdate);

        string sql = "select * from ScheduledLectures where Id=@Id";
        var result = await dataAccess.LoadData<ScheduledLecture, dynamic>(sql, new { Id = lectureToUpdate.Id });

        Assert.True(isSuccessful);
        var updatedLectureInDb = result.First();
        Assert.NotNull(updatedLectureInDb);
        Assert.Equal("Updated name", updatedLectureInDb.SubjectName);
        Assert.Equal("It's a new link", updatedLectureInDb.MeetingLink);
    }

    private static ReactiveScheduledLecture CreateSampleReactiveScheduledLecture()
    {
        return new ReactiveScheduledLecture
        {
            SubjectName = "Ανάλυση 1",
            Semester = 5,
            MeetingLink = "https://teams.microsoft.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };
    }
}
