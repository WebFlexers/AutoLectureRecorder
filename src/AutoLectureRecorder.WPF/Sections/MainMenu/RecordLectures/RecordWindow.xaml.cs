using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace AutoLectureRecorder.Sections.MainMenu.RecordLectures;

public partial class RecordWindow : ReactiveWindow<RecordWindowViewModel>
{
    public RecordWindow()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            DataContext = ViewModel;

            // Window State
            this.OneWayBind(ViewModel, vm => vm.RecordWindowState, v => v.RecordMainWindow.WindowState)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.RecordMainWindow.WindowState)
                .Subscribe(ws =>
                {
                    this.ToggleWindowStateButton.Style = App.GetStyleFromResourceDictionary(
                        styleName: ws == WindowState.Maximized ? "TitlebarRestoreDownButton" : "TitlebarMaximizeButton",
                        resourceDictionaryName: "TitleBar.xaml"
                    );
                })
                .DisposeWith(disposables);

            // WebView
            this.OneWayBind(ViewModel, vm => vm.WebViewSource, v => v.MainWebView.Source)
                .DisposeWith(disposables);

            MainWebView
                .Events().CoreWebView2InitializationCompleted
                .Select(args => this.MainWebView)
                .InvokeCommand(this, v => v.ViewModel!.JoinAndRecordMeetingCommand)
                .DisposeWith(disposables);

            RecordMainWindow
                .Events().Closing
                .Select(args => Unit.Default)
                .InvokeCommand(this, v => v.ViewModel!.CleanupResourcesCommand)
                .DisposeWith(disposables);

            MainWebView.DisposeWith(disposables);

            // Commands
            this.BindCommand(ViewModel, vm => vm.CloseWindowCommand, v => v.CloseWindowButton,
                    Observable.Return(this))
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.ToggleWindowStateCommand, v => v.ToggleWindowStateButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.MinimizeWindowCommand, v => v.MinimizeWindowButton)
                .DisposeWith(disposables);

            // Close window after finishing
            this.WhenAnyValue(v => v.ViewModel!.IsRecordingFinished)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(isRecordingFinished =>
                {
                    if (_resourcesCleaned && isRecordingFinished)
                    {
                        this.Close();
                    }
                }).DisposeWith(disposables);
        });
    }

    private bool _resourcesCleaned = false;
    private async void RecordWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        if (_resourcesCleaned) return;

        e.Cancel = true;
        await ViewModel!.CleanupResourcesCommand.Execute();
        _resourcesCleaned = true;
    }
}
