using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using AutoLectureRecorder.ReactiveUiUtilities;
using ReactiveMarbles.ObservableEvents;
using Serilog;

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

            // TitleBar
            this.BindCommand(ViewModel, vm => vm.ExitAppCommand, v => v.ExitAppButton)
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
                    this.ToggleWindowStateButton.Style = App.GetStyleFromResourceDictionary(
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