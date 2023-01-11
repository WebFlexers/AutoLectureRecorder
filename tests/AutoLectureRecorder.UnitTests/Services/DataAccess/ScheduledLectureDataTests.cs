using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

public class ScheduledLectureDataTests
{
    private readonly ITestOutputHelper _output;

    public ScheduledLectureDataTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task InsertScheduledLecture_ShouldInsertAScheduledLecture()
    {
        var scheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };

        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureData(dataAccess);

        var insertedScheduledLecture = await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester, 
                                                                                         scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                                                         scheduledLecture.StartTime, scheduledLecture.EndTime, 
                                                                                         scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        Assert.Equal("Αντικειμενοστρεφής ανάπτυξη εφαρμογών", insertedScheduledLecture!.SubjectName);
        Assert.Equal(5, insertedScheduledLecture.Semester);
        Assert.Equal("https://www.test.com", insertedScheduledLecture.MeetingLink);
        Assert.Equal(DayOfWeek.Monday, insertedScheduledLecture.Day);
        Assert.Equal("08:15", insertedScheduledLecture.StartTime!.Value.ToString("HH:mm"));
        Assert.Equal("10:15", insertedScheduledLecture.EndTime!.Value.ToString("HH:mm"));
        Assert.True(insertedScheduledLecture.IsScheduled);
        Assert.False(insertedScheduledLecture.WillAutoUpload);
    }

    [Fact]
    public async Task GetScheduledLectureById_ShouldFetchTheScheduledLectureWithTheId()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());

        var scheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };

        var scheduledLectureData = new ScheduledLectureData(dataAccess);

        var insertedScheduledLecture = await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                                         scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                                                         scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                                         scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        var fetchedScheduleLecture = await scheduledLectureData.GetScheduledLectureByIdAsync(insertedScheduledLecture.Id);

        Assert.NotNull(fetchedScheduleLecture);

        Assert.Equal("Αντικειμενοστρεφής ανάπτυξη εφαρμογών", fetchedScheduleLecture.SubjectName);
        Assert.Equal(5, fetchedScheduleLecture.Semester);
        Assert.Equal("https://www.test.com", fetchedScheduleLecture.MeetingLink);
        Assert.Equal(1, Convert.ToInt32(fetchedScheduleLecture.Day));
        Assert.Equal("08:15", fetchedScheduleLecture.StartTime?.ToString("HH:mm"));
        Assert.Equal("10:15", fetchedScheduleLecture.EndTime?.ToString("HH:mm"));
        Assert.Equal(1, Convert.ToInt32(fetchedScheduleLecture.IsScheduled));
        Assert.Equal(0, Convert.ToInt32(fetchedScheduleLecture.WillAutoUpload));
    }

    [Fact]
    public async Task GetAllScheduledLecturesSortedAsync_ShouldReturnAListOfSortedLecturesByDayAndStartTime()
    {
        var scheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };

        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureData(dataAccess);

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                               scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                               scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                               scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        scheduledLecture.StartTime = default(DateTime).AddHours(7).AddMinutes(15);
        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                       scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                       scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                       scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        scheduledLecture.StartTime = default(DateTime).AddHours(10).AddMinutes(30);
        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                       scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                       scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                       scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        scheduledLecture.Day = DayOfWeek.Tuesday;
        scheduledLecture.StartTime = default(DateTime).AddHours(9).AddMinutes(15);

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                            scheduledLecture.MeetingLink, scheduledLecture.Day,
                                            scheduledLecture.StartTime, scheduledLecture.EndTime,
                                            scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        scheduledLecture.Day = DayOfWeek.Tuesday;
        scheduledLecture.StartTime = default(DateTime).AddHours(8).AddMinutes(15);

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                    scheduledLecture.MeetingLink, scheduledLecture.Day,
                                    scheduledLecture.StartTime, scheduledLecture.EndTime,
                                    scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        var results = await scheduledLectureData.GetAllScheduledLecturesSortedAsync();

        foreach (var lecture in results)
        {
            _output.WriteLine($"Day: {lecture.Day} - Start time: {lecture.StartTime}");
        }

        Assert.True(results[0].Day!.Value == DayOfWeek.Monday);
        Assert.True(results[0].StartTime!.Value.Hour == 7 && results[0].StartTime!.Value.Minute == 15);
        Assert.True(results[1].Day!.Value == DayOfWeek.Monday);
        Assert.True(results[1].StartTime!.Value.Hour == 8 && results[1].StartTime!.Value.Minute == 15);
        Assert.True(results[2].Day!.Value == DayOfWeek.Monday);
        Assert.True(results[2].StartTime!.Value.Hour == 10 && results[2].StartTime!.Value.Minute == 30);
        Assert.True(results[3].Day!.Value == DayOfWeek.Tuesday);
        Assert.True(results[3].StartTime!.Value.Hour == 8 && results[3].StartTime!.Value.Minute == 15);
        Assert.True(results[4].Day!.Value == DayOfWeek.Tuesday);
        Assert.True(results[4].StartTime!.Value.Hour == 9 && results[4].StartTime!.Value.Minute == 15);
    }

    [Fact]
    public async Task GetAllScheduledLectures_ShouldReturnAListOfTheScheduledLectures()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());

        var scheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };

        var scheduledLectureData = new ScheduledLectureData(dataAccess);

        for (int i = 0; i < 2; i++)
        {
            await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                                         scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                                                         scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                                         scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);
        }
        
        var allScheduledLectures = await scheduledLectureData.GetAllScheduledLecturesAsync();

        Assert.True(allScheduledLectures.Count > 1);
    }

    [Fact]
    public async Task GetScheduledLecturesByDay_ShouldReturnAListOfTheScheduledLecturesInTheDay()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());

        var scheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };

        var scheduledLectureData = new ScheduledLectureData(dataAccess);

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                                     scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                                                     scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                                     scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                             scheduledLecture.MeetingLink, DayOfWeek.Friday,
                                                                             scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                             scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        var scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDayAsync(DayOfWeek.Friday);
        Assert.NotEmpty(scheduledLectures);

        scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDayAsync(DayOfWeek.Monday);
        Assert.NotEmpty(scheduledLectures);
    }

    [Fact]
    public async Task GetDistinctScheduledLectureSubjectNames_ShouldFetchDistinctSubjectNames()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureData(dataAccess);

        var scheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                             scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                                             scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                             scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                             scheduledLecture.MeetingLink, DayOfWeek.Friday,
                                                                             scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                             scheduledLecture.IsScheduled, true);

        string anotherSubjectName = "Another Subject";
        await scheduledLectureData.InsertScheduledLectureAsync(anotherSubjectName, scheduledLecture.Semester,
                                                                     scheduledLecture.MeetingLink, DayOfWeek.Friday,
                                                                     scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                     scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        var result = await scheduledLectureData.GetScheduledLecturesGroupedBySubjectNames();

        Assert.NotNull(result);
        Assert.True(result.Count() == 2);
        Assert.Equal(scheduledLecture.SubjectName, result[0]);
        Assert.Equal(anotherSubjectName, result[1]);
    }

    [Fact]
    public async Task GetDistinctScheduledLecturesByName_ShouldFetchAllScheduledLecturesWithDistinctNames()
    {


        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var scheduledLectureData = new ScheduledLectureData(dataAccess);

        var scheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                             scheduledLecture.MeetingLink, scheduledLecture.Day,
                                                                             scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                             scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        await scheduledLectureData.InsertScheduledLectureAsync(scheduledLecture.SubjectName, scheduledLecture.Semester,
                                                                             scheduledLecture.MeetingLink, DayOfWeek.Friday,
                                                                             scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                             scheduledLecture.IsScheduled, true);

        string anotherSubjectName = "Another Subject";
        await scheduledLectureData.InsertScheduledLectureAsync(anotherSubjectName, scheduledLecture.Semester,
                                                                     scheduledLecture.MeetingLink, DayOfWeek.Friday,
                                                                     scheduledLecture.StartTime, scheduledLecture.EndTime,
                                                                     scheduledLecture.IsScheduled, scheduledLecture.WillAutoUpload);

        var results = await scheduledLectureData.GetDistinctScheduledLecturesByName();

        foreach (ReactiveScheduledLecture lecture in results)
        {
            _output.WriteLine(lecture.SubjectName);
        }

        Assert.NotNull(results);
        Assert.True(results.Count() == 2);
        Assert.Equal(scheduledLecture.SubjectName, results[1].SubjectName);
        Assert.Equal(scheduledLecture.Semester, results[0].Semester);
    }
}
