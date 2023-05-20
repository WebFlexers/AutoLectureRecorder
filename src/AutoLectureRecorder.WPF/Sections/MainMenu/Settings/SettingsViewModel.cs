using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using AutoLectureRecorder.Services.Recording;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
using DynamicData;
using OpenQA.Selenium.DevTools.V111.WebAudio;

namespace AutoLectureRecorder.Sections.MainMenu.Settings;

public class SettingsViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    private readonly CompositeDisposable _disposables = new();

    private readonly ILogger<SettingsViewModel> _logger;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IRecorder _recorder;
    public string UrlPathSegment => nameof(SettingsViewModel);
    public IScreen HostScreen { get; }

    public ObservableCollection<ReactiveAudioDevice> InputAudioDevices { get; set; }
    public ObservableCollection<ReactiveAudioDevice> OutputAudioDevices { get; set; }
    public ObservableCollection<ReactiveResolution> SupportedScreenResolutions { get; set; }

    [Reactive]
    public ReactiveResolution SelectedResolution { get; set; }
    [Reactive]
    public ReactiveAudioDevice? SelectedInputDevice { get; set; }
    [Reactive]
    public ReactiveAudioDevice? SelectedOutputDevice { get; set; }
    [Reactive] 
    public ReactiveGeneralSettings GeneralSettings { get; set; }
    [Reactive] 
    public ReactiveRecordingSettings RecordingSettings { get; set; }

    public ReactiveCommand<Unit, Unit> BrowseDirectoryCommand { get; private set; }
    

    public SettingsViewModel(ILogger<SettingsViewModel> logger, IScreenFactory screenFactory, ISettingsRepository settingsRepository,
        IRecorder recorder)
    {
        _logger = logger;
        _settingsRepository = settingsRepository;
        _recorder = recorder;
        HostScreen = screenFactory.GetMainMenuViewModel();

        Observable.FromAsync(InitializeSettings)
            .Subscribe().DisposeWith(_disposables);

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
            this.WhenAnyValue(vm => vm.GeneralSettings.LaunchAtStartup, vm => vm.GeneralSettings.OnCloseKeepAlive,
                vm => vm.GeneralSettings.ShowSplashScreen)
                .InvokeCommand(ReactiveCommand.CreateFromTask<(bool, bool, bool), bool>(_ => UpdateGeneralSettings(GeneralSettings!)))
                .DisposeWith(disposables);

            // Recording Settings Update Observers
            this.WhenAnyValue(vm => vm.RecordingSettings.RecordingsLocalPath, vm => vm.RecordingSettings.Fps, 
                    vm => vm.RecordingSettings.Quality, vm => vm.RecordingSettings.IsInputDeviceEnabled)
                .InvokeCommand(ReactiveCommand.CreateFromTask<(string, int, int, bool), bool>(_ => UpdateRecordingSettings(RecordingSettings!)))
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.SelectedInputDevice.FriendlyName,
                    vm => vm.SelectedOutputDevice.FriendlyName)
                .InvokeCommand(ReactiveCommand.CreateFromTask<(string, string)>(async audioDevicesFriendlyNames =>
                {
                    var selectedInputDevice = InputAudioDevices!
                        .First(inputDevice => inputDevice.FriendlyName == audioDevicesFriendlyNames.Item1);
                    var selectedOutputDevice = OutputAudioDevices!
                        .First(outputDevice => outputDevice.FriendlyName == audioDevicesFriendlyNames.Item2);

                    SelectedInputDevice!.DeviceName = selectedInputDevice.DeviceName;
                    SelectedOutputDevice!.DeviceName = SelectedOutputDevice.DeviceName;

                    RecordingSettings!.InputDeviceFriendlyName = selectedInputDevice.FriendlyName;
                    RecordingSettings.InputDeviceName = selectedInputDevice.DeviceName;
                    RecordingSettings.OutputDeviceFriendlyName = selectedOutputDevice.FriendlyName;
                    RecordingSettings.OutputDeviceName = selectedOutputDevice.DeviceName;

                    await UpdateRecordingSettings(RecordingSettings);
                }))
                .DisposeWith(disposables);

            this.WhenAnyValue(vm => vm.SelectedResolution)
                .InvokeCommand(ReactiveCommand.CreateFromTask<ReactiveResolution>(async resolution =>
                {
                    if (RecordingSettings == null) return;

                    RecordingSettings.OutputFrameWidth = resolution.Width;
                    RecordingSettings.OutputFrameHeight = resolution.Height;

                    await UpdateRecordingSettings(RecordingSettings);
                }))
                .DisposeWith(disposables);

            _disposables.DisposeWith(disposables);
        });
    }

    public async Task InitializeSettings()
    {
        var fetchGeneralSettingsTask = _settingsRepository.GetGeneralSettings();
        var fetchRecordingSettingsTask = _settingsRepository.GetRecordingSettings();

        await Task.WhenAll(fetchGeneralSettingsTask, fetchRecordingSettingsTask);

        if (fetchGeneralSettingsTask.Result != null && fetchRecordingSettingsTask.Result != null)
        {
            GeneralSettings = fetchGeneralSettingsTask.Result;
            RecordingSettings = fetchRecordingSettingsTask.Result;
        }

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
            inputDevice.DeviceName == RecordingSettings.InputDeviceName);

        SelectedOutputDevice = OutputAudioDevices.FirstOrDefault(outputDevice =>
            outputDevice.FriendlyName == RecordingSettings.OutputDeviceFriendlyName &&
            outputDevice.DeviceName == RecordingSettings.OutputDeviceName);

        SelectedInputDevice ??= InputAudioDevices.First(inputDevice => inputDevice.FriendlyName == "Default");
        SelectedOutputDevice ??= OutputAudioDevices.First(outputDevice => outputDevice.FriendlyName == "Default");
    }

    public async Task<bool> UpdateGeneralSettings(ReactiveGeneralSettings newSettings)
    {
        return await _settingsRepository.SetGeneralSettings(newSettings);
    }

    public async Task<bool> UpdateRecordingSettings(ReactiveRecordingSettings newSettings)
    {
        return await _settingsRepository.SetRecordingSettings(newSettings);
    }

    #region Get Supported Resolutions

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
