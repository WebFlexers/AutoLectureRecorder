using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoLectureRecorder.Application.Common.Abstractions.LecturesSchedule;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Pages.MainMenu.CreateLecture;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Dashboard;

public class DashboardViewModel : RoutableViewModel, IActivatableViewModel
{
    public ILecturesScheduler LecturesScheduler { get; private set; }
    private readonly ILogger<DashboardViewModel> _logger;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    public ViewModelActivator Activator { get; }
    private readonly CompositeDisposable _disposables = new();

    private ObservableCollection<ReactiveScheduledLecture>? _todaysLectures;
    public ObservableCollection<ReactiveScheduledLecture>? TodaysLectures
    {
        get => _todaysLectures;
        set => this.RaiseAndSetIfChanged(ref _todaysLectures, value);
    }

    private readonly ObservableAsPropertyHelper<bool> _areLecturesScheduledToday;
    public bool AreLecturesScheduledToday => _areLecturesScheduledToday.Value;
    
    private string? _registrationNumber;
    public string? RegistrationNumber
    {
        get => _registrationNumber; 
        set => this.RaiseAndSetIfChanged(ref _registrationNumber, value);
    }

    public ReactiveCommand<Unit, Unit> NavigateToCreateLectureCommand { get; }

    public DashboardViewModel(ILogger<DashboardViewModel> logger, INavigationService navigationService, 
        IStudentAccountRepository studentAccountRepository, IScheduledLectureRepository scheduledLectureRepository, 
        ILecturesScheduler lecturesScheduler) 
        : base(navigationService)
    {
        LecturesScheduler = lecturesScheduler;
        _logger = logger;
        _scheduledLectureRepository = scheduledLectureRepository;
        Activator = new ViewModelActivator();

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
        {
            NavigationService.Navigate(typeof(CreateLectureViewModel), HostNames.MainMenuHost);
        });

        _areLecturesScheduledToday =
            this.WhenAnyValue(vm => vm.TodaysLectures)
                .Select(lectures => lectures is not null && lectures.Any())
                .ToProperty(this, vm => vm.AreLecturesScheduledToday)
                .DisposeWith(_disposables);

        Observable.FromAsync(async () =>
        {
            var fetchLecturesTask = FetchTodaysLectures();
            var fetchStudentTask = studentAccountRepository.GetStudentAccount();

            await Task.WhenAll(fetchLecturesTask, fetchStudentTask);

            if (fetchLecturesTask.Result is not null)
            {
                TodaysLectures?.Clear();
                TodaysLectures = new ObservableCollection<ReactiveScheduledLecture>(fetchLecturesTask.Result);
            }
            RegistrationNumber = fetchStudentTask.Result?.RegistrationNumber;
        })
        .Catch((Exception e) =>
        {
            _logger.LogError(e, "An error occurred while fetching todays lectures");
            return Observable.Empty<Unit>();
        }).Subscribe()
        .DisposeWith(_disposables);
        
        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }

    private async Task<List<ReactiveScheduledLecture>?> FetchTodaysLectures()
    {
        var todaysLecturesUnsorted = await _scheduledLectureRepository
            .GetScheduledLecturesByDay(DateTime.Now.DayOfWeek);

        var todaysLecturesSortedByStartTime =
            todaysLecturesUnsorted?.OrderBy(lecture => lecture.StartTime);

        // TODO: Create a way to detect whether a lecture was successfully recorded and change the icon accordingly
        return todaysLecturesSortedByStartTime?.ToList();
    }
}
