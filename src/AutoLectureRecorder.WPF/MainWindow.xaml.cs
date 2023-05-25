using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Resources.Themes;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace AutoLectureRecorder;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        // Taskbar Icon
        // This is done outside of WhenActivated in order to work when
        // the app is started as a background service, since
        // Show() is not called on main window immediately, thus
        // not executing the code inside this.WhenActivated()
        ViewModel ??= Locator.Current.GetService<MainWindowViewModel>();

        this.BindCommand(ViewModel, vm => vm.ShowAppCommand, v => v.MainTaskbarIcon,
                Observable.Return(this), nameof(MainTaskbarIcon.TrayMouseDoubleClick));
        this.BindCommand(ViewModel, vm => vm.ShowAppCommand, v => v.OpenAppMenuItem,
                Observable.Return(this));
        this.BindCommand(ViewModel, vm => vm.ExitAppCommand, v => v.ExitAppMenuItem);

        MainTaskbarIcon.LeftClickCommand = ViewModel!.ShowAppCommand;
        MainTaskbarIcon.LeftClickCommandParameter = this;
        MainTaskbarIcon.NoLeftClickDelay = true;

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsWindowTopMost, v => v.MainAppWindow.Topmost)
                .DisposeWith(disposables);

            // TitleBar
            this.BindCommand(ViewModel, vm => vm.AttemptExitAppCommand, v => v.ExitAppButton, 
                    Observable.Return(this))
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.ToggleWindowStateCommand, v => v.ToggleWindowStateButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.MinimizeWindowCommand, v => v.MinimizeWindowButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.MainWindowState, v => v.MainAppWindow.WindowState)
                .DisposeWith(disposables);
            this.WhenAnyValue(v => v.MainAppWindow.WindowState)
                .Subscribe(ws =>
                {
                    this.ToggleWindowStateButton.Style = ThemeManager.GetStyleFromResourceDictionary(
                            styleName: ws == WindowState.Maximized ? "TitlebarRestoreDownButton" : "TitlebarMaximizeButton",
                            resourceDictionaryName: "TitleBar.xaml"
                        );
                })
                .DisposeWith(disposables);

            // Event
            this.Events().SizeChanged
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Subscribe(e =>
                {
                    MessageBus.Current.SendMessage(e, PubSubMessages.ScreenDimensionChanged);
                }).DisposeWith(disposables);
        });
    }
}