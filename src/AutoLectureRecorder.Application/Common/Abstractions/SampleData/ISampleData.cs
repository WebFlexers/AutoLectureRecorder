using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Application.Common.Abstractions.SampleData;

public interface ISampleData
{
    List<ScheduledLecture> ScheduledLectures { get; set; }
    RecordingSettings? RecordingSettings { get; set; }
    GeneralSettings? GeneralSettings { get; set; }
    Statistics? Statistics { get; set; }

    /// <summary>
    /// Adds sample data to the SQLite database
    /// </summary>
    Task Seed();
}