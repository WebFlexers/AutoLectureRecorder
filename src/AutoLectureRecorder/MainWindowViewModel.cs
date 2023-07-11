using System;
using System.Reactive;
using System.Windows;
using System.Windows.Interop;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.WindowsServices;
using ReactiveUI;

namespace AutoLectureRecorder;

public class MainWindowViewModel : RoutableViewModelHost
{
    public ReactiveCommand<Window, Unit> AttemptExitAppCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitAppCommand { get; }
    public ReactiveCommand<Window, Unit> ShowAppCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; }

    private WindowState _mainWindowState;
    public WindowState MainWindowState
    {
        get => _mainWindowState;
        set => this.RaiseAndSetIfChanged(ref _mainWindowState, value);
    }

    public MainWindowViewModel(INavigationService navigationService) : base(navigationService)
    {
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
            /*var generalSettings = await settingsRepository.GetGeneralSettings();
            if (generalSettings != null && generalSettings.OnCloseKeepAlive)
            {
                window.Hide();
                return Unit.Default;
            }*/

            System.Windows.Application.Current.Shutdown();
            return Unit.Default;
        });

        ExitAppCommand = ReactiveCommand.Create(System.Windows.Application.Current.Shutdown);

        ToggleWindowStateCommand = ReactiveCommand.Create(() =>
        {
            MainWindowState = MainWindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        });

        MinimizeWindowCommand = ReactiveCommand.Create(() => MainWindowState = WindowState.Minimized);
    }
}