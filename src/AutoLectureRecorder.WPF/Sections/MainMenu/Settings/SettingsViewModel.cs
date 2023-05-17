using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AutoLectureRecorder.Sections.MainMenu.Settings;

public class SettingsViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    private readonly CompositeDisposable _disposables = new();

    private readonly ILogger<SettingsViewModel> _logger;
    private readonly ISettingsRepository _settingsRepository;
    public string UrlPathSegment => nameof(SettingsViewModel);
    public IScreen HostScreen { get; }

    [Reactive] 
    public ReactiveGeneralSettings GeneralSettings { get; set; }
    [Reactive] 
    public ReactiveRecordingSettings RecordingSettings { get; set; }

    public SettingsViewModel(ILogger<SettingsViewModel> logger, IScreenFactory screenFactory, ISettingsRepository settingsRepository)
    {
        _logger = logger;
        _settingsRepository = settingsRepository;
        HostScreen = screenFactory.GetMainMenuViewModel();

        Observable.FromAsync(FetchSettings)
            .Subscribe().DisposeWith(_disposables);

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(vm => vm.GeneralSettings.LaunchAtStartup, vm => vm.GeneralSettings.OnCloseKeepAlive,
                vm => vm.GeneralSettings.ShowSplashScreen)
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Do(_ => _logger.LogDebug("A value changed"))
                .InvokeCommand(ReactiveCommand.CreateFromTask<(bool, bool, bool), bool>(_ => UpdateGeneralSettings(GeneralSettings!)))
                .DisposeWith(disposables);

            _disposables.DisposeWith(disposables);
        });
    }

    public async Task FetchSettings()
    {
        var fetchGeneralSettingsTask = _settingsRepository.GetGeneralSettings();
        var fetchRecordingSettingsTask = _settingsRepository.GetRecordingSettings();

        await Task.WhenAll(fetchGeneralSettingsTask, fetchRecordingSettingsTask);

        if (fetchGeneralSettingsTask.Result != null && fetchRecordingSettingsTask.Result != null)
        {
            GeneralSettings = fetchGeneralSettingsTask.Result;
            RecordingSettings = fetchRecordingSettingsTask.Result;
        }
    }

    public async Task<bool> UpdateGeneralSettings(ReactiveGeneralSettings newSettings)
    {
        return await _settingsRepository.SetGeneralSettings(newSettings);
    }

    public async Task<bool> UpdateRecordingSettings(ReactiveRecordingSettings newSettings)
    {
        return await _settingsRepository.SetRecordingSettings(newSettings);
    }
}
