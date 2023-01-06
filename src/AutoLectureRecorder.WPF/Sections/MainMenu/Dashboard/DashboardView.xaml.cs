using ReactiveUI;
using System.Reactive.Disposables;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, vm => vm.NavigateToScheduleCommand, v => v.registrationNumTextBlock, nameof(registrationNumTextBlock.MouseDown))
                .DisposeWith(disposables);
        });
    }
}
