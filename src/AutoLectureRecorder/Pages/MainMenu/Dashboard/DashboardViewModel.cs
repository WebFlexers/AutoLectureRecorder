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
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
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

    private ObservableCollection<ReactiveScheduledLecture> _todaysLectures = new();
    public ObservableCollection<ReactiveScheduledLecture> TodaysLectures
    {
        get => _todaysLectures;
        set => this.RaiseAndSetIfChanged(ref _todaysLectures, value);
    }

    private ObservableAsPropertyHelper<bool> _areLecturesScheduledToday;
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
        ILecturesScheduler lecturesScheduler, IPersistentValidationContext persistentValidationContext) 
        : base(navigationService)
    {
        LecturesScheduler = lecturesScheduler;
        _logger = logger;
        _scheduledLectureRepository = scheduledLectureRepository;
        Activator = new ViewModelActivator();
        
        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
        {
            persistentValidationContext.RemoveAllValidationParameters();
            var parameters = new Dictionary<string, object>
                { { NavigationParameters.CreateLecture.IsUpdateMode, false } };
            NavigationService.Navigate(typeof(CreateLectureViewModel), HostNames.MainMenuHost, parameters);
        });

        _areLecturesScheduledToday =
            this.WhenAnyValue(vm => vm.TodaysLectures.Count)
                .Select(count => count > 0)
                .ToProperty(this, vm => vm.AreLecturesScheduledToday)
                .DisposeWith(_disposables);

        Observable.FromAsync(async () =>
        {
            var fetchLecturesTask = FetchTodaysLectures();
            var fetchStudentTask = studentAccountRepository.GetStudentAccount();

            await Task.WhenAll(fetchLecturesTask, fetchStudentTask);

            RegistrationNumber = fetchStudentTask.Result?.RegistrationNumber;
            
            if (fetchLecturesTask.Result is not null)
            {
                TodaysLectures.Clear();
                
                foreach (var lecture in fetchLecturesTask.Result)
                {
                    TodaysLectures.Add(lecture);
                    await Task.Delay(50);
                }
            }
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
