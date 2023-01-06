using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.MainMenu.Schedule;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using System.Reactive;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;

public class DashboardViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ILogger<DashboardViewModel> _logger;
    private readonly IViewModelFactory _viewModelFactory;

    public string? UrlPathSegment => nameof(DashboardViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> NavigateToScheduleCommand { get; private set; }

    public DashboardViewModel(ILogger<DashboardViewModel> logger, IScreenFactory screenFactory, IViewModelFactory viewModelFactory)
    {
        HostScreen = screenFactory.GetMainMenuViewModel();
        _logger = logger;
        _viewModelFactory = viewModelFactory;

        NavigateToScheduleCommand = ReactiveCommand.Create(() => {
            HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(ScheduleViewModel)));
            _logger.LogInformation("It happened");
        });
    }
}
