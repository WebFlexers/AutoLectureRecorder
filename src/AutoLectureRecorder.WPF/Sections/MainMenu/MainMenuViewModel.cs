using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.Login;
using AutoLectureRecorder.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.Sections.MainMenu.Dashboard;
using AutoLectureRecorder.Sections.MainMenu.Library;
using AutoLectureRecorder.Sections.MainMenu.Schedule;
using AutoLectureRecorder.Sections.MainMenu.Settings;
using AutoLectureRecorder.Sections.MainMenu.Upload;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.Sections.MainMenu;

public class MainMenuViewModel : ReactiveObject, IRoutableViewModel, IScreen, IActivatableViewModel
{
    private readonly ILogger<MainMenuViewModel> _logger;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IStudentAccountRepository _studentAccountData;

    public RoutingState Router { get; } = new();
    public ViewModelActivator Activator { get; } = new();
    public string UrlPathSegment => nameof(MainMenuViewModel);
    public IScreen HostScreen { get; }

    // Navigation Commands
    public ReactiveCommand<Unit, Unit> NavigateToCreateLectureCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToDashboardCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToLibraryCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToScheduleCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToUploadCommand { get; }

    public ReactiveCommand<Unit, Unit> NavigateToRecordWindowCommand { get; }

    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateForwardCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    // An extra navigation stack that handles forward and back navigation
    // since it doesn't already exist in ReactiveUI.
    // IMPORTANT: It also avoids memory leaks caused by ViewModel instances
    // being directly referenced by the ReactiveUI Router NavigationStack.
    // For that reason NavigateAndReset must always be used instead of Navigate
    // in order for the NavigationStack of the Router to only contain the current ViewModel
    private int _currentNavigationIndex = 0;
    private readonly List<Type> _navigationStack = new();

    [Reactive]
    public Visibility MenuVisibility { get; set; } = Visibility.Visible;

    public MainMenuViewModel(ILogger<MainMenuViewModel> logger, IScreenFactory screenFactory, IViewModelFactory viewModelFactory, 
        IWindowFactory windowFactory, IStudentAccountRepository studentAccountData)
    {
        _logger = logger;
        _viewModelFactory = viewModelFactory;
        _studentAccountData = studentAccountData;
        HostScreen = screenFactory.GetMainWindowViewModel();

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
            Navigate(typeof(CreateLectureViewModel)));
        NavigateToDashboardCommand = ReactiveCommand.Create(() =>
            Navigate(typeof(DashboardViewModel)));
        NavigateToLibraryCommand = ReactiveCommand.Create(() =>
            Navigate(typeof(LibraryViewModel)));
        NavigateToScheduleCommand = ReactiveCommand.Create(() =>
            Navigate(typeof(ScheduleViewModel)));
        NavigateToSettingsCommand = ReactiveCommand.Create(() =>
            Navigate(typeof(SettingsViewModel)));
        NavigateToUploadCommand = ReactiveCommand.Create(() =>
            Navigate(typeof(UploadViewModel)));

        NavigateToRecordWindowCommand = ReactiveCommand.Create(() =>
        {
            // Algorithmoi: https://teams.microsoft.com/l/meetup-join/19%3ameeting_NjFmMWM0ZjctNTFiNC00MTc0LWFjYTQtMzlhMGRkNTM0NjFi%40thread.v2/0?context=%7b%22Tid%22%3a%22d9c8dee3-558b-483d-b502-d31fa0cb24de%22%2c%22Oid%22%3a%2220dcfee8-a2d9-4250-aa03-bde3530991d8%22%7d
            // TODO: Modify this test method
            var recordWindow = windowFactory.CreateRecordWindow(new ReactiveScheduledLecture
            {
                Id = 5000,
                SubjectName = "Εκπαιδευτικό Λογισμικό",
                Semester = 8,
                MeetingLink = @"https://teams.microsoft.com/l/meetup-join/19%3Ameeting_OGE0OWFiNmUtYzM4YS00Y2IwLWI1NjgtMGI4NjQ4ZWVjMTM0%40thread.v2/0?context=%7B%22Tid%22%3A%225f3b4a0c-0b1e-4776-9e95-6933e4408e97%22%2C%22Oid%22%3A%22a0248d49-c7e0-48f7-94c2-d5f02c150d78%22%7D",
                Day = DayOfWeek.Friday,
                StartTime = DateTime.MinValue.AddHours(8),
                EndTime = DateTime.MinValue.AddHours(10).AddMinutes(15),
                IsScheduled = true,
                WillAutoUpload = false
            });
            recordWindow.Show();
        });

        NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
        NavigateForwardCommand = ReactiveCommand.Create(NavigateForward);

        LogoutCommand = ReactiveCommand.CreateFromTask(Logout);

        // To handle fullscreen player mode we can't just move the VlcPlayer control to a new view,
        // because it stops the player and creates many problems. Instead we have to hide everything from
        // the screen and make the window fullscreen
        MessageBus.Current.Listen<bool>(PubSubMessages.UpdateVideoFullScreen)
            .Subscribe(ToggleMenuVisibility);

        // Navigate to the Dashboard at startup
        this.WhenActivated(disposables =>
        {
            NavigateToDashboardCommand.Execute()
                .Subscribe()
                .DisposeWith(disposables);
        });
    }

    private async Task Logout()
    {
        await _studentAccountData.DeleteStudentAccountAsync();
        HostScreen.Router.NavigateAndReset.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));
    }

    private void ToggleMenuVisibility(bool isFullScreen)
    {
        MenuVisibility = isFullScreen ? Visibility.Collapsed : Visibility.Visible;
    }

    private void SetRoutedViewHostContent(IRoutableViewModel viewModel)
    {
        var vmType = viewModel.GetType();
        if (Router.NavigationStack.LastOrDefault()?.GetType() == vmType) return;

        // Navigate and reset to avoid memory leaks with ViewModels being retained in memory
        Router.NavigateAndReset.Execute(viewModel);
        _logger.LogInformation("Navigated to {viewModel}", vmType.Name);
    }

    public void Navigate(IRoutableViewModel viewModel)
    {
        if (_currentNavigationIndex < _navigationStack.Count - 1)
        {
            for (int i = _navigationStack.Count - 1; i > _currentNavigationIndex; i--)
            {
                _navigationStack.RemoveAt(i);
            }
        }

        var vmType = viewModel.GetType();

        _navigationStack.Add(vmType);
        _currentNavigationIndex = _navigationStack.Count - 1;

        SetRoutedViewHostContent(viewModel);
    }

    public void Navigate(Type type)
    {
        var viewModel = _viewModelFactory.CreateRoutableViewModel(type);
        Navigate(viewModel);
    }

    public void NavigateBack()
    {
        var backIndex = _currentNavigationIndex - 1;
        var navigationStackCount = _navigationStack.Count;

        if (navigationStackCount > 1 && backIndex >= 0)
        {
            _currentNavigationIndex = backIndex;
            var viewModelType = _navigationStack.ElementAt(backIndex);
            SetRoutedViewHostContent(_viewModelFactory.CreateRoutableViewModel(viewModelType));
        }
        else
        {
            _logger.LogWarning("Tried to navigate back, but there was no ViewModel left on the stack");
        }
    }

    public void NavigateForward()
    {
        var forwardIndex = _currentNavigationIndex + 1;
        var navigationStackCount = _navigationStack.Count;

        if (navigationStackCount > 1 && forwardIndex < navigationStackCount)
        {
            _currentNavigationIndex = forwardIndex;
            var viewModelType = _navigationStack.ElementAt(forwardIndex);
            SetRoutedViewHostContent(_viewModelFactory.CreateRoutableViewModel(viewModelType));
        }
        else
        {
            _logger.LogWarning("Tried to navigate forward, but there was no ViewModel left on the stack");
        }
    }
}
