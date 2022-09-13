using AutoLectureRecorder.Data.Exceptions;
using AutoLectureRecorder.Data.Models;
using Xunit.Abstractions;

namespace AutoLectureRecorder.Tests.ALR.Data;

public class ScheduleTests
{
    private readonly ITestOutputHelper _output;

    public ScheduleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private Schedule CreateValidSchedule()
    {
        Dictionary<WeekDay, SortedSet<Lecture>> lecturesByDay = new();

        lecturesByDay.Add(WeekDay.Monday, new SortedSet<Lecture>
        {
            new Lecture
            (
                "Quantom physics",
                WeekDay.Monday,
                new TimeSpan(13, 30, 0),
                new TimeSpan(14, 0, 0),
                "Team1"
            ),

            new Lecture
            (
                "Math",
                WeekDay.Monday,
                new TimeSpan(12, 30, 0),
                new TimeSpan(13, 0, 0),
                "Team1"
            ),
        });

        lecturesByDay.Add(WeekDay.Wednesday, new SortedSet<Lecture>
        {
            new Lecture
            (
                "Literature",
                WeekDay.Wednesday,
                new TimeSpan(8, 30, 0),
                new TimeSpan(10, 30, 0),
                "Team1"
            ),

            new Lecture
            (
                "Math",
                WeekDay.Wednesday,
                new TimeSpan(10, 35, 0),
                new TimeSpan(12, 35, 0),
                "Team1"
            ),
        });

        return new Schedule(lecturesByDay);
    }

    [Fact]
    public void GetLecturesByDay_ShouldReturnSortedLectures()
    {
        Schedule schedule = CreateValidSchedule();

        schedule.AddLectureToDay(WeekDay.Saturday, new Lecture
        (
            "Religion",
            WeekDay.Saturday,
            new TimeSpan(10, 35, 0),
            new TimeSpan(12, 35, 0),
            "Team1"
        ));

        var lectures = schedule.GetLecturesByDay(WeekDay.Monday);
        foreach (var lecture in lectures)
        {
            _output.WriteLine("*-----------------------------------------------*");
            _output.WriteLine(
                    $"\nName: {lecture.Name}" +
                    $"\nDay: {lecture.Day}" +
                    $"\nStart Time {lecture.StartTime}" +
                    $"\nEnd Time {lecture.EndTime}"
                );
        }

        var lectures2 = schedule.GetLecturesByDay(WeekDay.Wednesday);
        foreach (var lecture in lectures2)
        {
            _output.WriteLine("*-----------------------------------------------*");
            _output.WriteLine(
                    $"\nName: {lecture.Name}" +
                    $"\nDay: {lecture.Day}" +
                    $"\nStart Time {lecture.StartTime}" +
                    $"\nEnd Time {lecture.EndTime}"
                );
        }

        var lectures3 = schedule.GetLecturesByDay(WeekDay.Sunday);
        Assert.Null(lectures3);
    }

    [Theory]
    [InlineData(13, 40, 15, 40,
                14, 40, 16, 40)]

    [InlineData(14, 40, 16, 40,
                13, 40, 15, 40)]

    [InlineData(14, 40, 15, 40,
                13, 40, 16, 40)]

    [InlineData(13, 40, 16, 40,
                14, 40, 15, 40)]

    [InlineData(13, 40, 15, 40,
                15, 40, 17, 40)]
    public void AddLectureToDay_ShouldThrow(int startTime1Hours, int startTime1Minutes, int endTime1Hours, int endTime1Minutes,
                                            int startTime2Hours, int startTime2Minutes, int endTime2Hours, int endTime2Minutes)
    {
        Schedule schedule = CreateValidSchedule();

        schedule.AddLectureToDay(WeekDay.Saturday, new Lecture
        (
            "Religion",
            WeekDay.Saturday,
            new TimeSpan(startTime1Hours, startTime1Minutes, 0),
            new TimeSpan(endTime1Hours, endTime1Minutes, 0),
            "Team1"
        ));

        Assert.Throws<TimesOverlapException>(() =>
        {
            schedule.AddLectureToDay(WeekDay.Saturday, new Lecture
            (
                "Math",
                WeekDay.Saturday,
                new TimeSpan(startTime2Hours, startTime2Minutes, 0),
                new TimeSpan(endTime2Hours, endTime2Minutes, 0),
                "Team1"
            ));
        });
    }
}
