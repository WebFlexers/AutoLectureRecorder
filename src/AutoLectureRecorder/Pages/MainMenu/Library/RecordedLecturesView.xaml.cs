using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Library;

public partial class RecordedLecturesView : ReactiveUserControl<RecordedLecturesViewModel>
{
    public RecordedLecturesView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            // Item Source bindings
            this.OneWayBind(ViewModel, 
                vm => vm.RecordedLectures, 
                v => v.RecordedLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);

            // Texts
            this.OneWayBind(ViewModel, 
                vm => vm.ScheduledLecture.SubjectName, 
                v => v.SubjectNameTextBox.Text)
                .DisposeWith(disposables);
            
            // Visibilities
            this.OneWayBind(ViewModel,
                vm => vm.HasLectures,
                v => v.NoLecturesTextBlock.Visibility,
                hasLectures => hasLectures ? Visibility.Collapsed : Visibility.Visible);
        });
    }
}
