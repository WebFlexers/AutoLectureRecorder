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
using AutoLectureRecorder.Services.DataAccess.Interfaces;
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
            // TODO: Modify this test method
            var recordWindow = windowFactory.CreateRecordWindow(new ReactiveScheduledLecture
            {
                Id = 1,
                SubjectName = "My test lecture",
                Semester = 2,
                MeetingLink = "https://teams.microsoft.com/l/team/19%3a4f80471db6464f18a64f47be0dcd660d%40thread.tacv2/conversations?groupId=29a0e98c-f210-4df8-afe9-a9f9f6d02264&tenantId=d9c8dee3-558b-483d-b502-d31fa0cb24de",
                Day = DayOfWeek.Wednesday,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddMinutes(1),
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

    private void SetRoutedViewHostContent(Type type)
    {
        if (Router.NavigationStack.LastOrDefault()?.GetType() == type) return;

        // Navigate and reset to avoid memory leaks with ViewModels being retained in memory
        Router.NavigateAndReset.Execute(_viewModelFactory.CreateRoutableViewModel(type));
        _logger.LogInformation("Navigated to {viewModel}", type.Name);
    }

    public void Navigate(Type type)
    {
        if (_currentNavigationIndex < _navigationStack.Count - 1)
        {
            for (int i = _navigationStack.Count - 1; i > _currentNavigationIndex; i--)
            {
                _navigationStack.RemoveAt(i);
            }
        }

        _navigationStack.Add(type);
        _currentNavigationIndex = _navigationStack.Count - 1;

        SetRoutedViewHostContent(type);
    }

    public void NavigateBack()
    {
        var backIndex = _currentNavigationIndex - 1;
        var navigationStackCount = _navigationStack.Count;

        if (navigationStackCount > 1 && backIndex >= 0)
        {
            _currentNavigationIndex = backIndex;
            var viewModelType = _navigationStack.ElementAt(backIndex);
            SetRoutedViewHostContent(viewModelType);
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
            SetRoutedViewHostContent(viewModelType);
        }
        else
        {
            _logger.LogWarning("Tried to navigate forward, but there was no ViewModel left on the stack");
        }
    }
}
