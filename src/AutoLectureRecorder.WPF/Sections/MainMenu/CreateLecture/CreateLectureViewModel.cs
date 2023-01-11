using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
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

namespace AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;

public class CreateLectureViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly ILogger<CreateLectureViewModel> _logger;
    private readonly IValidator<ReactiveScheduledLecture> _lectureValidator;
    private readonly IScheduledLectureData _lectureData;

    public ViewModelActivator Activator { get; set; } = new ViewModelActivator();
    public string? UrlPathSegment => nameof(CreateLectureViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<string, Unit> FilterSubjectNamesCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> AutoFillSemesterAndLinkCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> CreateScheduledLectureCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ValidateScheduledLectureCommand { get; private set; }

    // Used to avoid validating empty fields right after successful lecture insertion
    private bool _justSuccessfullyAddedLecture = false;

    // Used to show and hide the Snackbars regarding Scheduled Lectures creation
    [Reactive]
    public bool IsFailedInsertionSnackbarActive { get; set; } = false;
    [Reactive]
    public bool IsSuccessfulInsertionSnackbarActive { get; set; } = false;

    // Used to disable the semester field when an already existing Subject name is selected
    [Reactive]
    public bool IsSemesterEnabled { get; set; } = true;

    // Used to get the unique subject names
    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture> DistinctScheduledLectures { get; private set; }

    [Reactive]
    public ReactiveScheduledLecture ScheduledLecture { get; set; } = new ReactiveScheduledLecture { IsScheduled = true, WillAutoUpload = true };
    [Reactive]
    public ValidatableScheduledLecture ScheduledLectureValidationErrors { get; set; } = new ValidatableScheduledLecture();
    [Reactive]
    public Visibility ValidateErrorsVisibility { get; set; } = Visibility.Hidden;

    public CreateLectureViewModel(ILogger<CreateLectureViewModel> logger, IScreenFactory hostScreen, 
                                  IValidator<ReactiveScheduledLecture> lectureValidator, IScheduledLectureData lectureData)
    {
        HostScreen = hostScreen.GetMainMenuViewModel();
        _logger = logger;
        _lectureValidator = lectureValidator;
        _lectureData = lectureData;

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

            var newLecture = await InsertScheduledLectureToDB();
            _justSuccessfullyAddedLecture = true;

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

            MessageBus.Current.SendMessage<bool>(true, PubSubMessages.CheckClosestScheduledLecture);
        });

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

        this.WhenAnyValue(vm => vm.IsFailedInsertionSnackbarActive)
            .Throttle(TimeSpan.FromSeconds(3))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((isActive) =>
            {
                if (isActive)
                {
                    IsFailedInsertionSnackbarActive = false;
                }
            });

        this.WhenAnyValue(vm => vm.IsSuccessfulInsertionSnackbarActive)
            .Throttle(TimeSpan.FromSeconds(3))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((isActive) =>
            {
                if (isActive)
                {
                    IsSuccessfulInsertionSnackbarActive = false;
                }
            });

        Observable.StartAsync(async () =>
        {
            var distinctNamesTask = _lectureData.GetDistinctScheduledLecturesByName();
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
        if (DistinctScheduledLectures == null)
        {
            return;
        }

        var filteredCollection = DistinctScheduledLectures.Where(lecture => lecture.SubjectName.Contains(containedText));
        DistinctScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>(filteredCollection);
    }

    private async Task<bool> ValidateScheduledLecture()
    {
        var isLectureValid = await ScheduledLectureValidationErrors.ValidateAndPopulateErrors(_lectureValidator, ScheduledLecture);

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

    private Task<ReactiveScheduledLecture?> InsertScheduledLectureToDB()
    {
        var result = _lectureData.InsertScheduledLectureAsync(ScheduledLecture.SubjectName, ScheduledLecture.Semester, 
                                                              ScheduledLecture.MeetingLink, ScheduledLecture.Day, 
                                                              ScheduledLecture.StartTime, ScheduledLecture.EndTime, 
                                                              ScheduledLecture.IsScheduled, ScheduledLecture.WillAutoUpload);

        _logger.LogInformation("Inserted scheduled lecture to database");

        return result;
    }
}
