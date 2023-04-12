using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.RecordLectures;

public partial class RecordWindow : ReactiveWindow<RecordWindowViewModel>
{
    public RecordWindow()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            // Window State
            this.OneWayBind(ViewModel, vm => vm.RecordWindowState, v => v.RecordMainWindow.WindowState)
                .DisposeWith(disposables);

            // TODO: Update hover color on titlebar buttons when theme changes
            this.WhenAnyValue(v => v.RecordMainWindow.WindowState)
                .Subscribe(ws =>
                {
                    this.ToggleWindowStateButton.Style = App.GetStyleFromResourceDictionary(
                        styleName: ws == WindowState.Maximized ? "TitlebarRestoreDownButton" : "TitlebarMaximizeButton",
                        resourceDictionaryName: "TitleBar.xaml"
                    );
                })
                .DisposeWith(disposables);

            // Commands
            this.BindCommand(ViewModel, vm => vm.CloseWindowCommand, v => v.CloseWindowButton,
                    Observable.Return(this))
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.ToggleWindowStateCommand, v => v.ToggleWindowStateButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.MinimizeWindowCommand, v => v.MinimizeWindowButton)
                .DisposeWith(disposables);

        });
    }
}
