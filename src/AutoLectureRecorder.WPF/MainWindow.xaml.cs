using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Resources.Themes;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
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

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsWindowTopMost, v => v.MainAppWindow.Topmost)
                .DisposeWith(disposables);

            // TaskBar
            this.BindCommand(ViewModel, vm => vm.ShowAppCommand, v => v.MainTaskbarIcon,
                    Observable.Return(this), nameof(MainTaskbarIcon.TrayMouseDoubleClick))
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.ShowAppCommand, v => v.OpenAppMenuItem,
                    Observable.Return(this))
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.ExitAppCommand, v => v.ExitAppMenuItem)
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