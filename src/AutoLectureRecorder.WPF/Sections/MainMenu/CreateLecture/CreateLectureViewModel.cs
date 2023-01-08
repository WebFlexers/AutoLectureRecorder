using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;

public class CreateLectureViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ILogger<CreateLectureViewModel> _logger;
    private readonly IValidator<ReactiveScheduledLecture> _lectureValidator;
    private readonly IScheduledLectureData _lectureData;

    public string? UrlPathSegment => nameof(CreateLectureViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> CreateScheduledLectureCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ValidateScheduledLectureCommand { get; private set; }

    public CreateLectureViewModel(ILogger<CreateLectureViewModel> logger, IScreenFactory hostScreen, 
                                  IValidator<ReactiveScheduledLecture> lectureValidator, IScheduledLectureData lectureData)
    {
        HostScreen = hostScreen.GetMainMenuViewModel();
        _logger = logger;
        _lectureValidator = lectureValidator;
        _lectureData = lectureData;

        ValidateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ValidateScheduledLecture();
        });

        CreateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            ValidateErrorsVisibility = Visibility.Visible;

            var isLectureValid = await ValidateScheduledLecture();

            if (isLectureValid == false)
            {
                IsFailedInsertionSnackbarActive = true;
                return;
            }

            await InsertScheduledLectureToDB();
            _justSuccessfullyAddedLecture = true;

            ClearDayAndTimeFields();

            IsSuccessfulInsertionSnackbarActive = true;
        });

        this.WhenAnyValue(vm => vm.ScheduledLecture.SubjectName, vm => vm.ScheduledLecture.Semester,
                          vm => vm.ScheduledLecture.MeetingLink, vm => vm.ScheduledLecture.Day,
                          vm => vm.ScheduledLecture.StartTime, vm => vm.ScheduledLecture.EndTime)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((_) =>
            {
                if (_justSuccessfullyAddedLecture)
                {
                    _justSuccessfullyAddedLecture = false;
                    return;
                }

                ValidateScheduledLectureCommand.Execute().Subscribe();
            });

        this.WhenAnyValue(vm => vm.IsFailedInsertionSnackbarActive)
            .Throttle(TimeSpan.FromSeconds(2))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((isActive) =>
            {
                if (isActive)
                {
                    IsFailedInsertionSnackbarActive = false;
                }
            });

        this.WhenAnyValue(vm => vm.IsSuccessfulInsertionSnackbarActive)
            .Throttle(TimeSpan.FromSeconds(2))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe((isActive) =>
            {
                if (isActive)
                {
                    IsSuccessfulInsertionSnackbarActive = false;
                }
            });
    }

    // Used to avoid validating empty fields right after successful lecture insertion
    private bool _justSuccessfullyAddedLecture = false;

    private void ClearDayAndTimeFields()
    {
        ScheduledLecture.Day = null;
        ScheduledLecture.StartTime = null;
        ScheduledLecture.EndTime = null;

        ScheduledLectureValidationErrors.DayError = string.Empty;
        ScheduledLectureValidationErrors.TimeError = string.Empty;
    }

    [Reactive]
    public ReactiveScheduledLecture ScheduledLecture { get; set; } = new ReactiveScheduledLecture();

    [Reactive]
    public ValidatableScheduledLecture ScheduledLectureValidationErrors { get; set; } = new ValidatableScheduledLecture();

    [Reactive]
    public bool IsFailedInsertionSnackbarActive { get; set; } = false;

    [Reactive]
    public bool IsSuccessfulInsertionSnackbarActive { get; set; } = false;

    [Reactive]
    public Visibility ValidateErrorsVisibility { get; set; } = Visibility.Hidden;

    private async Task<bool> ValidateScheduledLecture()
    {
        if (ValidateErrorsVisibility == Visibility.Hidden)
        {
            return false;
        }

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

    private Task InsertScheduledLectureToDB()
    {
        var result = _lectureData.InsertScheduledLectureAsync(ScheduledLecture.SubjectName, ScheduledLecture.Semester, 
                                                              ScheduledLecture.MeetingLink, ScheduledLecture.Day, 
                                                              ScheduledLecture.StartTime, ScheduledLecture.EndTime, 
                                                              ScheduledLecture.IsScheduled, ScheduledLecture.WillAutoUpload);

        _logger.LogInformation("Inserted scheduled lecture to database");

        return result;
    }
}
