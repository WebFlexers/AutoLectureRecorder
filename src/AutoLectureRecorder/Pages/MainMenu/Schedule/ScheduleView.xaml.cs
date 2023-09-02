using System.Reactive.Disposables;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule;

public partial class ScheduleView : ReactiveUserControl<ScheduleViewModel>
{
    public ScheduleView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Commands
            this.BindCommand(ViewModel,
                    vm => vm.NavigateToCreateLecture,
                    v => v.ScheduleLecturesButton)
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.NavigateToCreateLecture,
                    v => v.ScheduleLectureActionButton)
                .DisposeWith(disposables);
        });
    }
}