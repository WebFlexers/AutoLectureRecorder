using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.Login;
using AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;
using AutoLectureRecorder.WPF.Sections.MainMenu.Library;
using AutoLectureRecorder.WPF.Sections.MainMenu.Schedule;
using AutoLectureRecorder.WPF.Sections.MainMenu.Settings;
using AutoLectureRecorder.WPF.Sections.MainMenu.Upload;
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

namespace AutoLectureRecorder.WPF.Sections.MainMenu;

public class MainMenuViewModel : ReactiveObject, IRoutableViewModel, IScreen, IActivatableViewModel
{
    private readonly ILogger<MainMenuViewModel> _logger;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IStudentAccountData _studentAccountData;

    public RoutingState Router { get; } = new RoutingState();
    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    public string? UrlPathSegment => nameof(MainMenuViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> NavigateToCreateLectureCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> NavigateToDashboardCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> NavigateToLibraryCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> NavigateToScheduleCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> NavigateToSettingsCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> NavigateToUploadCommand { get; private set; }

    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> NavigateForwardCommand { get; private set; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }

    public MainMenuViewModel(ILogger<MainMenuViewModel> logger, IScreenFactory screenFactory, IViewModelFactory viewModelFactory, IStudentAccountData studentAccountData)
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

        MessageBus.Current.Listen<bool>("VideoFullScreen").Subscribe(isFullScreen =>
        {
            ToggleMenuVisibility(isFullScreen);
        });

        this.WhenActivated(disposables =>
        {
            NavigateToCreateLectureCommand.Execute()
                .Subscribe()
                .DisposeWith(disposables);
        });
    }

    private async Task Logout()
    {
        await _studentAccountData.DeleteStudentAccount();
        HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));
    }

    private Stack<Type> _navigationStack = new Stack<Type>();
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

    [Reactive]
    public Visibility MenuVisibility { get; set; } = Visibility.Visible;

    private void ToggleMenuVisibility(bool isFullScreen)
    {
        if (isFullScreen)
        {
            MenuVisibility = Visibility.Collapsed;
        }
        else
        {
            MenuVisibility = Visibility.Visible;
        }
    }
}
