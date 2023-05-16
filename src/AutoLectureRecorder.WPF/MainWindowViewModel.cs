using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using AutoLectureRecorder.Resources.Themes;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;

namespace AutoLectureRecorder;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly ILogger<MainWindowViewModel> _logger;

    public RoutingState Router { get; } = new();

    // Titlebar commands
    public ReactiveCommand<Unit, Unit> ExitAppCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; }

    public ReactiveCommand<Type, Unit> Navigate { get; }

    private bool _isFullScreenVideoPlaying;
    [Reactive]
    public bool IsWindowTopMost { get; set; }
    [Reactive]
    public WindowState MainWindowState { get; set; }
    [Reactive]
    public Style MaximizeButtonStyle { get; set; }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IViewModelFactory viewModelFactory, IScheduledLectureRepository scheduledLectureRepository)
    {
        _viewModelFactory = viewModelFactory;
        _scheduledLectureRepository = scheduledLectureRepository;
        _logger = logger;

        // Navigation
        Navigate = ReactiveCommand.Create<Type>(SetRoutedViewHostContent);

        // Titlebar
        MaximizeButtonStyle = ThemeManager.GetStyleFromResourceDictionary("TitlebarMaximizeButton", "TitleBar.xaml")!;

        ExitAppCommand = ReactiveCommand.Create(Application.Current.Shutdown);
        ToggleWindowStateCommand = ReactiveCommand.Create(() =>
        {
            if (MainWindowState == WindowState.Maximized)
            {
                MainWindowState = WindowState.Normal;
            }
            else
            {
                MainWindowState = WindowState.Maximized;
            }
        });
        MinimizeWindowCommand = ReactiveCommand.Create(() => MainWindowState = WindowState.Minimized);

        //MessageBus.Current.Listen<bool>(PubSubMessages.UpdateWindowTopMost)
        //    .Subscribe(tm => IsWindowTopMost = tm);

        MessageBus.Current.Listen<bool>(PubSubMessages.UpdateVideoFullScreen)
            .Subscribe(makeVideoFullScreen =>
            {
                _isFullScreenVideoPlaying = makeVideoFullScreen;
                ToggleFullscreenVideoMode();
            });
    }

    private void ToggleFullscreenVideoMode()
    {
        IsWindowTopMost = _isFullScreenVideoPlaying;
        MainWindowState = _isFullScreenVideoPlaying ? WindowState.Maximized : WindowState.Normal;
    }

    public void SetRoutedViewHostContent(Type type)
    {
        if (Router.NavigationStack.LastOrDefault()?.GetType() == type) return;

        Router.NavigateAndReset.Execute(_viewModelFactory.CreateRoutableViewModel(type));

        _logger.LogInformation("Navigated to {viewModel}", type.Name);
    }
}
