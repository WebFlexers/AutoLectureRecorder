using AutoLectureRecorder.WPF.Sections.Home;
using AutoLectureRecorder.WPF.Sections.Settings;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace AutoLectureRecorder.WPF;
public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.routedViewHost.Router)
                .DisposeWith(disposables);

            navigateToHomeButton
                .Events().Click
                .Subscribe(_ => ViewModel?.Navigate.Execute(typeof(HomeViewModel)).Subscribe())
                .DisposeWith(disposables);

            navigateToSettingsButton
                .Events().Click
                .Subscribe(_ => ViewModel?.Navigate.Execute(typeof(SettingsViewModel)).Subscribe())
                .DisposeWith(disposables);

            // TitleBar
            this.BindCommand(ViewModel, vm => vm.ExitAppCommand, v => v.exitAppButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.ToggleWindowStateCommand, v => v.toggleWindowStateButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.MinimizeWindowCommand, v => v.minimizeWindowButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.MainWindowState, v => v.mainAppWindow.WindowState)
                .DisposeWith(disposables);
            this.WhenAnyValue(v => v.mainAppWindow.WindowState)
                .Subscribe(ws =>
                {
                    this.toggleWindowStateButton.Style = ViewModel!.GetStyleFromResourceDictionary(
                            styleName: ws == System.Windows.WindowState.Maximized ? "TitlebarRestoreDownButton" : "TitlebarMaximizeButton",
                            resourceDictionaryName: "TitleBar.xaml"
                        );
                })
                .DisposeWith(disposables);
        });
    }
}