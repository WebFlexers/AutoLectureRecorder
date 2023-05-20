using System.Reactive.Disposables;
using System;
using AutoLectureRecorder.Services.Recording;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Settings;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            // General Settings
            this.OneWayBind(ViewModel, vm => vm.SupportedScreenResolutions, v => v.OutputResolutionCombobox.ItemsSource)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.GeneralSettings.LaunchAtStartup, v => v.OnStartupToggleButton.IsChecked)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.GeneralSettings.OnCloseKeepAlive, v => v.OnCloseMinimizeToggleButton.IsChecked)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.GeneralSettings.ShowSplashScreen, v => v.ShowSplashScreenToggleButton.IsChecked)
                .DisposeWith(disposables);

            // Recording Settings
            // Video
            this.Bind(ViewModel, vm => vm.RecordingSettings.RecordingsLocalPath, v => v.RecordingPathTextBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.RecordingSettings.Quality, v => v.QualityTextBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.RecordingSettings.Fps, v => v.FpsTextBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.SelectedResolution, v => v.OutputResolutionCombobox.Text,
                    this.ConvertResolutionToString, this.ConvertStringToResolution)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.BrowseDirectoryCommand, v => v.BrowseRecordingDirectoryButton)
                .DisposeWith(disposables);

            // Audio
            this.Bind(ViewModel, vm => vm.RecordingSettings.IsInputDeviceEnabled, v => v.RecordAudioToggleButton.IsChecked)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.InputAudioDevices, v => v.InputDeviceCombobox.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.OutputAudioDevices, v => v.OutputDeviceCombobox.ItemsSource)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.SelectedInputDevice, v => v.InputDeviceCombobox.Text,
                    this.ConvertAudioDeviceToString, this.ConvertStringToAudioDevice)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.SelectedOutputDevice, v => v.OutputDeviceCombobox.Text,
                    this.ConvertAudioDeviceToString, this.ConvertStringToAudioDevice)
                .DisposeWith(disposables);
        });
    }

    private string ConvertResolutionToString(ReactiveResolution? resolution)
    {
        return $"{resolution?.Width}x{resolution?.Height}";
    }

    private ReactiveResolution ConvertStringToResolution(string resolution)
    {
        var splitDimensions = resolution.Split('x');
        return new ReactiveResolution
        {
            Width = Int32.Parse(splitDimensions[0]),
            Height = Int32.Parse(splitDimensions[1])
        };
    }

    private string ConvertAudioDeviceToString(ReactiveAudioDevice? audioDevice)
    {
        return audioDevice?.FriendlyName != null ? audioDevice.FriendlyName : string.Empty;
    }

    private ReactiveAudioDevice ConvertStringToAudioDevice(string deviceFriendlyName)
    {
        return new ReactiveAudioDevice
        {
            FriendlyName = deviceFriendlyName
        };
    }
}
