using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule;

public partial class ScheduleView : ReactiveUserControl<ScheduleViewModel>
{
    public ScheduleView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;
        });
    }
    
    private void ActiveCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        
    }

    private void ActiveCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
    {
        
    }
}