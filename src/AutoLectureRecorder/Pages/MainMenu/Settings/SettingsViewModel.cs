using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Application.Common.Abstractions.StartupManager;
using AutoLectureRecorder.Application.Recording;
using AutoLectureRecorder.Application.Recording.Common;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Domain.ReactiveModels;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Settings;

public class SettingsViewModel : RoutableViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    private readonly CompositeDisposable _disposables = new();

    private readonly ILogger<SettingsViewModel> _logger;
    private readonly INavigationService _navigationService;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IRecorder _recorder;
    private readonly IStartupManager _startupManager;

    public ObservableCollection<ReactiveAudioDevice> InputAudioDevices { get; private set; }
    public ObservableCollection<ReactiveAudioDevice> OutputAudioDevices { get; private set; }
    public ObservableCollection<ReactiveResolution> SupportedScreenResolutions { get; private set; }

    private ReactiveResolution _selectedResolution;
    public ReactiveResolution SelectedResolution 
    { 
        get => _selectedResolution;
        set => this.RaiseAndSetIfChanged(ref _selectedResolution, value); 
    }

    private ReactiveAudioDevice _selectedInputDevice;
    public ReactiveAudioDevice SelectedInputDevice 
    { 
        get => _selectedInputDevice; 
        set => this.RaiseAndSetIfChanged(ref _selectedInputDevice, value); 
    }

    private ReactiveAudioDevice _selectedOutputDevice;
    public ReactiveAudioDevice SelectedOutputDevice 
    { 
        get => _selectedOutputDevice; 
        set => this.RaiseAndSetIfChanged(ref _selectedOutputDevice, value); 
    }
    
    private ReactiveGeneralSettings _generalSettings;
    public ReactiveGeneralSettings GeneralSettings 
    { 
        get => _generalSettings; 
        set => this.RaiseAndSetIfChanged(ref _generalSettings, value); 
    }
    
    private ReactiveRecordingSettings _recordingSettings;
    public ReactiveRecordingSettings RecordingSettings 
    { 
        get => _recordingSettings; 
        set => this.RaiseAndSetIfChanged(ref _recordingSettings, value); 
    }
    
    private bool _launchAtStartup;
    public bool LaunchAtStartup 
    { 
        get => _launchAtStartup; 
        set => this.RaiseAndSetIfChanged(ref _launchAtStartup, value); 
    }

    public ReactiveCommand<Unit, Unit> BrowseDirectoryCommand { get; private set; }
    
    public SettingsViewModel(ILogger<SettingsViewModel> logger, INavigationService navigationService, 
        ISettingsRepository settingsRepository, IRecorder recorder, IStartupManager startupManager) 
        : base(navigationService)
    {
        _logger = logger;
        _navigationService = navigationService;
        _settingsRepository = settingsRepository;
        _recorder = recorder;
        _startupManager = startupManager;

        BrowseDirectoryCommand = ReactiveCommand.Create(() =>
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                RecordingSettings!.RecordingsLocalPath = dialog.SelectedPath;
            }
        });

        this.WhenActivated(disposables =>
        {
            // General Settings Update Observers
            this.WhenAnyValue(vm => vm.LaunchAtStartup)
                .Skip(1)
                .InvokeCommand(ReactiveCommand.Create<bool>(launchAtStartup =>
                {
                    SetStartupPolicy(launchAtStartup);
                }))
                .DisposeWith(disposables);

            this._startupManager.IsStartupEnabledObservable
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(isStartupEnabled =>
                {
                    LaunchAtStartup = isStartupEnabled;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.GeneralSettings.OnCloseKeepAlive)
                .Skip(1)
                .Select(_ => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask<Unit>(async _ => 
                    await UpdateGeneralSettings(GeneralSettings!)))
                .DisposeWith(disposables);

            // Recording Settings Update Observers
            this.WhenAnyValue(
                    vm => vm.RecordingSettings.RecordingsLocalPath, 
                    vm => vm.RecordingSettings.Fps, 
                    vm => vm.RecordingSettings.Quality, 
                    vm => vm.RecordingSettings.IsInputDeviceEnabled)
                .Skip(1)
                .InvokeCommand(ReactiveCommand.CreateFromTask<(string, int, int, bool), bool>(_ => 
                    UpdateRecordingSettings(RecordingSettings!)))
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.SelectedInputDevice.FriendlyName,
                    vm => vm.SelectedOutputDevice.FriendlyName)
                .Skip(1)
                .InvokeCommand(ReactiveCommand.CreateFromTask<(string, string)>(async audioDevicesFriendlyNames =>
                {
                    var selectedInputDevice = InputAudioDevices!
                        .First(inputDevice => inputDevice.FriendlyName == audioDevicesFriendlyNames.Item1);
                    var selectedOutputDevice = OutputAudioDevices!
                        .First(outputDevice => outputDevice.FriendlyName == audioDevicesFriendlyNames.Item2);

                    SelectedInputDevice!.DeviceName = selectedInputDevice.DeviceName;
                    SelectedOutputDevice!.DeviceName = selectedOutputDevice.DeviceName;

                    RecordingSettings!.InputDeviceFriendlyName = selectedInputDevice.FriendlyName;
                    RecordingSettings.InputDeviceName = selectedInputDevice.DeviceName;
                    RecordingSettings.OutputDeviceFriendlyName = selectedOutputDevice.FriendlyName;
                    RecordingSettings.OutputDeviceName = selectedOutputDevice.DeviceName;

                    await UpdateRecordingSettings(RecordingSettings);
                }))
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.SelectedResolution)
                .Skip(1)
                .InvokeCommand(ReactiveCommand.CreateFromTask<ReactiveResolution>(async resolution =>
                {
                    RecordingSettings.OutputFrameWidth = resolution.Width;
                    RecordingSettings.OutputFrameHeight = resolution.Height;

                    await UpdateRecordingSettings(RecordingSettings);
                }))
                .DisposeWith(disposables);

            Observable.FromAsync(FetchSettings)
                .Subscribe()
                .DisposeWith(disposables);

            _disposables.DisposeWith(disposables);
        });
    }

    private void SetStartupPolicy(bool startWithWindows)
    {
        // If the process fails we don't want to try again automatically
        var modifiedSuccessfully = _startupManager.ModifyLaunchOnStartup(startWithWindows);

        if (modifiedSuccessfully == false) return;

        LaunchAtStartup = _startupManager.IsStartupEnabled();
    }

    private async Task FetchSettings()
    {
        var fetchGeneralSettingsTask = _settingsRepository.GetGeneralSettings();
        var fetchRecordingSettingsTask = _settingsRepository.GetRecordingSettings();

        await Task.WhenAll(fetchGeneralSettingsTask, fetchRecordingSettingsTask);

        if (fetchGeneralSettingsTask.Result != null && fetchRecordingSettingsTask.Result != null)
        {
            GeneralSettings = fetchGeneralSettingsTask.Result;
            RecordingSettings = fetchRecordingSettingsTask.Result;
        }

        // General Settings
        LaunchAtStartup = _startupManager.IsStartupEnabled();

        // Recording Settings
        SupportedScreenResolutions =
            new ObservableCollection<ReactiveResolution>(GetSupportedResolutions());

        var storedResolution = new ReactiveResolution
        {
            Width = RecordingSettings.OutputFrameWidth,
            Height = RecordingSettings.OutputFrameHeight,
        };

        SelectedResolution = SupportedScreenResolutions.Any(res => res.StringValue == storedResolution.StringValue) 
            ? storedResolution 
            : SupportedScreenResolutions.Last();

        InputAudioDevices = new ObservableCollection<ReactiveAudioDevice>
        {
            new ReactiveAudioDevice
            {
                FriendlyName = "Default",
                DeviceName = "Default"
            }
        };
        OutputAudioDevices = new ObservableCollection<ReactiveAudioDevice>
        {
            new ReactiveAudioDevice
            {
                FriendlyName = "Default",
                DeviceName = "Default"
            }
        };

        InputAudioDevices.AddRange(_recorder.GetInputAudioDevices());
        OutputAudioDevices.AddRange(_recorder.GetOutputAudioDevices());

        SelectedInputDevice = InputAudioDevices.FirstOrDefault(inputDevice =>
            inputDevice.FriendlyName == RecordingSettings.InputDeviceFriendlyName &&
            inputDevice.DeviceName == RecordingSettings.InputDeviceName) ?? new ReactiveAudioDevice 
            { DeviceName = "Default", FriendlyName = "Default" };

        SelectedOutputDevice = OutputAudioDevices.FirstOrDefault(outputDevice =>
            outputDevice.FriendlyName == RecordingSettings.OutputDeviceFriendlyName &&
            outputDevice.DeviceName == RecordingSettings.OutputDeviceName) ?? new ReactiveAudioDevice 
            { DeviceName = "Default", FriendlyName = "Default" };
    }

    private async Task<bool> UpdateGeneralSettings(ReactiveGeneralSettings newSettings)
    {
        return await _settingsRepository.SetGeneralSettings(newSettings);
    }

    private async Task<bool> UpdateRecordingSettings(ReactiveRecordingSettings newSettings)
    {
        return await _settingsRepository.SetRecordingSettings(newSettings);
    }

    #region Get Supported Screen Resolutions

    [DllImport("user32.dll")]
    public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
    }

    public static List<ReactiveResolution> GetSupportedResolutions()
    {
        DEVMODE devMode = new DEVMODE();
        devMode.dmSize = (short)Marshal.SizeOf(devMode);
        int modeNum = 0;

        var supportedDimensions = new List<ReactiveResolution>();

        while (EnumDisplaySettings(null, modeNum, ref devMode) != 0)
        {
            modeNum++;

            if (supportedDimensions.Any(res => 
                    res.Width == devMode.dmPelsWidth &&
                    res.Height == devMode.dmPelsHeight))
            {
                continue;
            }

            supportedDimensions.Add(new ReactiveResolution
            {
                Width = devMode.dmPelsWidth,
                Height = devMode.dmPelsHeight,
            });
        }

        return supportedDimensions;
    }

    #endregion
}
