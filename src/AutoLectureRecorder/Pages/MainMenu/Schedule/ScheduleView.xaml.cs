using System;
using System.Windows;
using System.Windows.Controls;
using AutoLectureRecorder.Domain.ReactiveModels;
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
    
    private void CheckBox_OnCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            var scheduledLecture = (ReactiveScheduledLecture)checkBox.DataContext;
            ViewModel!.UpdateScheduledLectureCommand.Execute(scheduledLecture).Subscribe();
        }
    }
}