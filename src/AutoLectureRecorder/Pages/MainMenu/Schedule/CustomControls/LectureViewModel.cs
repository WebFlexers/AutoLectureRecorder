using AutoLectureRecorder.Domain.ReactiveModels;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule.CustomControls;

public class LectureViewModel : ReactiveObject
{
    private ReactiveScheduledLecture _scheduledLecture;
    public ReactiveScheduledLecture ScheduledLecture 
    { 
        get => _scheduledLecture; 
        set => this.RaiseAndSetIfChanged(ref _scheduledLecture, value); 
    }

    private bool _isSelected;
    public bool IsSelected 
    { 
        get => _isSelected; 
        set => this.RaiseAndSetIfChanged(ref _isSelected, value); 
    }

    public LectureViewModel(ReactiveScheduledLecture scheduledLecture, bool isSelected)
    {
        _scheduledLecture = scheduledLecture;
        _isSelected = isSelected;
    }
}