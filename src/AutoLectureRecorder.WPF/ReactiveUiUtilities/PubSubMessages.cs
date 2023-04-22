namespace AutoLectureRecorder.ReactiveUiUtilities;

public static class PubSubMessages
{
    public static string FillLoginCredentials = nameof(FillLoginCredentials);
    public static string UpdateLoginErrorMessage = nameof(UpdateLoginErrorMessage);
    public static string CheckClosestScheduledLecture = nameof(CheckClosestScheduledLecture);
    public static string SetUpdateModeToScheduledLecture = nameof(SetUpdateModeToScheduledLecture);
    public static string UpdateVideoFullScreen = nameof(UpdateVideoFullScreen);
    public static string UpdateTheme = nameof(UpdateTheme);
}
