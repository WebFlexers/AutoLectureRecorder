using System.Collections;

namespace AutoLectureRecorder.Data.Models;

public class Lecture : IComparable<Lecture>
{
    public string Name { get; set; }
    public WeekDay Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string MeetingTeam { get; set; }
    public bool IsLectureActive { get; set; }
    public bool IsAutoUploadActive { get; set; }

    private void ValidateTime()
    {
        if (TimeSpan.Compare(StartTime, EndTime) > -1)
        {
            throw new InvalidDataException("The start time must always be lesser that the end time in a lecture!");
        }
    }

    public Lecture(string name, WeekDay day, TimeSpan startTime, TimeSpan endTime, string meetingTeam)
    {
        Name = name;
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
        MeetingTeam = meetingTeam;
        ValidateTime();
    }

    public Lecture(string name, WeekDay day, TimeSpan startTime, TimeSpan endTime, string meetingTeam, bool isLectureActive, bool isAutoUploadActive)
    {
        Name = name;
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
        MeetingTeam = meetingTeam;
        IsLectureActive = isLectureActive;
        IsAutoUploadActive = isAutoUploadActive;
        ValidateTime();
    }

    public int CompareTo(Lecture otherLecture)
    {
        return TimeSpan.Compare(this.StartTime, otherLecture.StartTime);
    }
}
