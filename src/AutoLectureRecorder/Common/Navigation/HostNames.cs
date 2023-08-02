using AutoLectureRecorder.Pages.MainMenu;
using AutoLectureRecorder.Pages.RecordLecture;

namespace AutoLectureRecorder.Common.Navigation;

public static class HostNames
{
    public static string MainWindowHost => nameof(MainWindowViewModel);
    public static string MainMenuHost => nameof(MainMenuViewModel);
    public static string RecordWindowHost => nameof(RecordWindowViewModel);
}