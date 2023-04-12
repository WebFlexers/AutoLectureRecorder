using System.Reactive;
using System.Windows;
using AutoLectureRecorder.Data.ReactiveModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Sections.MainMenu.RecordLectures;

public class RecordWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new();

    [Reactive]
    public ReactiveScheduledLecture? LectureToRecord { get; set; }

    public ReactiveCommand<Window, Unit> CloseWindowCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; }

    [Reactive]
    public WindowState RecordWindowState { get; set; }

    public void Initialize(ReactiveScheduledLecture scheduledLecture)
    {
        LectureToRecord = scheduledLecture;
        RecordWindowState = WindowState.Maximized;
    }

    public RecordWindowViewModel()
    {
        CloseWindowCommand = ReactiveCommand.Create<Window>(window =>
        {
            window.Close();
        });
        ToggleWindowStateCommand = ReactiveCommand.Create(() =>
        {
            if (RecordWindowState == WindowState.Maximized)
            {
                RecordWindowState = WindowState.Normal;
            }
            else
            {
                RecordWindowState = WindowState.Maximized;
            }
        });
        MinimizeWindowCommand = ReactiveCommand.Create(() => RecordWindowState = WindowState.Minimized);
    }
}
