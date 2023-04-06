using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.Sections.MainMenu.CreateLecture;

// TODO: Give the user the ability to add an overlapping lecture, but warn him.
// TODO: If they add it as activated any overlapping lectures must be deactivated
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
    public ReactiveCommand<Unit, Unit> ValidateScheduledLectureCommand { get; }

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

    // Used to get the unique subject names
    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture> DistinctScheduledLectures { get; private set; } = new();

    [Reactive]
    public ReactiveScheduledLecture ScheduledLecture { get; set; } = new() { IsScheduled = true, WillAutoUpload = true };
    [Reactive]
    public ValidatableScheduledLecture ScheduledLectureValidationErrors { get; set; } = new();
    [Reactive]
    public Visibility ValidateErrorsVisibility { get; set; } = Visibility.Collapsed;

    [Reactive] 
    public bool IsUpdateModeSelected { get; set; } = false;

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

            if (existingLecture != null)
            {
                ScheduledLecture.Semester = existingLecture.Semester;
                ScheduledLecture.MeetingLink = existingLecture.MeetingLink;
            }
        });

        ValidateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ValidateScheduledLecture();
        });

        CreateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsConfirmationDialogActive = true;

            if (await ValidateLectureAndUpdateUI() == false) return;

            var newLecture = await _scheduledLectureRepository.InsertScheduledLectureAsync(ScheduledLecture);

            if (newLecture == null)
            {
                IsFailedInsertionSnackbarActive = true;
                IsSuccessfulInsertionSnackbarActive = false;
                return;
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

            IsFailedInsertionSnackbarActive = false;
            IsSuccessfulInsertionSnackbarActive = true;

            // Send message to recalculate the closest scheduled lecture to now,
            // in case the new lecture is closer
            MessageBus.Current.SendMessage(true, PubSubMessages.CheckClosestScheduledLecture);
        });

        UpdateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (await ValidateLectureAndUpdateUI() == false) return;

            var result = await _scheduledLectureRepository.UpdateScheduledLectureAsync(ScheduledLecture);

            if (result == false)
            {
                IsFailedUpdateSnackbarActive = true;
                IsSuccessfulUpdateSnackbarActive = false;
                return;
            }

            IsFailedUpdateSnackbarActive = false;
            IsSuccessfulUpdateSnackbarActive = true;

            // Send message to recalculate the closest scheduled lecture to now,
            // in case the newly updated lecture is closer
            MessageBus.Current.SendMessage(true, PubSubMessages.CheckClosestScheduledLecture);
        });

        // Update autocomplete
        this.WhenAnyValue(vm => vm.ScheduledLecture.SubjectName)
            .Throttle(TimeSpan.FromMilliseconds(800))
            .Subscribe(async searchText =>
            {
                await FilterSubjectNames(searchText);
            });

        // Validate all the fields whenever any validatable field inside Scheduled Lecture changes
        this.WhenAnyValue(vm => vm.ScheduledLecture.SubjectName, vm => vm.ScheduledLecture.Semester,
               vm => vm.ScheduledLecture.MeetingLink, vm => vm.ScheduledLecture.Day,
               vm => vm.ScheduledLecture.StartTime, vm => vm.ScheduledLecture.EndTime)
             .Throttle(TimeSpan.FromMilliseconds(100))
             .ObserveOn(RxApp.MainThreadScheduler)
             .Subscribe((props) =>
             {
                 if (_justSuccessfullyAddedLecture)
                 {
                     _justSuccessfullyAddedLecture = false;
                     return;
                 }

                 ValidateScheduledLectureCommand.Execute().Subscribe();
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

    private async Task PopulateLecturesWithDistinctSubjectNames()
    {
        var lecturesWithDistinctNames = await _scheduledLectureRepository.GetScheduledLecturesGroupedByNameAsync();

        if (lecturesWithDistinctNames != null && lecturesWithDistinctNames.Any())
        {
            DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>(lecturesWithDistinctNames);
        }
        else
        {
            DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>();
        }
    }

    /// <summary>
    /// Validates the lecture and shows the error snackbar
    /// if it fails.
    /// </summary>
    /// <returns>True if the validation was successful and false otherwise</returns>
    private async Task<bool> ValidateLectureAndUpdateUI()
    {
        var isLectureValid = await ValidateScheduledLecture();

        if (string.IsNullOrWhiteSpace(ScheduledLectureValidationErrors.TimeWarning) == false)
        {

        }

        if (isLectureValid == false)
        {
            IsSuccessfulInsertionSnackbarActive = false;
            IsFailedInsertionSnackbarActive = true;
            ValidateErrorsVisibility = Visibility.Visible;
            return false;
        }

        return true;
    }

    private void ClearDayAndTime()
    {
        ScheduledLecture.Day = null;
        ScheduledLecture.StartTime = null;
        ScheduledLecture.EndTime = null;

        ScheduledLectureValidationErrors.DayError = string.Empty;
        ScheduledLectureValidationErrors.TimeError = string.Empty;
    }

    /// <summary>
    /// Filters the distinct scheduled lectures list by a search term
    /// </summary>
    /// <param name="containedText">The search term</param>
    private async Task FilterSubjectNames(string? containedText)
    {
        if (containedText == null) return;

        var lecturesWithDistinctNames = await _scheduledLectureRepository.GetScheduledLecturesGroupedByNameAsync();

        if (lecturesWithDistinctNames == null || lecturesWithDistinctNames.Any() == false) return;

        DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>();

        foreach (var lecture in lecturesWithDistinctNames)
        {
            if (lecture.SubjectName.Contains(containedText))
            {
                DistinctScheduledLectures.Add(lecture);
            }
        }
    }

    private async Task<bool> ValidateScheduledLecture()
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
        }
        else
        {
            _logger.LogDebug("Validated scheduled lecture. There is one or more invalid fields");
        }

        return isLectureValid;
    }
}
