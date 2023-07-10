using AutoLectureRecorder.Domain.ReactiveModels;

namespace AutoLectureRecorder.Infrastructure.Common;

public static class ReactiveScheduledLectureExtensions
{
    /// <summary>
    /// Checks if the source lecture's start and end time overlap with the provided lecture 
    /// </summary>
    public static bool OverlapsWithLecture(this ReactiveScheduledLecture thisLecture, 
        ReactiveScheduledLecture lectureToCheck)
    {
        return thisLecture.StartTime <= lectureToCheck.EndTime && lectureToCheck.StartTime <= thisLecture.EndTime;
    }
}