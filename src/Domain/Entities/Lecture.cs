using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities;

public class Lecture : BaseAuditableEntity
{
    #region Properties

    public string Name { get; set; }
    public string? MeetingTeamName { get; set; }
    public string? MeetingTeamLink { get; set; }
    public bool IsActive { get; set; }
    public bool IsAutoUploadActive { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    #endregion

    /// <summary>
    /// Creates a lecture object that has a meeting team, but no meeting link
    /// </summary>
    public static Lecture CreateWithMeetingTeam(string name, string meetingTeamName, TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek,
                                                bool isActive = true, bool isAutoUploadActive = false)
    {
        return new Lecture(name, meetingTeamName, startTime, endTime, dayOfWeek, usesTeamLink: false, isActive, isAutoUploadActive);
    }

    /// <summary>
    /// Creates a lecture object that has a meeting link, but no meeting team
    /// </summary>
    /// <returns></returns>
    public static Lecture CreateWithLink(string name, string meetingLink, TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek,
                                                bool isActive, bool isAutoUploadActive)
    {
        return new Lecture(name, meetingLink, startTime, endTime, dayOfWeek, usesTeamLink: true, isActive, isAutoUploadActive);
    }

    private Lecture(string name, string meetingTeamOrLink, TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek, bool usesTeamLink,
                    bool isActive, bool isAutoUploadActive) 
    {
        if (startTime <= endTime)
        {
            throw new InvalidTimeException("Invalid time. End time can't be less or equal to start time!");
        }

        if (usesTeamLink)
        {
            MeetingTeamLink = meetingTeamOrLink;
        }
        else
        {
            MeetingTeamName = meetingTeamOrLink;
        }

        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        DayOfWeek = dayOfWeek;
        IsActive = isActive;
        IsAutoUploadActive = isAutoUploadActive;

        Created = DateTime.Now;
        LastModified = Created;
    }
}
