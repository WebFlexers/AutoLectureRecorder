using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.CreateLecture;

public partial class CreateLectureView : ReactiveUserControl<CreateLectureViewModel>
{
    // In this view we use xaml built in binding instead of the reactive ui one for the most part, because
    // INotifyDataError only works with xaml binding
    public CreateLectureView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            // Observable collection bindings
            this.OneWayBind(ViewModel, 
                    vm => vm.DistinctScheduledLectures, 
                    v => v.SubjectNameComboBox.ItemsSource)
                .DisposeWith(disposables);

            // Combobox behaviour
            SubjectNameComboBox.Events().GotFocus
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    if (ViewModel!.DistinctScheduledLectures is null || ViewModel!.DistinctScheduledLectures.Any() == false)
                    {
                        SubjectNameComboBox.IsDropDownOpen = false;
                        return;
                    }

                    SubjectNameComboBox.IsDropDownOpen = true;
                }).DisposeWith(disposables);

            SubjectNameComboBox.Events().MouseDown
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    if (ViewModel!.DistinctScheduledLectures?.Any() == false)
                    {
                        SubjectNameComboBox.IsDropDownOpen = false;
                        return;
                    }

                    SubjectNameComboBox.IsDropDownOpen = true;
                }).DisposeWith(disposables);

            // Command bindings
            this.BindCommand(ViewModel,
                vm => vm.CreateScheduleLectureCommand,
                v => v.CreateButton)
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                vm => vm.UpdateScheduleLectureCommand,
                v => v.UpdateButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                vm => vm.ProceedAnywayCommand,
                v => v.ProceedInsertionAnywayButton)
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.ProceedAnywayCommand,
                    v => v.ProceedUpdateAnywayButton)
                .DisposeWith(disposables);
            
            // Visibilities
            this.OneWayBind(ViewModel,
                vm => vm.IsOnUpdateMode,
                v => v.ProceedInsertionAnywayButton.Visibility,
                isOnUpdateMode => isOnUpdateMode ? Visibility.Collapsed : Visibility.Visible);
            
            this.OneWayBind(ViewModel,
                vm => vm.IsOnUpdateMode,
                v => v.ProceedUpdateAnywayButton.Visibility,
                isOnUpdateMode => isOnUpdateMode ? Visibility.Visible : Visibility.Collapsed);
            
            this.OneWayBind(ViewModel,
                vm => vm.IsOnUpdateMode,
                v => v.CreateTitleTextBlock.Visibility,
                isOnUpdateMode => isOnUpdateMode ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposables);
            
            this.OneWayBind(ViewModel,
                    vm => vm.IsOnUpdateMode,
                    v => v.CreateButton.Visibility,
                    isOnUpdateMode => isOnUpdateMode ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposables);
            
            this.OneWayBind(ViewModel,
                    vm => vm.IsOnUpdateMode,
                    v => v.UpdateTitleTextBlock.Visibility,
                    isOnUpdateMode => isOnUpdateMode ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.IsOnUpdateMode,
                    v => v.UpdateButton.Visibility,
                    isOnUpdateMode => isOnUpdateMode ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

            // Snackbars
            this.OneWayBind(ViewModel, 
                    vm => vm.IsFailedInsertionSnackbarActive, 
                    v => v.InsertionFailedSnackbar.IsActive)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, 
                    vm => vm.IsSuccessfulInsertionSnackbarActive, 
                    v => v.InsertionSucceededSnackbar.IsActive)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, 
                    vm => vm.IsFailedUpdateSnackbarActive, 
                    v => v.UpdateFailedSnackbar.IsActive)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, 
                    vm => vm.IsSuccessfulUpdateSnackbarActive, 
                    v => v.UpdateSucceededSnackbar.IsActive)
                .DisposeWith(disposables);
            
            // Dialogs
            this.OneWayBind(ViewModel, 
                    vm => vm.ConfirmationDialogContent, 
                    v => v.DialogContentTextBlock.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, 
                    vm => vm.IsConfirmationDialogActive, 
                    v => v.ConfirmationDialogHost.IsOpen)
                .DisposeWith(disposables);
            
            // Time Errors
            this.OneWayBind(ViewModel,
                vm => vm.ValidatableScheduledLecture.IsTimeErrorLecturesOverlap,
                v => v.TimeWarningTextBlock.Visibility,
                isErrorOverlap => isErrorOverlap ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.ValidatableScheduledLecture.TimeError,
                v => v.TimeErrorTextBlock.Visibility,
                errorDescription => string.IsNullOrWhiteSpace(errorDescription) == false && 
                                    ViewModel!.ValidatableScheduledLecture.IsTimeErrorLecturesOverlap == false
                    ? Visibility.Visible 
                    : Visibility.Collapsed)
                .DisposeWith(disposables);
            
            // IsDefault
            this.OneWayBind(ViewModel,
                vm => vm.IsOnUpdateMode,
                v => v.CreateButton.IsDefault,
                isOnUpdateMode => !isOnUpdateMode);
            
            this.OneWayBind(ViewModel,
                vm => vm.IsOnUpdateMode,
                v => v.UpdateButton.IsDefault,
                isOnUpdateMode => isOnUpdateMode);
            
            // IsEnabled
            this.OneWayBind(ViewModel,
                vm => vm.IsOnUpdateMode,
                v => v.CreateButton.IsEnabled,
                isOnUpdateMode => !isOnUpdateMode);
            
            this.OneWayBind(ViewModel,
                vm => vm.IsOnUpdateMode,
                v => v.UpdateButton.IsEnabled,
                isOnUpdateMode => isOnUpdateMode);
        });
        
    }
}
