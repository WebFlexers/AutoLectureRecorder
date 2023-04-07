using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AutoLectureRecorder.Sections.MainMenu.CreateLecture;

public class CreateLectureViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly ILogger<CreateLectureViewModel> _logger;
    private readonly IValidationFactory _validationFactory;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    public ViewModelActivator Activator { get; set; } = new();
    public string UrlPathSegment => nameof(CreateLectureViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> AutoFillSemesterAndLinkCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateScheduledLectureCommand { get; }
    public ReactiveCommand<Unit, Unit> UpdateScheduledLectureCommand { get; }
    public ReactiveCommand<Unit, Unit> ConfirmSubmitCommand { get; }

    // Used to avoid validating empty fields right after successful lecture insertion
    private bool _justSuccessfullyAddedLecture;

    // Used to show and hide the Snackbars regarding Scheduled Lectures creation and update
    [Reactive]
    public bool IsFailedInsertionSnackbarActive { get; set; }
    [Reactive]
    public bool IsSuccessfulInsertionSnackbarActive { get; set; }
    [Reactive]
    public bool IsFailedUpdateSnackbarActive { get; set; }
    [Reactive]
    public bool IsSuccessfulUpdateSnackbarActive { get; set; }
    [Reactive]
    public bool IsConfirmationDialogActive { get; set; }

    [Reactive]
    public string ConfirmationDialogContent { get; set; }

    // Used to get the unique subject names
    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture> DistinctScheduledLectures { get; private set; } = new();

    [Reactive]
    public ReactiveScheduledLecture ScheduledLecture { get; set; } = new() { IsScheduled = true, WillAutoUpload = true };
    [Reactive]
    public ValidatableScheduledLecture ScheduledLectureValidationErrors { get; set; } = new();

    [Reactive] 
    public bool IsUpdateModeSelected { get; set; } = false;

    private bool _firstTimeSubmit = true;

    public CreateLectureViewModel(ILogger<CreateLectureViewModel> logger, IScreenFactory hostScreen,
                                  IValidationFactory validationFactory, IScheduledLectureRepository scheduledLectureRepository)
    {
        HostScreen = hostScreen.GetMainMenuViewModel();
        _logger = logger;
        _validationFactory = validationFactory;
        _scheduledLectureRepository = scheduledLectureRepository;

        AutoFillSemesterAndLinkCommand = ReactiveCommand.Create(() =>
        {
            var existingLecture = DistinctScheduledLectures?.FirstOrDefault(
                    lecture => lecture.SubjectName.Equals(ScheduledLecture.SubjectName)
                );

            if (existingLecture == null) return;

            ScheduledLecture.Semester = existingLecture.Semester;
            ScheduledLecture.MeetingLink = existingLecture.MeetingLink;
        });

        CreateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_firstTimeSubmit)
            {
                WatchAndValidateFields();
                _firstTimeSubmit = false;
            }

            if (await ValidateLectureAndUpdateUI(false, true) == false) return;

            if (await CreateScheduledLecture() == false) return;

            ShowSuccessfulCreationSnackbar();
        });

        UpdateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_firstTimeSubmit)
            {
                WatchAndValidateFields();
                _firstTimeSubmit = false;
            }

            if (await ValidateLectureAndUpdateUI(false, true) == false) return;

            await UpdateScheduledLecture();

            ShowSuccessfulUpdateSnackbar();
        });

        ConfirmSubmitCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (IsUpdateModeSelected)
            {
                if (await ValidateLectureAndUpdateUI(true, true) == false) return;
                if (await DisableConflictingLectures() == false) return;
                await UpdateScheduledLecture();
                ShowSuccessfulUpdateSnackbar();
            }
            else
            {
                if (await ValidateLectureAndUpdateUI(true, true) == false) return;
                if (await DisableConflictingLectures() == false) return;
                await CreateScheduledLecture();
                ShowSuccessfulCreationSnackbar();
            }

            IsConfirmationDialogActive = false;
        });

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
            });

        this.WhenAnyValue(vm => vm.IsSuccessfulInsertionSnackbarActive)
            .Throttle(snackBarVisibilityTime)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((isActive) =>
            {
                if (isActive)
                {
                    IsSuccessfulInsertionSnackbarActive = false;
                }
            });

        this.WhenAnyValue(vm => vm.IsFailedUpdateSnackbarActive)
            .Throttle(snackBarVisibilityTime)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((isActive) =>
            {
                if (isActive)
                {
                    IsFailedUpdateSnackbarActive = false;
                }
            });

        this.WhenAnyValue(vm => vm.IsSuccessfulUpdateSnackbarActive)
            .Throttle(snackBarVisibilityTime)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((isActive) =>
            {
                if (isActive)
                {
                    IsSuccessfulUpdateSnackbarActive = false;
                }
            });

        // Get the Scheduled Lectures grouped by name, in order prevent adding
        // lectures that conflict with the existing ones
        var getLecturesTask = PopulateLecturesWithDistinctSubjectNames();

        MessageBus.Current.Listen<ReactiveScheduledLecture>(PubSubMessages.SetUpdateModeToScheduledLecture)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async lecture =>
            {
                await getLecturesTask;
               
                IsUpdateModeSelected = true;
                ScheduledLecture = lecture;
            });
    }

    private void WatchAndValidateFields()
    {
        // Start validating all the fields whenever any validatable field inside Scheduled Lecture changes
        this.WhenAnyValue(vm => vm.ScheduledLecture.SubjectName, vm => vm.ScheduledLecture.Semester,
                vm => vm.ScheduledLecture.MeetingLink, vm => vm.ScheduledLecture.Day,
                vm => vm.ScheduledLecture.StartTime, vm => vm.ScheduledLecture.EndTime)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async _ =>
            {
                if (_justSuccessfullyAddedLecture)
                {
                    _justSuccessfullyAddedLecture = false;
                    return;
                }

                await ValidateLectureAndUpdateUI(false, false);
            });
    }

    private async Task<bool> CreateScheduledLecture()
    {
        var newLecture = await _scheduledLectureRepository.InsertScheduledLectureAsync(ScheduledLecture);

        if (newLecture == null)
        {
            ShowFailedCreationSnackbar();
            return false;
        }

        _justSuccessfullyAddedLecture = true;

        // If the new lecture has a subject name that didn't exist previously, add it to the list
        if (DistinctScheduledLectures.Any(
                lecture => lecture.SubjectName.Equals(ScheduledLecture.SubjectName)) == false
           )
        {
            DistinctScheduledLectures.Add(newLecture!);
        }

        ClearDayAndTime();

        // Send message to recalculate the closest scheduled lecture to now,
        // in case the new lecture is closer
        MessageBus.Current.SendMessage(true, PubSubMessages.CheckClosestScheduledLecture);

        return true;
    }

    private async Task UpdateScheduledLecture()
    {
        var result = await _scheduledLectureRepository.UpdateScheduledLectureAsync(ScheduledLecture);

        if (result == false)
        {
            ShowFailedUpdateSnackbar();
            return;
        }

        // Send message to recalculate the closest scheduled lecture to now,
        // in case the newly updated lecture is closer
        MessageBus.Current.SendMessage(true, PubSubMessages.CheckClosestScheduledLecture);
    }

    private async Task<bool> DisableConflictingLectures()
    {
        if (ScheduledLecture.IsScheduled == false) return true;

        var dayLectures = await _scheduledLectureRepository.GetScheduledLecturesByDayAsync(ScheduledLecture.Day);

        if (dayLectures == null || dayLectures.Any() == false) return false;

        var updateLecturesTasks = new List<Task>();
        foreach (var lecture in dayLectures)
        {
            if (ReactiveScheduledLecture.AreLecturesOverlapping(ScheduledLecture, lecture))
            {
                lecture.IsScheduled = false;
                updateLecturesTasks.Add(_scheduledLectureRepository.UpdateScheduledLectureAsync(lecture));
            }
        }

        await Task.WhenAll(updateLecturesTasks);

        return true;
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

    private async Task PopulateLecturesWithDistinctSubjectNames()
    {
        var lecturesWithDistinctNames = await _scheduledLectureRepository.GetScheduledLecturesGroupedByNameAsync();

        Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            if (lecturesWithDistinctNames != null && lecturesWithDistinctNames.Any())
            {
                DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>(lecturesWithDistinctNames);
            }
            else
            {
                DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>();
            }
        });
    }

    /// <summary>
    /// Validates the lecture and shows the error snackbar
    /// if it fails.
    /// </summary>
    /// <returns>True if the validation was successful and false otherwise</returns>
    private async Task<bool> ValidateLectureAndUpdateUI(bool ignoreWarnings, bool showSnackbarOnError)
    {
        var existingLectures = await _scheduledLectureRepository.GetScheduledLecturesByDayAsync(ScheduledLecture.Day);

        if (existingLectures != null && IsUpdateModeSelected)
        {
            // Remove the lecture to be updated, because since it will be updated
            // we don't want to count it's start time and end time in the validation
            var lectureToRemove = existingLectures.FirstOrDefault(lecture => lecture.Id == ScheduledLecture.Id);
            if (lectureToRemove != null)
            {
                existingLectures.Remove(lectureToRemove);
            }
        }

        var scheduledLectureValidator = _validationFactory.CreateReactiveScheduledLectureValidator(existingLectures);
        var isLectureValid = await ScheduledLectureValidationErrors.ValidateAndPopulateErrors(scheduledLectureValidator, ScheduledLecture);

        if (isLectureValid)
        {
            _logger.LogDebug("Validated scheduled lecture. The fields are ALL valid");
            return true;
        }

        _logger.LogDebug("Scheduled lecture failed to pass validation. There is one or more invalid fields");

        if (ScheduledLectureValidationErrors is { HasWarnings: true, HasErrors: false })
        {
            ConfirmationDialogContent = $"{ScheduledLectureValidationErrors.TimeWarning}. " +
                                        "If you add this lecture the conflicting lectures will be deactivated.";

            if (showSnackbarOnError)
            {
                IsConfirmationDialogActive = true;
            }

            if (ignoreWarnings)
            {
                ScheduledLectureValidationErrors.HasWarnings = false;
            }
            return ignoreWarnings;
        }

        if (showSnackbarOnError)
        {
            IsSuccessfulInsertionSnackbarActive = false;
            IsFailedInsertionSnackbarActive = true;
        }

        return false;

    }

    private void ClearDayAndTime()
    {
        ScheduledLecture.Day = null;
        ScheduledLecture.StartTime = null;
        ScheduledLecture.EndTime = null;

        ScheduledLectureValidationErrors.DayError = string.Empty;
        ScheduledLectureValidationErrors.TimeError = string.Empty;
    }
}
