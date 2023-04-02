using System.Collections.Specialized;
using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Services.DataAccess.Interfaces;

namespace AutoLectureRecorder.Services.DataAccess.Seeding;

public class SampleData
{
    public const int RandomSeed = 323454384;

    private readonly ISqliteDataAccess _dataAccess;
    private readonly Random _random;

    public List<RecordedLecture> RecordedLectures { get; set; } = new();
    public RecordingSettings? RecordingSettings { get; set; }
    public List<ScheduledLecture> ScheduledLectures { get; set; } = new();
    public Statistics? Statistics { get; set; }

    public SampleData(ISqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
        _random = new Random(RandomSeed);

        CreateRecordedLectures();
        CreateRecordingSettings();
        CreateScheduledLectures();
        CreateStatistics();
    }

    /// <summary>
    /// Adds sample data to the SQLite database
    /// </summary>
    public async Task Seed()
    {
        await DeleteEverythingFrom("RecordedLectures");
        await DeleteEverythingFrom("RecordingSettings");
        await DeleteEverythingFrom("ScheduledLectures");
        await DeleteEverythingFrom("Statistics");

        foreach (var recordedLecture in RecordedLectures)
        {
            await InsertRecordedLectureToDb(recordedLecture);
        }

        if (RecordingSettings != null)
        {
            await InsertRecordingSettingsToDb(RecordingSettings);
        }

        foreach (var scheduledLecture in ScheduledLectures)
        {
            await InsertScheduledLectureToDb(scheduledLecture);
        }

        if (Statistics != null)
        {
            await InsertStatisticsToDb(Statistics);
        }
    }

    private void CreateRecordedLectures()
    {

    }

    private void CreateRecordingSettings()
    {

    }

    private static readonly string[] CourseNames =
    {
        "Short Course", 
        "Medium Course Name Length",
        "Lengthy Course Name For Testing More Extreme Cases On User Interface"
    };
    private static readonly Dictionary<string, int> CourseNamesByNumberOfAppearances = new() 
    { 
        { CourseNames[0], 0 }, 
        { CourseNames[1], 0 }, 
        { CourseNames[2], 0 }
    };
    private void CreateScheduledLectures()
    {
        int lectureId = 0;
        for (int semester = 1; semester <= 8; semester++)
        {
            for (int day = 0; day < 7; day++)
            {
                // Pick times starting from 10 am
                var startTime = DateTime.MinValue.AddHours(10).AddMinutes(15);
                var endTime = DateTime.MinValue.AddHours(12).AddMinutes(14);

                for (int lectureNum = 0; lectureNum < 3; lectureNum++)
                {
                    // Pick random name and add indexes at the end of the names according to how many times
                    // a name is used. This is done to simulate different lectures. For example Short Course 1
                    // it a completely different course that Short Course 2
                    var randomNum = _random.Next(3);
                    var randomName = CourseNames[randomNum];
                    CourseNamesByNumberOfAppearances[randomName]++;
                    var subjectName = $"{randomName} {CourseNamesByNumberOfAppearances[randomName]}";

                    var scheduledLecture = new ScheduledLecture
                    {
                        Id = lectureId++,
                        SubjectName = subjectName,
                        Semester = semester,
                        MeetingLink = "https://teams.microsoft.com",
                        Day = day,
                        StartTime = startTime.ToString("HH:mm"),
                        EndTime = endTime.ToString("HH:mm"),
                        IsScheduled = _random.Next(2),
                        WillAutoUpload = _random.Next(2)
                    };

                    ScheduledLectures.Add(scheduledLecture);

                    startTime = startTime.AddHours(2);
                    endTime = endTime.AddHours(2);
                }
            }
        }
    }

    private void CreateStatistics()
    {

    }

    private async Task DeleteEverythingFrom(string tableName)
    {
        string sql = $"delete from {tableName}";
        await _dataAccess.SaveData(sql, new { });
    }

    private async Task InsertRecordedLectureToDb(RecordedLecture recordedLecture)
    {
        string sql = "insert into RecordedLecture (StudentRegistrationNumber, SubjectName, Semester, CloudLink, StartedAt, EndedAt) " +
                     "values (@StudentRegistrationNumber, @SubjectName, @Semester, @CloudLink, @StartedAt, @EndedAt)";

        await _dataAccess.SaveData(sql, recordedLecture).ConfigureAwait(false);
    }

    private async Task InsertRecordingSettingsToDb(RecordingSettings recordingSettings)
    {
        string sql = "insert into RecordingSettings (RecordingsLocalPath, OutputDevice, InputDevice, IsInputDeviceEnabled, Quality, Fps, OutputFrameWidth, OutputFrameHeight) " +
                     "values (@RecordingsLocalPath, @OutputDevice, @InputDevice, @IsInputDeviceEnabled, @Quality, @Fps, @OutputFrameWidth, @OutputFrameHeight)";

        await _dataAccess.SaveData(sql, recordingSettings).ConfigureAwait(false);
    }

    private async Task InsertScheduledLectureToDb(ScheduledLecture scheduledLecture)
    {
        string sql = "insert into ScheduledLectures (Id, SubjectName, Semester, MeetingLink, Day, StartTime, EndTime, IsScheduled, WillAutoUpload) " +
                     "values (@Id, @SubjectName, @Semester, @MeetingLink, @Day, @StartTime, @EndTime, @IsScheduled, @WillAutoUpload)";

        await _dataAccess.SaveData(sql, scheduledLecture).ConfigureAwait(false);
    }

    private async Task InsertStatisticsToDb(Statistics statistics)
    {
        string sql = "insert into Statistics (TotalRecordAttempts, RecordingSucceededNumber, RecordingFailedNumber) " +
                     "values (@TotalRecordAttempts, @RecordingSucceededNumber, @RecordingFailedNumber)";

        await _dataAccess.SaveData(sql, statistics).ConfigureAwait(false);
    }
}
