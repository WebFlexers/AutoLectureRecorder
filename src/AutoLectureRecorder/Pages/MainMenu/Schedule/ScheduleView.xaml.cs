using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using AutoLectureRecorder.Pages.MainMenu.Schedule.CustomControls;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule;

public partial class ScheduleView : ReactiveUserControl<ScheduleViewModel>
{
    private CompositeDisposable _disposables = new();
    
    public ScheduleView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.DataContext = ViewModel;
            
            // Item Source Bindings
            this.OneWayBind(ViewModel,
                    vm => vm.FilteredLecturesByDay[DayOfWeek.Monday],
                    v => v.MondaysLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel,
                    vm => vm.FilteredLecturesByDay[DayOfWeek.Tuesday],
                    v => v.TuesdaysLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel,
                    vm => vm.FilteredLecturesByDay[DayOfWeek.Wednesday],
                    v => v.WednesdaysLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel,
                    vm => vm.FilteredLecturesByDay[DayOfWeek.Thursday],
                    v => v.ThursdaysLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel,
                    vm => vm.FilteredLecturesByDay[DayOfWeek.Friday],
                    v => v.FridaysLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel,
                    vm => vm.FilteredLecturesByDay[DayOfWeek.Saturday],
                    v => v.SaturdaysLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel,
                    vm => vm.FilteredLecturesByDay[DayOfWeek.Sunday],
                    v => v.SundaysLecturesItemsControl.ItemsSource)
                .DisposeWith(disposables);
            
            // Dialogs
            this.Bind(ViewModel, 
                    vm => vm.IsDeleteLecturesConfirmationDialogOpen, 
                    v => v.DeleteConfirmationDialogHost.IsOpen)
                .DisposeWith(disposables);
            
            // CheckBoxes
            this.OneWayBind(ViewModel,
                vm => vm.SelectedLecturesCount,
                v => v.SelectAllLecturesCheckBox.IsChecked,
                selectedCount => selectedCount == ViewModel!.AllLecturesCount);

            SelectAllLecturesCheckBox.Events().Click
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel!.ChangeAllLecturesSelectionCommand)
                .DisposeWith(disposables);
            
            // Commands
            this.BindCommand(ViewModel,
                    vm => vm.NavigateToCreateLecture,
                    v => v.ScheduleLecturesButton)
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.NavigateToCreateLecture,
                    v => v.ScheduleLectureActionButton)
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.AttemptToDeleteLecturesCommand,
                    v => v.DeleteSelectedLecturesButton)
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.DeleteLecturesCommand,
                    v => v.ProceedWithDeletionButton)
                .DisposeWith(disposables);
            
            _disposables.DisposeWith(disposables);
        });
    }

    private void LectureComponent_OnEditClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Lecture lectureComponent) return;

        this.ViewModel!.NavigateToEditLecture
            .Execute(lectureComponent.LectureViewModel.ScheduledLecture)
            .Subscribe()
            .DisposeWith(_disposables);
    }

    private void LectureComponent_OnCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not Lecture lectureComponent) return;

        this.ViewModel!.ModifyLectureStateCommand
            .Execute(lectureComponent.LectureViewModel.ScheduledLecture)
            .Subscribe()
            .DisposeWith(_disposables);
    }

    private void LectureComponent_OnSelectedChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not Lecture lectureComponent) return;

        this.ViewModel!.ChangeSelectedLecturesCountCommand
            .Execute(lectureComponent.LectureViewModel.IsSelected)
            .Subscribe()
            .DisposeWith(_disposables);
    }
}