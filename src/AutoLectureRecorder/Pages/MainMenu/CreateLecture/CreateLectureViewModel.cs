using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.CreateScheduledLecture.Mapping;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture.Mapping;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using AutoLectureRecorder.Application.ScheduledLectures.Queries;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Domain.ReactiveModels;
using ErrorOr;
using FluentValidation;
using MediatR;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Pages.MainMenu.CreateLecture;

public class CreateLectureViewModel : RoutableViewModel, INotifyDataErrorInfo, IActivatableViewModel
{
    public bool IgnoreOverlappingLectures { get; set; } = false;
    public ViewModelActivator Activator { get; }

    private bool _isOnUpdateMode = false;
    public bool IsOnUpdateMode
    {
        get => _isOnUpdateMode;
        set => this.RaiseAndSetIfChanged(ref _isOnUpdateMode, value);
    }

    private bool _isFailedInsertionSnackbarActive;
    public bool IsFailedInsertionSnackbarActive 
    { 
        get => _isFailedInsertionSnackbarActive; 
        set => this.RaiseAndSetIfChanged(ref _isFailedInsertionSnackbarActive, value); 
    }

    private bool _isSuccessfulInsertionSnackbarActive;
    public bool IsSuccessfulInsertionSnackbarActive 
    { 
        get => _isSuccessfulInsertionSnackbarActive; 
        set => this.RaiseAndSetIfChanged(ref _isSuccessfulInsertionSnackbarActive, value); 
    }

    private bool _isFailedUpdateSnackbarActive;
    public bool IsFailedUpdateSnackbarActive 
    { 
        get => _isFailedUpdateSnackbarActive; 
        set => this.RaiseAndSetIfChanged(ref _isFailedUpdateSnackbarActive, value); 
    }

    private bool _isSuccessfulUpdateSnackbarActive;
    public bool IsSuccessfulUpdateSnackbarActive 
    { 
        get => _isSuccessfulUpdateSnackbarActive; 
        set => this.RaiseAndSetIfChanged(ref _isSuccessfulUpdateSnackbarActive, value); 
    }

    private bool _isConfirmationDialogActive;
    public bool IsConfirmationDialogActive
    {
        get => _isConfirmationDialogActive;
        set => this.RaiseAndSetIfChanged(ref _isConfirmationDialogActive, value);
    }

    private string? _confirmationDialogContent;
    public string? ConfirmationDialogContent
    {
        get => _confirmationDialogContent;
        set => this.RaiseAndSetIfChanged(ref _confirmationDialogContent, value);
    }
    
    public ObservableCollection<ReactiveScheduledLecture>? DistinctScheduledLectures { get; private set; }
    public ValidatableScheduledLecture ValidatableScheduledLecture { get; set; }
    
    public ReactiveCommand<Unit, Unit> CreateScheduleLectureCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> UpdateScheduleLectureCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ProceedAnywayCommand { get; private set; }

    public bool HasErrors => ValidatableScheduledLecture.HasErrors;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public IEnumerable GetErrors(string? propertyName)
    {
        return ValidatableScheduledLecture.GetErrors(propertyName);
    }
    
