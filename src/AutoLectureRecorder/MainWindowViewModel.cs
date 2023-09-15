using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Options;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Messages;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.WindowsServices;
using ReactiveUI;

namespace AutoLectureRecorder;

public class MainWindowViewModel : RoutableViewModelHost
{
    /// <summary>
    /// Attempts to close the given window. If the on close keep alive setting is set to true and forceClose is false
    /// then the application is hidden instead of shutting down completely
    /// </summary>
    public ReactiveCommand<(Window window, CancelEventArgs cancelEventArgs), Unit> AttemptExitAppCommand { get; }
    /// <summary>
    /// If the bool argument is set to true the application shuts down without minimizing for any reason. Otherwise
    /// if the on close keep alive setting is set to true then the app is minimized instead of closing
    /// </summary>
    public ReactiveCommand<(Window, bool), Unit> ExitAppCommand { get; }
    public ReactiveCommand<Window, Unit> ShowAppCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenBrowserHelpPageCommand { get; }

    private bool _isHelpModalOpen = false;
    public bool IsHelpModalOpen
    {
        get => _isHelpModalOpen;
        set => this.RaiseAndSetIfChanged(ref _isHelpModalOpen, value);
    }
    
    private bool _forceAppShutdown = false;

    private WindowState _mainWindowState;
    public WindowState MainWindowState
    {
        get => _mainWindowState;
        set => this.RaiseAndSetIfChanged(ref _mainWindowState, value);
    }

    public MainWindowViewModel(INavigationService navigationService, ISettingsRepository settingsRepository) 
        : base(navigationService)
    {
        ShowAppCommand = ReactiveCommand.Create<Window, Unit>(window =>
        {
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.WindowState = WindowState.Normal;
            window.Show();
            
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            Win32Api.ShowWindow(windowHandle, 5);
            Win32Api.SetForegroundWindow(windowHandle);
            
            ((App)System.Windows.Application.Current).MakeWindowFullScreenOnTooLargeScreen();
            
            return Unit.Default;
        });

        AttemptExitAppCommand = ReactiveCommand.CreateFromTask<(Window, CancelEventArgs), Unit>(async
            parameters =>
        {
            var window = parameters.Item1;
            var cancelEventArgs = parameters.Item2;

            var generalSettings = await settingsRepository.GetGeneralSettings();
            if (generalSettings is { OnCloseKeepAlive: true } && _forceAppShutdown == false)
            {
                cancelEventArgs.Cancel = true;
                window.Hide();
                return Unit.Default;
            }
            
            System.Windows.Application.Current.Shutdown();
            
            return Unit.Default;
        });
        
        ExitAppCommand = ReactiveCommand.Create<(Window, bool)>(parameters =>
        {
            // TODO: Add a check to notify the user if a recording is currently active
            Window window = parameters.Item1;
            _forceAppShutdown = parameters.Item2;
            
            window.Close();
        });

        ToggleWindowStateCommand = ReactiveCommand.Create(() =>
        {
            MainWindowState = MainWindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        });

        MinimizeWindowCommand = ReactiveCommand.Create(() => MainWindowState = WindowState.Minimized);
        
        OpenBrowserHelpPageCommand = ReactiveCommand.Create(() =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Websites.HelpPageLink,
                UseShellExecute = true
            });

            IsHelpModalOpen = false;
        });

        MessageBus.Current.Listen<Unit>(PubSubMessages.OpenHelpPageModal)
            .Subscribe(_ => IsHelpModalOpen = true);
    }
}