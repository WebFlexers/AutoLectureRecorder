using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Resources.Themes;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using AutoLectureRecorder.WindowsServices;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AutoLectureRecorder;

public class MainWindowViewModel : ReactiveObject, IScreen, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    private CompositeDisposable _disposables = new();

    private readonly IViewModelFactory _viewModelFactory;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ILogger<MainWindowViewModel> _logger;

    public RoutingState Router { get; } = new();

    // Titlebar commands
    public ReactiveCommand<Window, Unit> AttemptExitAppCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitAppCommand { get; }
    public ReactiveCommand<Window, Unit> ShowAppCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; }

    public ReactiveCommand<Type, Unit> Navigate { get; }

    [Reactive]
    public bool IsWindowTopMost { get; set; }
    [Reactive]
    public WindowState MainWindowState { get; set; }
    [Reactive]
    public Style MaximizeButtonStyle { get; set; }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IViewModelFactory viewModelFactory, 
        ISettingsRepository settingsRepository)
    {
        _viewModelFactory = viewModelFactory;
        _settingsRepository = settingsRepository;
        _logger = logger;

        // Navigation
        Navigate = ReactiveCommand.Create<Type>(SetRoutedViewHostContent);

        // Titlebar
        MaximizeButtonStyle = ThemeManager.GetStyleFromResourceDictionary("TitlebarMaximizeButton", "TitleBar.xaml")!;

        ShowAppCommand = ReactiveCommand.Create<Window, Unit>(window =>
        {
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.WindowState = WindowState.Normal;
            window.Show();
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            Win32Api.ShowWindow(windowHandle, 5);
            Win32Api.SetForegroundWindow(windowHandle);
            return Unit.Default;
        });

        AttemptExitAppCommand = ReactiveCommand.CreateFromTask<Window, Unit>(async window =>
        {
            var generalSettings = await settingsRepository.GetGeneralSettings();
            if (generalSettings != null && generalSettings.OnCloseKeepAlive)
            {
                window.Hide();
                return Unit.Default;
            }

            Application.Current.Shutdown();
            return Unit.Default;
        });

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

        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }

    public void SetRoutedViewHostContent(Type type)
    {
        if (Router.NavigationStack.LastOrDefault()?.GetType() == type) return;

        Router.NavigateAndReset.Execute(_viewModelFactory.CreateRoutableViewModel(type));

        _logger.LogInformation("Navigated to {viewModel}", type.Name);
    }
}
