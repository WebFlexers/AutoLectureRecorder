using AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Application.Common;

public static class ReactiveScheduledLectureExtensions
{
    /// <summary>
    /// Checks if the source lecture's start and end time overlap with the provided lecture 
    /// </summary>
    public static bool OverlapsWithLecture(this ReactiveScheduledLecture thisLecture, 
        ReactiveScheduledLecture lectureToCheck)
    {
        return LecturesOverlap(thisLecture.StartTime, thisLecture.EndTime,
            lectureToCheck.StartTime, lectureToCheck.EndTime);
    }
    
    /// <summary>
    /// Checks if the source lecture's start and end time overlap with the provided lecture 
    /// </summary>
    public static bool OverlapsWithLecture(this CreateScheduledLectureCommand thisLecture, 
        ReactiveScheduledLecture lectureToCheck)
    {
        return LecturesOverlap(thisLecture.StartTime, thisLecture.EndTime,
            lectureToCheck.StartTime, lectureToCheck.EndTime);
    }
    
    /// <summary>
    /// Checks if the source lecture's start and end time overlap with the provided lecture 
    /// </summary>
    public static bool OverlapsWithLecture(this IValidatableScheduledLecture thisLecture, 
        ReactiveScheduledLecture lectureToCheck)
    {
        return LecturesOverlap(thisLecture.StartTime, thisLecture.EndTime,
            lectureToCheck.StartTime, lectureToCheck.EndTime);
    }

    private static bool LecturesOverlap(TimeOnly lecture1StartTime, TimeOnly lecture1EndTime,
        TimeOnly lecture2StartTime, TimeOnly lecture2EndTime)
    {
        return lecture1StartTime < lecture2EndTime && lecture2StartTime < lecture1EndTime;
    }
}