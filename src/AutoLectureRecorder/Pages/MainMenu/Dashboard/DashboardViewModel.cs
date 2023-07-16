using System;
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
    private readonly IStudentAccountRepository _studentAccountRepository;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    public ViewModelActivator Activator { get; }

    public ObservableCollection<ReactiveScheduledLecture> TodaysLectures { get; private set; } = new();
    

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
        _studentAccountRepository = studentAccountRepository;
        _scheduledLectureRepository = scheduledLectureRepository;
        Activator = new ViewModelActivator();

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
        {
            NavigationService.Navigate(typeof(CreateLectureViewModel), HostNames.MainMenuHost);
        });

        _areLecturesScheduledToday =
            this.WhenAnyValue(vm => vm.TodaysLectures)
                .Select(lectures => lectures.Any())
                .ToProperty(this, vm => vm.AreLecturesScheduledToday);

        this.WhenActivated(disposables =>
        {
            Observable.FromAsync(async () =>
            {
                var fetchLecturesTask = FetchTodaysLectures();
                var fetchStudentTask = _studentAccountRepository.GetStudentAccount();

                await Task.WhenAll(fetchLecturesTask, fetchStudentTask);
                RegistrationNumber = fetchStudentTask.Result?.RegistrationNumber;
            })
            .Catch((Exception e) =>
            {
                _logger.LogError(e, "An error occurred while fetching todays lectures");
                return Observable.Empty<Unit>();
            }).Subscribe()
            .DisposeWith(disposables);
        });
    }

    private async Task FetchTodaysLectures()
    {
        var todaysLecturesUnsorted = await _scheduledLectureRepository
            .GetScheduledLecturesByDay(DateTime.Now.DayOfWeek);

        if (todaysLecturesUnsorted == null || todaysLecturesUnsorted.Any() == false) return;

        var todaysLecturesSortedByStartTime =
            todaysLecturesUnsorted.OrderBy(lecture => lecture.StartTime);

        // TODO: Create a way to detect whether a lecture was successfully recorded and change the icon accordingly
        TodaysLectures = new ObservableCollection<ReactiveScheduledLecture>(todaysLecturesSortedByStartTime);
    }
}