    public CreateLectureViewModel(INavigationService navigationService, ISender mediatorSender, 
        IValidator<ValidatableScheduledLecture> validator, IPersistentValidationContext persistentValidationContext) 
        : base(navigationService)
    {
        // TODO: Get parameters that indicate if we are on update mode or create mode
        Activator = new ViewModelActivator();

        ValidatableScheduledLecture = new ValidatableScheduledLecture(validator)
        {
            IsScheduled = true,
            WillAutoUpload = true
        };
        
        CreateScheduleLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var command = ValidatableScheduledLecture.MapToCreateCommand(IgnoreOverlappingLectures);
            persistentValidationContext.AddValidationParameter(command.GetType(), 
                ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures,
                IgnoreOverlappingLectures);
            var result = await mediatorSender.Send(command);
            
            if (result.IsError)
            {
                AddErrors(result.Errors);
                if (IsConfirmationDialogActive == false)
                {
                    ShowFailedCreationSnackbar();
                }
                return;
            }

            ShowSuccessfulCreationSnackbar();
            ClearDayAndTime();
            IgnoreOverlappingLectures = false;
        });
        
        // TODO: Prefill the fields when updating a record 
        UpdateScheduleLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var command = ValidatableScheduledLecture.MapToUpdateCommand(IgnoreOverlappingLectures);
            persistentValidationContext.AddValidationParameter(command.GetType(), 
                ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures,
                IgnoreOverlappingLectures);
            var result = await mediatorSender.Send(command);
            
            if (result.IsError)
            {
                AddErrors(result.Errors);
                if (IsConfirmationDialogActive == false)
                {
                    ShowFailedUpdateSnackbar();
                }
                return;
            }

            ShowSuccessfulUpdateSnackbar();
            IgnoreOverlappingLectures = false;
        });

        ProceedAnywayCommand = ReactiveCommand.Create(() =>
        {
            IgnoreOverlappingLectures = true;
            
            if (IsOnUpdateMode)
            {
                UpdateScheduleLectureCommand.Execute().Subscribe();
            }
            else
            {
                CreateScheduleLectureCommand.Execute().Subscribe();
            }

            IsConfirmationDialogActive = false;
        });

        // TODO: Make it so the semester and meeting link are filled when the user selects or types an existing lecture
        Observable.FromAsync(async () => 
            DistinctScheduledLectures = await mediatorSender.Send(new DistinctScheduledLecturesQuery()));
        
        this.WhenActivated(disposables =>
        {
            ValidatableScheduledLecture.ErrorsChanged += ErrorsViewModelOnErrorsChanged;
            ValidatableScheduledLecture.DisposeWith(disposables);
            
            
            // Hide the snackbars after a set amount of time
            TimeSpan snackBarVisibilityTime = TimeSpan.FromSeconds(4);
            this.WhenAnyValue(vm => vm.IsFailedInsertionSnackbarActive)
                .Throttle(snackBarVisibilityTime)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((isActive) =>
                {
                    if (isActive)
                    {
                        IsFailedInsertionSnackbarActive = false;
                    }
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.IsSuccessfulInsertionSnackbarActive)
                .Throttle(snackBarVisibilityTime)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((isActive) =>
                {
                    if (isActive)
                    {
                        IsSuccessfulInsertionSnackbarActive = false;
                    }
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.IsFailedUpdateSnackbarActive)
                .Throttle(snackBarVisibilityTime)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((isActive) =>
                {
                    if (isActive)
                    {
                        IsFailedUpdateSnackbarActive = false;
                    }
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.IsSuccessfulUpdateSnackbarActive)
                .Throttle(snackBarVisibilityTime)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((isActive) =>
                {
                    if (isActive)
                    {
                        IsSuccessfulUpdateSnackbarActive = false;
                    }
                })
                .DisposeWith(disposables);
        });
    }

    private void AddErrors(List<Error> errors)
    {
        foreach (var error in errors)
        {
            // For time fields we use a different message in the view instead of the standard
            // one that is below each respective field. In particular we have exactly one text block
            // below the start and end time fields that displays errors for both of them
            if (error.Code is 
                nameof(CreateScheduledLectureCommand.StartTime) or 
                nameof(CreateScheduledLectureCommand.EndTime) or 
                nameof(UpdateScheduledLectureCommand.StartTime) or 
                nameof(UpdateScheduledLectureCommand.EndTime))
            {
                ValidatableScheduledLecture.ClearPropertyErrors(error.Code);
                ValidatableScheduledLecture.IsTimeErrorLecturesOverlap = false;
                ValidatableScheduledLecture.TimeError = error.Description;
                ValidatableScheduledLecture.AddError($"{error.Code}ForView", string.Empty);
                
                continue;
            }
            
            // The error.Code is either a specific code like the one below, or a property name
            if (error.Code == "OverlappingLecture")
            {
                ValidatableScheduledLecture.ClearPropertyErrors(nameof(ValidatableScheduledLecture.StartTimeForView));
                ValidatableScheduledLecture.ClearPropertyErrors(nameof(ValidatableScheduledLecture.EndTimeForView));
                    
                ValidatableScheduledLecture.IsTimeErrorLecturesOverlap = true;
                ValidatableScheduledLecture.TimeError = error.Description;

                // We only want to show the confirmation dialog if no error exists other than this one
                if (errors.Count == 1)
                {
                    ConfirmationDialogContent = $"{error.Description}. " +
                                                $"If you add this lecture the conflicting lectures will be deactivated.";
                    IsConfirmationDialogActive = true;
                }

                continue;
            }
            
            ValidatableScheduledLecture.ClearPropertyErrors(error.Code);
            ValidatableScheduledLecture.AddError(error.Code, error.Description);
        }
    }

    private void ErrorsViewModelOnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        ErrorsChanged?.Invoke(this, e);
    }
    
    private void ShowSuccessfulCreationSnackbar()
    {
        IsFailedInsertionSnackbarActive = false;
        IsSuccessfulInsertionSnackbarActive = true;
    }

    private void ShowFailedCreationSnackbar()
    {
        IsFailedInsertionSnackbarActive = true;
        IsSuccessfulInsertionSnackbarActive = false;
    }
    
    private void ShowSuccessfulUpdateSnackbar()
    {
        IsFailedUpdateSnackbarActive = false;
        IsSuccessfulUpdateSnackbarActive = true;
    }

    private void ShowFailedUpdateSnackbar()
    {
        IsFailedUpdateSnackbarActive = true;
        IsSuccessfulUpdateSnackbarActive = false;
    }
    
    private void ClearDayAndTime()
    {
        ValidatableScheduledLecture.Day = null;
        ValidatableScheduledLecture.StartTimeForView = null;
        ValidatableScheduledLecture.EndTimeForView = null;
        
        ValidatableScheduledLecture.TimeError = string.Empty;
    }
}