using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public partial class ScheduleView : ReactiveUserControl<ScheduleViewModel>
{
    public ScheduleView()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            this.OneWayBind(ViewModel, vm => vm.VisibleScheduledLecturesByDay[DayOfWeek.Monday], 
                v => v.MondaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VisibleScheduledLecturesByDay[DayOfWeek.Tuesday], 
                v => v.TuesdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VisibleScheduledLecturesByDay[DayOfWeek.Wednesday], 
                v => v.WednesdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VisibleScheduledLecturesByDay[DayOfWeek.Thursday], 
                v => v.ThursdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VisibleScheduledLecturesByDay[DayOfWeek.Friday], 
                v => v.FridaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VisibleScheduledLecturesByDay[DayOfWeek.Saturday], 
                v => v.SaturdaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.VisibleScheduledLecturesByDay[DayOfWeek.Sunday], 
                v => v.SundaysLecturesItemsControl.ItemsSource).DisposeWith(disposables);
        });
    }

    private void ScheduledLectureComponent_OnClick(object sender, RoutedEventArgs e)
    {
        var lectureComponent = (ScheduledLectureComponent)sender;
        ViewModel!.NavigateToCreateLectureCommand.Execute(lectureComponent.Lecture)
            .Subscribe();
    }

    private void LecturesScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer) return;

        var downArrowPackIcon = default(PackIcon);

        bool endOfScrollViewerReached = scrollViewer.VerticalOffset.Equals(scrollViewer.ScrollableHeight);

        switch (scrollViewer.Name)
        {
            case nameof(MondaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand
                        .Execute(DayOfWeek.Monday).Subscribe();
                }
                downArrowPackIcon = MondayDownArrowPackIcon;
                break;
            case nameof(TuesdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand
                        .Execute(DayOfWeek.Tuesday).Subscribe();
                }
                downArrowPackIcon = TuesdayDownArrowPackIcon;
                break;
            case nameof(WednesdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand
                        .Execute(DayOfWeek.Wednesday).Subscribe();
                }
                downArrowPackIcon = WednesdayDownArrowPackIcon;
                break;
            case nameof(ThursdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand
                        .Execute(DayOfWeek.Thursday).Subscribe();
                }
                downArrowPackIcon = ThursdayDownArrowPackIcon;
                break;
            case nameof(FridaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand
                        .Execute(DayOfWeek.Friday).Subscribe();
                }
                downArrowPackIcon = FridayDownArrowPackIcon;
                break;
            case nameof(SaturdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand
                        .Execute(DayOfWeek.Saturday).Subscribe();
                }
                downArrowPackIcon = SaturdayDownArrowPackIcon;
                break;
            case nameof(SundaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand
                        .Execute(DayOfWeek.Sunday).Subscribe();
                }
                downArrowPackIcon = SundayDownArrowPackIcon;
                break;
        }

        if (downArrowPackIcon == null) return;

        if (endOfScrollViewerReached)
        {
            downArrowPackIcon.Visibility = Visibility.Hidden;
        }
        else
        {
            downArrowPackIcon.Visibility = Visibility.Visible;
        }
    }

    private void LectureComponent_OnCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not ScheduledLectureComponent lectureComponent) return;

        ViewModel!.UpdateScheduledLectureCommand
            .Execute(lectureComponent.Lecture).Subscribe();
    }
}
