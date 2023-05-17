using System.Reactive.Disposables;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Settings;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // General Settings
            this.Bind(ViewModel, vm => vm.GeneralSettings.LaunchAtStartup, v => v.OnStartupToggleButton.IsChecked)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.GeneralSettings.OnCloseKeepAlive, v => v.OnCloseMinimizeToggleButton.IsChecked)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.GeneralSettings.ShowSplashScreen, v => v.ShowSplashScreenToggleButton.IsChecked)
                .DisposeWith(disposables);

            // Recording Settings
        });
    }
}
