using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.RecordLecture;

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
            this.OneWayBind(ViewModel, 
                    vm => vm.RecordWindowState, 
                    v => v.RecordMainWindow.WindowState)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.RecordMainWindow.WindowState)
                .Subscribe(ws =>
                {
                    this.ToggleWindowStateButton.Style = ws == WindowState.Maximized
                        ? AlrResources.Styles.TitlebarRestoreDownButton
                        : AlrResources.Styles.TitlebarMaximizeButton;
                })
                .DisposeWith(disposables);

            // WebView
            this.OneWayBind(ViewModel, 
                    vm => vm.WebViewSource, 
                    v => v.MainWebView.Source)
                .DisposeWith(disposables);

            MainWebView
                .Events().CoreWebView2InitializationCompleted
                .Select(_ => this.MainWebView)
                .InvokeCommand(this, v => v.ViewModel!.JoinAndRecordMeetingCommand)
                .DisposeWith(disposables);

            MainWebView.DisposeWith(disposables);

            // Commands
            this.BindCommand(ViewModel, 
                    vm => vm.CloseWindowCommand, 
                    v => v.CloseWindowButton,
                    Observable.Return(this))
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, 
                    vm => vm.ToggleWindowStateCommand, 
                    v => v.ToggleWindowStateButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, 
                    vm => vm.MinimizeWindowCommand, 
                    v => v.MinimizeWindowButton)
                .DisposeWith(disposables);
            
            // Cleanup on close
            this.RecordMainWindow.Events().Closing
                .Select(_ => this.MainWebView)
                .InvokeCommand(ViewModel!.CleanupBeforeExitCommand)
                .DisposeWith(disposables);
        });
    }
}
