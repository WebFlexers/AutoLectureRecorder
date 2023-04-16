using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveMarbles.ObservableEvents;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public partial class ScheduleView : ReactiveUserControl<ScheduleViewModel>
{
    private readonly CompositeDisposable _externalDisposables = new();

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

            disposables.Add(_externalDisposables);
        });
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
                    ViewModel!.LoadNextScheduledLecturesCommand?
                        .Execute(DayOfWeek.Monday)
                        .Subscribe().DisposeWith(_externalDisposables);
                }
                downArrowPackIcon = MondayDownArrowPackIcon;
                break;
            case nameof(TuesdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand?
                        .Execute(DayOfWeek.Tuesday)
                        .Subscribe().DisposeWith(_externalDisposables);
                }
                downArrowPackIcon = TuesdayDownArrowPackIcon;
                break;
            case nameof(WednesdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand?
                        .Execute(DayOfWeek.Wednesday)
                        .Subscribe().DisposeWith(_externalDisposables);
                }
                downArrowPackIcon = WednesdayDownArrowPackIcon;
                break;
            case nameof(ThursdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand?
                        .Execute(DayOfWeek.Thursday)
                        .Subscribe().DisposeWith(_externalDisposables);
                }
                downArrowPackIcon = ThursdayDownArrowPackIcon;
                break;
            case nameof(FridaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand?
                        .Execute(DayOfWeek.Friday)
                        .Subscribe().DisposeWith(_externalDisposables);
                }
                downArrowPackIcon = FridayDownArrowPackIcon;
                break;
            case nameof(SaturdaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand?
                        .Execute(DayOfWeek.Saturday)
                        .Subscribe().DisposeWith(_externalDisposables);
                }
                downArrowPackIcon = SaturdayDownArrowPackIcon;
                break;
            case nameof(SundaysLecturesScrollViewer):
                if (endOfScrollViewerReached)
                {
                    ViewModel!.LoadNextScheduledLecturesCommand?
                        .Execute(DayOfWeek.Sunday)
                        .Subscribe().DisposeWith(_externalDisposables);
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
}
