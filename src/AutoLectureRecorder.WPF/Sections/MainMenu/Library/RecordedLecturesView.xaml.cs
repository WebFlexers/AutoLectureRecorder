using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public partial class RecordedLecturesView : ReactiveUserControl<RecordedLecturesViewModel>
{
    public RecordedLecturesView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;
        });
    }
}
