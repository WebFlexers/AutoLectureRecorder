namespace AutoLectureRecorder.ReactiveUiUtilities;

public static class PubSubMessages
{
    public static string FillLoginCredentials = nameof(FillLoginCredentials);
    public static string UpdateLoginErrorMessage = nameof(UpdateLoginErrorMessage);
    public static string CheckClosestScheduledLecture = nameof(CheckClosestScheduledLecture);
    public static string DisableConflictingLectures = nameof(DisableConflictingLectures);
    public static string SetUpdateModeToScheduledLecture = nameof(SetUpdateModeToScheduledLecture);
    public static string UpdateWindowTopMost = nameof(UpdateWindowTopMost);
    public static string UpdateVideoFullScreen = nameof(UpdateVideoFullScreen);
}
