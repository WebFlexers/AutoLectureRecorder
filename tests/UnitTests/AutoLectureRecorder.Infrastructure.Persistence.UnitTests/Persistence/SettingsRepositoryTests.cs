using AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;
using AutoLectureRecorder.Infrastructure.Persistence.UnitTests.TestUtils.Fixture;
using AutoLectureRecorder.Infrastructure.Recording;
using CommonTestsUtils.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace AutoLectureRecorder.Infrastructure.Persistence.UnitTests.Persistence;

[Collection("DatabaseCollection")]
public class SettingsRepositoryTests
{
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<SettingsRepository> _logger;

    public SettingsRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<SettingsRepository>(output);
    }

    [Fact]
    public async Task GetRecordingSettings_ShouldFetchSuccessfully()
    {
        // Arrange
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger);

        var windowsRecorder = new WindowsRecorder(null);
        var expectedRecordingSettings = windowsRecorder.GetDefaultSettings(1920, 1080);

        // Act
        var actualRecordingSettings = await settingsRepository.GetRecordingSettings();

        // Assert
        JsonConvert.SerializeObject(expectedRecordingSettings)
            .Should().Be(JsonConvert.SerializeObject(actualRecordingSettings));
    }

    [Fact]
    public async Task GetGeneralSettings_ShouldFetchSuccessfully()
    {
        // Arrange
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger);

        var expectedGeneralSettings = new ReactiveGeneralSettings
        (
            onCloseKeepAlive: true
        );

        // Act
        var actualGeneralSettings = await settingsRepository.GetGeneralSettings();

        // Assert
        JsonConvert.SerializeObject(expectedGeneralSettings)
            .Should().Be(JsonConvert.SerializeObject(actualGeneralSettings));
    }

    [Fact]
    public async Task SetRecordingSettings_ShouldUpdateTheRecordingSettings()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger);

        var expectedNewSettings = new ReactiveRecordingSettings
        (
            recordingsLocalPath: Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
            outputDeviceName: "fjskierfjw4oijfd",
            outputDeviceFriendlyName: "Audient ID4 - Output",
            inputDeviceName: "dfnbseurhvso4iv",
            inputDeviceFriendlyName: "Audient ID4 - Input",
            isInputDeviceEnabled: false,
            quality: 100,
            fps: 60, 
            outputFrameWidth: 1280,
            outputFrameHeight: 720
        );

        // Act
        var result = await settingsRepository.SetRecordingSettings(expectedNewSettings);

        string fetchSql = "select * from RecordingSettings";
        var actualNewSettings = (await _fixture.DataAccess.LoadData<RecordingSettings, dynamic>(
            fetchSql, new { })).First().MapToReactive();
        
        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        result.Should().BeTrue();
        JsonConvert.SerializeObject(expectedNewSettings)
            .Should().Be(JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task SetGeneralSettings_ShouldUpdateTheGeneralSettings()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger);

        var expectedNewSettings = new ReactiveGeneralSettings
        (
            onCloseKeepAlive: false
        );

        // Act
        var result = await settingsRepository.SetGeneralSettings(expectedNewSettings);

        string fetchSql = "select * from GeneralSettings";
        var actualNewSettings = (await _fixture.DataAccess.LoadData<GeneralSettings, dynamic>(
            fetchSql, new { })).First().MapToReactive();
        
        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        result.Should().BeTrue();
        JsonConvert.SerializeObject(expectedNewSettings)
            .Should().Be(JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task ResetRecordingSettings_ShouldRestoreSettingsToDefault()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger);

        var windowsRecorder = new WindowsRecorder(null);
        var expectedNewSettings = windowsRecorder.GetDefaultSettings(1920, 1080);

        // Act
        bool result = await settingsRepository.ResetRecordingSettings(1920, 1080, windowsRecorder);

        string fetchSql = "select * from RecordingSettings";
        var actualNewSettings = (await _fixture.DataAccess.LoadData<RecordingSettings, dynamic>(
            fetchSql, new { })).First().MapToReactive();

        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        result.Should().BeTrue();
        JsonConvert.SerializeObject(expectedNewSettings)
            .Should().Be(JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task ResetGeneralSettings_ShouldRestoreSettingsToDefault()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger);

        var expectedNewSettings = new GeneralSettings
        {
            OnCloseKeepAlive = 1
        };

        // Act
        bool result = await settingsRepository.ResetGeneralSettings();

        string fetchSql = "select * from GeneralSettings";
        var actualNewSettings = (await _fixture.DataAccess.LoadData<GeneralSettings, dynamic>(
            fetchSql, new { })).First();

        _fixture.DataAccess.RollbackPendingTransaction();

        // Arrange
        result.Should().BeTrue();
        JsonConvert.SerializeObject(expectedNewSettings)
            .Should().Be(JsonConvert.SerializeObject(actualNewSettings));
    }

    [Fact]
    public async Task ResetAllSettings_ShouldRestoreAllSettingsToDefault()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        
        var settingsRepository =
            new SettingsRepository(_fixture.DataAccess, _logger);

        var windowsRecorder = new WindowsRecorder(null);
        var expectedRecordingSettings = windowsRecorder.GetDefaultSettings(1920, 1080);
        var expectedGeneralSettings = new GeneralSettings
        {
            OnCloseKeepAlive = 1,
        };

        // Act
        bool result = await settingsRepository.ResetAllSettings(1920, 1080, windowsRecorder);

        string fetchRecordingSettingsSql = "select * from RecordingSettings";
        var actualRecordingSettings = (await _fixture.DataAccess.LoadData<RecordingSettings, dynamic>(
            fetchRecordingSettingsSql, new { })).First().MapToReactive();

        string fetchGeneralSettingsSql = "select * from GeneralSettings";
        var actualGeneralSettings = (await _fixture.DataAccess.LoadData<GeneralSettings, dynamic>(
            fetchGeneralSettingsSql, new { })).First();

        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        result.Should().BeTrue();
        JsonConvert.SerializeObject(expectedRecordingSettings)
            .Should().Be(JsonConvert.SerializeObject(actualRecordingSettings));
        JsonConvert.SerializeObject(expectedGeneralSettings)
            .Should().Be(JsonConvert.SerializeObject(actualGeneralSettings));
    }
}
