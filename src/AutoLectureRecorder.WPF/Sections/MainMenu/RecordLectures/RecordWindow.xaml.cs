using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows;
using AutoLectureRecorder.Resources.Themes;

namespace AutoLectureRecorder.Sections.MainMenu.RecordLectures;

public partial class RecordWindow : ReactiveWindow<RecordWindowViewModel>
{
    private bool _currentlyCleaningResources = false;
    private bool _resourcesCleaned = false;

    public RecordWindow()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            DataContext = ViewModel;

            // Title Bar
            this.WhenAnyValue(v => v._currentlyCleaningResources)
                .Subscribe(cleaningResources =>
                {
                    CloseWindowButton.IsEnabled = cleaningResources == false;
                }).DisposeWith(disposables);

            // Window State
            this.OneWayBind(ViewModel, vm => vm.RecordWindowState, v => v.RecordMainWindow.WindowState)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.RecordMainWindow.WindowState)
                .Subscribe(ws =>
                {
                    this.ToggleWindowStateButton.Style = ThemeManager.GetStyleFromResourceDictionary(
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

            // TODO: Closing event is used twice (once here and once above). Investigate
            // On Closing
            this.Events().Closing
                .Subscribe(async e =>
                {
                    if (_currentlyCleaningResources || _resourcesCleaned) return;

                    _currentlyCleaningResources = true;
                    e.Cancel = true;

                    var cleanupTask = ViewModel!.CleanupResourcesCommand.Execute()
                        .Take(1)
                        .ToTask();

                    e.Cancel = false;
                    _resourcesCleaned = true;
                    _currentlyCleaningResources = false;

                    await cleanupTask;
                }).DisposeWith(disposables);
        });
    }
}
