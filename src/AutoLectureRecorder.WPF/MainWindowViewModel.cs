using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace AutoLectureRecorder.WPF;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IScheduledLectureData _lectureData;
    private readonly ILogger<MainWindowViewModel> _logger;

    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, Unit> ExitAppCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; set; }
    public ReactiveCommand<string, Unit> UpdateMaximizedButtonStyle { get; set; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; set; }
    public ReactiveCommand<Type, Unit> Navigate { get; private set; }

    [Reactive]
    public bool IsWindowTopMost { get; set; } = false;
    [Reactive]
    public WindowState MainWindowState { get; set; }
    [Reactive]
    public Style MaximizeButtonStyle { get; set; }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IViewModelFactory viewModelFactory, IScheduledLectureData lectureData)
    {
        _viewModelFactory = viewModelFactory;
        _lectureData = lectureData;
        _logger = logger;

        // Navigation
        Navigate = ReactiveCommand.Create<Type>(SetRoutedViewHostContent);

        // Titlebar
        MaximizeButtonStyle = ((App)Application.Current)
                .GetStyleFromResourceDictionary("TitlebarMaximizeButton", "TitleBar.xaml")!;

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

        // Message Buses
        MessageBus.Current.Listen<bool>(PubSubMessages.UpdateWindowTopMost).Subscribe(tm => IsWindowTopMost = tm);

        MessageBus.Current.Listen<bool>(PubSubMessages.UpdateVideoFullScreen).Subscribe(makeVideoFullScreen =>
        {
            _isFullScreenVideoPlaying = makeVideoFullScreen;
            ToggleFullscreenVideoMode();
        });
    }

    private bool _isFullScreenVideoPlaying = false;

    private void ToggleFullscreenVideoMode()
    {
        IsWindowTopMost = _isFullScreenVideoPlaying;

        if (_isFullScreenVideoPlaying)
        {
            MainWindowState = WindowState.Maximized;
        }
        else
        {
            MainWindowState = WindowState.Normal;
        }
    }

    public void SetRoutedViewHostContent(Type type)
    {
        if (Router.NavigationStack.LastOrDefault()?.GetType() == type)
        {
            return;
        }

        Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(type));

        _logger.LogInformation("Navigated to {viewModel}", type.Name);
    }
}
