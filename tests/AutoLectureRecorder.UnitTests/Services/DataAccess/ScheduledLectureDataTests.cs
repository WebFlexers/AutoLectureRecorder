using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

public class ScheduledLectureDataTests
{
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

        Assert.Equal("Αντικειμενοστρεφής ανάπτυξη εφαρμογών", insertedScheduledLecture.SubjectName);
        Assert.Equal(5, insertedScheduledLecture.Semester);
        Assert.Equal("https://www.test.com", insertedScheduledLecture.MeetingLink);
        Assert.Equal(1, insertedScheduledLecture.Day);
        Assert.Equal("08:15", insertedScheduledLecture.StartTime);
        Assert.Equal("10:15", insertedScheduledLecture.EndTime);
        Assert.Equal(1, insertedScheduledLecture.IsScheduled);
        Assert.Equal(0, insertedScheduledLecture.WillAutoUpload);
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
        Assert.Single(scheduledLectures);

        scheduledLectures = await scheduledLectureData.GetScheduledLecturesByDayAsync(DayOfWeek.Monday);
        Assert.Single(scheduledLectures);
    }
}
