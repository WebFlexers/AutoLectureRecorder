namespace AutoLectureRecorder.Application.ScheduledLectures.Common;

public static class ValidationParameters
{
    public static class ScheduledLectures
    {
        public const string IgnoreOverlappingLectures = nameof(IgnoreOverlappingLectures);
        public const string IsOnUpdateMode = nameof(IsOnUpdateMode);
        public const string ScheduledLectureId = nameof(ScheduledLectureId);
    }
}