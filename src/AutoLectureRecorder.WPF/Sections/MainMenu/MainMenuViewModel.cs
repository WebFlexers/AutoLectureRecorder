using AutoLectureRecorder.DependencyInjection.Factories;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.Login;
using AutoLectureRecorder.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.Sections.MainMenu.Dashboard;
using AutoLectureRecorder.Sections.MainMenu.Library;
using AutoLectureRecorder.Sections.MainMenu.Schedule;
using AutoLectureRecorder.Sections.MainMenu.Settings;
using AutoLectureRecorder.Sections.MainMenu.Upload;
using AutoLectureRecorder.Services.DataAccess;
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

    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateForwardCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    // An extra navigation stack that handles forward navigation
    // since it doesn't already exist in ReactiveUI
    private readonly Stack<Type> _navigationStack = new();

    [Reactive]
    public Visibility MenuVisibility { get; set; } = Visibility.Visible;

    public MainMenuViewModel(ILogger<MainMenuViewModel> logger, IScreenFactory screenFactory, IViewModelFactory viewModelFactory, IStudentAccountRepository studentAccountData)
    {
        _logger = logger;
        _viewModelFactory = viewModelFactory;
        _studentAccountData = studentAccountData;
        HostScreen = screenFactory.GetMainWindowViewModel();

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
            SetRoutedViewHostContent(typeof(CreateLectureViewModel)));
        NavigateToDashboardCommand = ReactiveCommand.Create(() =>
            SetRoutedViewHostContent(typeof(DashboardViewModel)));
        NavigateToLibraryCommand = ReactiveCommand.Create(() =>
            SetRoutedViewHostContent(typeof(LibraryViewModel)));
        NavigateToScheduleCommand = ReactiveCommand.Create(() =>
            SetRoutedViewHostContent(typeof(ScheduleViewModel)));
        NavigateToSettingsCommand = ReactiveCommand.Create(() =>
            SetRoutedViewHostContent(typeof(SettingsViewModel)));
        NavigateToUploadCommand = ReactiveCommand.Create(() =>
            SetRoutedViewHostContent(typeof(UploadViewModel)));

        NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
        NavigateForwardCommand = ReactiveCommand.Create(NavigateForward);

        LogoutCommand = ReactiveCommand.CreateFromTask(Logout);

        // To handle fullscreen player mode we can't just move the VlcPlayer control to a new view,
        // because it stops the player and creates many problems. Instead we have to hide everything from
        // the screen and make the window fullscreen
        MessageBus.Current.Listen<bool>(PubSubMessages.UpdateVideoFullScreen).Subscribe(ToggleMenuVisibility);

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
        HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));
    }

    public void SetRoutedViewHostContent(Type type)
    {
        if (Router.NavigationStack.LastOrDefault()?.GetType() == type)
        {
            return;
        }

        Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(type));

        _navigationStack.Push(type);
        _logger.LogInformation("Navigated to {viewModel}", type.Name);
    }

    private void NavigateBack()
    {
        if (Router.NavigationStack.Count > 1)
        {
            Router.NavigateBack.Execute();
            _logger.LogInformation("Navigated to {viewModel}", Router.GetCurrentViewModel()!.ToString());
        }
        else
        {
            _logger.LogWarning("Tried to navigate back, but there was no ViewModel left on the stack");
        }
    }

    private void NavigateForward()
    {
        if (_navigationStack.Count > 1)
        {
            Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(_navigationStack.Pop()));
            _logger.LogInformation("Navigated to {viewModel}", Router.GetCurrentViewModel()!.ToString());
        }
        else
        {
            _logger.LogWarning("Tried to navigate forward, but there was no ViewModel left on the stack");
        }
    }

    private void ToggleMenuVisibility(bool isFullScreen)
    {
        MenuVisibility = isFullScreen ? Visibility.Collapsed : Visibility.Visible;
    }
}
