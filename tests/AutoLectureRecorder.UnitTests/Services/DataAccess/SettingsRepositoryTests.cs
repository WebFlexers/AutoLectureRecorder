using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Repositories;
using AutoLectureRecorder.Services.Recording;
using AutoLectureRecorder.UnitTests.Fixture;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

[Collection("DatabaseCollection")]
public class SettingsRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<SettingsRepository> _logger;

    public SettingsRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _output = output;
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<SettingsRepository>(output);
    }

    [Fact]
    public async Task GetRecordingSettings_ShouldFetchSuccessfully()
    {
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger, DataAccessMockHelper.CreateConfiguration());

        var windowsRecorder = new WindowsRecorder(null);
        var expectedRecordingSettings = windowsRecorder.GetDefaultSettings(1920, 1080);

        var actualRecordingSettings = await settingsRepository.GetRecordingSettings();

        Assert.Equal(JsonConvert.SerializeObject(expectedRecordingSettings), 
            JsonConvert.SerializeObject(actualRecordingSettings));
    }

    [Fact]
    public async Task GetGeneralSettings_ShouldFetchSuccessfully()
    {
        var config = DataAccessMockHelper.CreateConfiguration();
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger, config);

        var expectedGeneralSettings = new ReactiveGeneralSettings
        {
            OnCloseKeepAlive = true,
            ShowSplashScreen = true
        };

        var actualGeneralSettings = await settingsRepository.GetGeneralSettings();

        Assert.Equal(JsonConvert.SerializeObject(expectedGeneralSettings), 
            JsonConvert.SerializeObject(actualGeneralSettings));
    }

    [Fact]
    public async Task SetRecordingSettings_ShouldUpdateTheRecordingSettings()
    {
        await _fixture.DataAccess.BeginTransaction();

        var config = DataAccessMockHelper.CreateConfiguration();
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger, config);

        var expectedNewSettings = new ReactiveRecordingSettings
        {
            RecordingsLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
            OutputDeviceName = "fjskierfjw4oijfd",
            OutputDeviceFriendlyName = "Audient ID4 - Output",
            InputDeviceName = "dfnbseurhvso4iv",
            InputDeviceFriendlyName = "Audient ID4 - Input",
            IsInputDeviceEnabled = false,
            Quality = 100,
            Fps = 60, 
            OutputFrameWidth = 1280,
            OutputFrameHeight = 720
        };

        var result = await settingsRepository.SetRecordingSettings(expectedNewSettings);

        string fetchSql = "select * from RecordingSettings";
        var actualNewSettings = SettingsRepository.ConvertRecordingSettingsToReactive(
            (await _fixture.DataAccess.LoadData<RecordingSettings, dynamic>(fetchSql, new { })).First());
        
        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.True(result);
        Assert.Equal(JsonConvert.SerializeObject(expectedNewSettings), 
            JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task SetGeneralSettings_ShouldUpdateTheGeneralSettings()
    {
        await _fixture.DataAccess.BeginTransaction();

        var config = DataAccessMockHelper.CreateConfiguration();
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger, config);

        var expectedNewSettings = new ReactiveGeneralSettings
        {
            OnCloseKeepAlive = false,
            ShowSplashScreen = true
        };

        var result = await settingsRepository.SetGeneralSettings(expectedNewSettings);

        string fetchSql = "select * from GeneralSettings";
        var actualNewSettings = SettingsRepository.ConvertGeneralSettingsToReactive(
            (await _fixture.DataAccess.LoadData<GeneralSettings, dynamic>(fetchSql, new { })).First());
        
        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.True(result);
        Assert.Equal(JsonConvert.SerializeObject(expectedNewSettings), 
            JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task ResetRecordingSettings_ShouldRestoreSettingsToDefault()
    {
        await _fixture.DataAccess.BeginTransaction();

        var config = DataAccessMockHelper.CreateConfiguration();
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger, config);

        var windowsRecorder = new WindowsRecorder(null);
        var expectedNewSettings = windowsRecorder.GetDefaultSettings(1920, 1080);

        bool result = await settingsRepository.ResetRecordingSettings(1920, 1080, windowsRecorder);

        string fetchSql = "select * from RecordingSettings";
        var actualNewSettings = SettingsRepository.ConvertRecordingSettingsToReactive(
            (await _fixture.DataAccess.LoadData<RecordingSettings, dynamic>(fetchSql, new { })).First());

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.True(result);
        Assert.Equal(JsonConvert.SerializeObject(expectedNewSettings), 
            JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task ResetGeneralSettings_ShouldRestoreSettingsToDefault()
    {
        await _fixture.DataAccess.BeginTransaction();

        var config = DataAccessMockHelper.CreateConfiguration();
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger, config);

        var expectedNewSettings = new GeneralSettings
        {
            OnCloseKeepAlive = 1,
            ShowSplashScreen = 1
        };

        bool result = await settingsRepository.ResetGeneralSettings();

        string fetchSql = "select * from GeneralSettings";
        var actualNewSettings = (await _fixture.DataAccess.LoadData<GeneralSettings, dynamic>(fetchSql, new { })).First();

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.True(result);
        Assert.Equal(JsonConvert.SerializeObject(expectedNewSettings), 
            JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task ResetAllSettings_ShouldRestoreAllSettingsToDefault()
    {
        await _fixture.DataAccess.BeginTransaction();

        var config = DataAccessMockHelper.CreateConfiguration();
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger, config);

        var windowsRecorder = new WindowsRecorder(null);
        var expectedRecordingSettings = windowsRecorder.GetDefaultSettings(1920, 1080);
        var expectedGeneralSettings = new GeneralSettings
        {
            OnCloseKeepAlive = 1,
            ShowSplashScreen = 1
        };

        bool result = await settingsRepository.ResetAllSettings(1920, 1080, windowsRecorder);

        string fetchRecordingSettingsSql = "select * from RecordingSettings";
        var actualRecordingSettings = SettingsRepository.ConvertRecordingSettingsToReactive(
            (await _fixture.DataAccess.LoadData<RecordingSettings, dynamic>(fetchRecordingSettingsSql, new { })).First());

        string fetchGeneralSettingsSql = "select * from GeneralSettings";
        var actualGeneralSettings = (await _fixture.DataAccess.LoadData<GeneralSettings, dynamic>(fetchGeneralSettingsSql, new { })).First();

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.True(result);
        Assert.Equal(JsonConvert.SerializeObject(expectedRecordingSettings), 
            JsonConvert.SerializeObject(actualRecordingSettings));
        Assert.Equal(JsonConvert.SerializeObject(expectedGeneralSettings), 
            JsonConvert.SerializeObject(actualGeneralSettings));
    }
}
