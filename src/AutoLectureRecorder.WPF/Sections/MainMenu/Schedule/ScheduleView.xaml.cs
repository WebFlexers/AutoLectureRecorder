using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;

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
}
