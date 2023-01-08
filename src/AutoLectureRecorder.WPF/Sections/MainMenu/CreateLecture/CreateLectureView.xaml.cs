using ReactiveUI;
using System.Reactive.Disposables;
using MaterialDesignThemes.Wpf;
using System;
using System.Reactive.Linq;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;

public partial class CreateLectureView : ReactiveUserControl<CreateLectureViewModel>
{
    public CreateLectureView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Fields values bindings
            this.Bind(ViewModel, vm => vm.ScheduledLecture.SubjectName, v => v.subjectNameComboBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.Semester, v => v.semesterComboBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.MeetingLink, v => v.meetingLinkTextBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.Day, v => v.dayComboBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.StartTime, v => v.startTimePicker.SelectedTime)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.EndTime, v => v.endTimePicker.SelectedTime)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.IsScheduled, v => v.isScheduledToggleButton.IsChecked)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.WillAutoUpload, v => v.autoUploadToggleButton.IsChecked)
                .DisposeWith(disposables);

            // Fields validation bindings
            this.OneWayBind(ViewModel, vm => vm.ScheduledLectureValidationErrors.SubjectNameError, v => v.subjectNameErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ScheduledLectureValidationErrors.SemesterError, v => v.semesterErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ScheduledLectureValidationErrors.MeetingLinkError, v => v.meetingLinkErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ScheduledLectureValidationErrors.DayError, v => v.dayErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ScheduledLectureValidationErrors.TimeError, v => v.startTimeErrorTextBlock.Text)
                .DisposeWith(disposables);

            // Fields validation errors visibility
            this.OneWayBind(ViewModel, vm => vm.ValidateErrorsVisibility, v => v.subjectNameErrorTextBlock.Visibility)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ValidateErrorsVisibility, v => v.semesterErrorTextBlock.Visibility)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ValidateErrorsVisibility, v => v.meetingLinkErrorTextBlock.Visibility)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ValidateErrorsVisibility, v => v.dayErrorTextBlock.Visibility)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ValidateErrorsVisibility, v => v.startTimeErrorTextBlock.Visibility)
                .DisposeWith(disposables);

            // Snackbars
            this.OneWayBind(ViewModel, vm => vm.IsFailedInsertionSnackbarActive, v => v.insertionFailedSnackbar.IsActive)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsSuccessfulInsertionSnackbarActive, v => v.insertionSucceededSnackbar.IsActive)
                .DisposeWith(disposables);

            // Commands
            this.BindCommand(ViewModel, vm => vm.CreateScheduledLectureCommand, v => v.submitButton);
        });
    }
}
