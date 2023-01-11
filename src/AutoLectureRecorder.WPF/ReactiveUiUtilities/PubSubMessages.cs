namespace AutoLectureRecorder.ReactiveUiUtilities;

public static class PubSubMessages
{
    public static string GetStudentAccount = nameof(GetStudentAccount);
    public static string UpdateLoginErrorMessage = nameof(UpdateLoginErrorMessage);
    public static string CheckClosestScheduledLecture = nameof(CheckClosestScheduledLecture);
    public static string UpdateWindowTopMost = nameof(UpdateWindowTopMost);
    public static string UpdateVideoFullScreen = nameof(UpdateVideoFullScreen);
}
