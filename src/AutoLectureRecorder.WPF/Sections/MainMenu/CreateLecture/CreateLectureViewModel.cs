using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Services.DataAccess;
using FluentValidation;
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

public class CreateLectureViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly ILogger<CreateLectureViewModel> _logger;
    private readonly IValidationFactory _validationFactory;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    public ViewModelActivator Activator { get; set; } = new();
    public string UrlPathSegment => nameof(CreateLectureViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<string, Unit> FilterSubjectNamesCommand { get; }
    public ReactiveCommand<Unit, Unit> AutoFillSemesterAndLinkCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateScheduledLectureCommand { get; }
    public ReactiveCommand<Unit, Unit> ValidateScheduledLectureCommand { get; }

    // Used to avoid validating empty fields right after successful lecture insertion
    private bool _justSuccessfullyAddedLecture;

    // Used to show and hide the Snackbars regarding Scheduled Lectures creation
    [Reactive]
    public bool IsFailedInsertionSnackbarActive { get; set; }
    [Reactive]
    public bool IsSuccessfulInsertionSnackbarActive { get; set; }

    // Used to disable the semester field when an already existing Subject name is selected
    [Reactive]
    public bool IsSemesterEnabled { get; set; } = true;

    // Used to get the unique subject names
    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture> DistinctScheduledLectures { get; private set; } = new();

    [Reactive]
    public ReactiveScheduledLecture ScheduledLecture { get; set; } = new() { IsScheduled = true, WillAutoUpload = true };
    [Reactive]
    public ValidatableScheduledLecture ScheduledLectureValidationErrors { get; set; } = new();
    [Reactive]
    public Visibility ValidateErrorsVisibility { get; set; } = Visibility.Collapsed;

    public CreateLectureViewModel(ILogger<CreateLectureViewModel> logger, IScreenFactory hostScreen,
                                  IValidationFactory validationFactory, IScheduledLectureRepository scheduledLectureRepository)
    {
        HostScreen = hostScreen.GetMainMenuViewModel();
        _logger = logger;
        _validationFactory = validationFactory;
        _scheduledLectureRepository = scheduledLectureRepository;

        FilterSubjectNamesCommand = ReactiveCommand.Create<string>(FilterSubjectNames);

        AutoFillSemesterAndLinkCommand = ReactiveCommand.Create(() =>
        {
            var existingLecture = DistinctScheduledLectures?.FirstOrDefault(
                    lecture => lecture.SubjectName.Equals(ScheduledLecture.SubjectName)
                );

            if (existingLecture != null)
            {
                ScheduledLecture.Semester = existingLecture.Semester;
                IsSemesterEnabled = false;
                ScheduledLecture.MeetingLink = existingLecture.MeetingLink;
            }
            else
            {
                IsSemesterEnabled = true;
            }
        });

        ValidateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ValidateScheduledLecture();
        });

        CreateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var isLectureValid = await ValidateScheduledLecture();

            if (isLectureValid == false)
            {
                IsSuccessfulInsertionSnackbarActive = false;
                IsFailedInsertionSnackbarActive = true;
                ValidateErrorsVisibility = Visibility.Visible;
                return;
            }

            var newLecture = await InsertScheduledLectureToDb();
            _justSuccessfullyAddedLecture = true;

            // If the new lecture has a subject name that didn't exist previously, add it to the list
            if (DistinctScheduledLectures!.Any(
                lecture => lecture.SubjectName.Equals(ScheduledLecture.SubjectName)) == false
            )
            {
                DistinctScheduledLectures!.Add(newLecture!);
            }

            ClearDayAndTimeFields();

            IsFailedInsertionSnackbarActive = false;
            IsSuccessfulInsertionSnackbarActive = true;
            IsSemesterEnabled = false;

            // Send message to recalculate the closest scheduled lecture to now,
            // in case the new lecture is closer
            MessageBus.Current.SendMessage(true, PubSubMessages.CheckClosestScheduledLecture);
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
        TimeSpan snackBarVisibilityTime = TimeSpan.FromSeconds(3);
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

        // Get the Scheduled Lectures grouped by name
        Observable.StartAsync(async () =>
        {
            var distinctNamesTask = _scheduledLectureRepository.GetScheduledLecturesGroupedByName();
            var distinctNames = await distinctNamesTask;

            if (distinctNames != null)
            {
                DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>(distinctNames);
            }
            else
            {
                DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>();
            }

            distinctNamesTask.Dispose();
        });
    }

    private void ClearDayAndTimeFields()
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
    private void FilterSubjectNames(string containedText)
    {
        var filteredCollection = DistinctScheduledLectures.Where(lecture => lecture.SubjectName.Contains(containedText));
        DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>(filteredCollection);
    }

    private async Task<bool> ValidateScheduledLecture()
    {
        var existingLectures = await _scheduledLectureRepository.GetScheduledLecturesByDayAsync(ScheduledLecture.Day);
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

    private Task<ReactiveScheduledLecture?> InsertScheduledLectureToDb()
    {
        var result = _scheduledLectureRepository.InsertScheduledLectureAsync(
            ScheduledLecture.SubjectName, ScheduledLecture.Semester,
            ScheduledLecture.MeetingLink, ScheduledLecture.Day,
            ScheduledLecture.StartTime, ScheduledLecture.EndTime,
            ScheduledLecture.IsScheduled, ScheduledLecture.WillAutoUpload);

        _logger.LogInformation("Inserted scheduled lecture to database");

        return result;
    }
}
