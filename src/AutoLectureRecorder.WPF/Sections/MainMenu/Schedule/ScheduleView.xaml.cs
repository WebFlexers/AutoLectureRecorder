using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public partial class ScheduleView : ReactiveUserControl<ScheduleViewModel>
{
    public ScheduleView()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecturesByDay[DayOfWeek.Monday], 
                v => v.MondaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecturesByDay[DayOfWeek.Tuesday], 
                v => v.TuesdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecturesByDay[DayOfWeek.Wednesday], 
                v => v.WednesdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecturesByDay[DayOfWeek.Thursday], 
                v => v.ThursdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecturesByDay[DayOfWeek.Friday], 
                v => v.FridaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecturesByDay[DayOfWeek.Saturday], 
                v => v.SaturdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecturesByDay[DayOfWeek.Sunday], 
                v => v.SundaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);
        });
    }

    private void ScheduledLectureComponent_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add a parameter with the ReactiveScheduledLecture in the Command
        ViewModel!.NavigateToCreateLectureCommand.Execute().Subscribe();
        //ViewModel!.Lecture = new ReactiveScheduledLecture
        //{
        //    SubjectName = "Jack",
        //    StartTime = DateTime.Now,
        //    EndTime = DateTime.Now,
        //    IsScheduled = true,
        //    WillAutoUpload = false
        //};
    }

    private void LecturesScrollViewers_OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer) return;

        var downArrowPackIcon = default(PackIcon);

        switch (scrollViewer.Name)
        {
            case nameof(MondaysLecturesScrollViewer):
                downArrowPackIcon = MondayDownArrowPackIcon;
                break;
            case nameof(TuesdaysLecturesScrollViewer):
                downArrowPackIcon = TuesdayDownArrowPackIcon;
                break;
            case nameof(WednesdaysLecturesScrollViewer):
                downArrowPackIcon = WednesdayDownArrowPackIcon;
                break;
            case nameof(ThursdaysLecturesScrollViewer):
                downArrowPackIcon = ThursdayDownArrowPackIcon;
                break;
            case nameof(FridaysLecturesScrollViewer):
                downArrowPackIcon = FridayDownArrowPackIcon;
                break;
            case nameof(SaturdaysLecturesScrollViewer):
                downArrowPackIcon = SaturdayDownArrowPackIcon;
                break;
            case nameof(SundaysLecturesScrollViewer):
                downArrowPackIcon = SundayDownArrowPackIcon;
                break;
        }

        if (downArrowPackIcon == null) return;

        if (scrollViewer.VerticalOffset.Equals(scrollViewer.ScrollableHeight))
        {
            downArrowPackIcon.Visibility = Visibility.Hidden;
        }
        else
        {
            downArrowPackIcon.Visibility = Visibility.Visible;
        }
    }
}
