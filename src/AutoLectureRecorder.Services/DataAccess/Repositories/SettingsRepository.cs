using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using AutoLectureRecorder.Services.Recording;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Services.DataAccess.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly ISqliteDataAccess _dataAccess;
    private readonly ILogger<SettingsRepository>? _logger;
    private readonly IConfiguration? _config;

    public SettingsRepository(ISqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public SettingsRepository(ISqliteDataAccess dataAccess, ILogger<SettingsRepository> logger, IConfiguration config)
    {
        _dataAccess = dataAccess;
        _logger = logger;
        _config = config;
    }

    /// <summary>
    /// Fetches all settings related to recording
    /// </summary>
    public async Task<ReactiveRecordingSettings?> GetRecordingSettings()
    {
        try
        {
            string sql = @"select * from RecordingSettings";

            var recordingSettings = await _dataAccess.LoadData<RecordingSettings, dynamic>(sql, new { })
                .ConfigureAwait(false);

            return ConvertRecordingSettingsToReactive(recordingSettings.First());
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while trying to fetch recording settings");
            return null;
        }
    }

    /// <summary>
    /// Fetches general settings like show at startup, or on close keep alive
    /// </summary>
    public async Task<ReactiveGeneralSettings?> GetGeneralSettings()
    {
        try
        {
            string sql = @"select * from GeneralSettings";

            var generalSettings = await _dataAccess.LoadData<GeneralSettings, dynamic>(sql, new { })
                .ConfigureAwait(false);

            return ConvertGeneralSettingsToReactive(generalSettings.First());
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while trying to fetch general settings");
            return null;
        }
    }

    /// <summary>
    /// Replaces the current recording settings with the provided settings
    /// </summary>
    /// <returns>Whether the operation was successful or not</returns>
    public async Task<bool> SetRecordingSettings(ReactiveRecordingSettings recordingSettings)
    {
        try
        {
            string sql = @"delete from RecordingSettings";
            await _dataAccess.SaveData<dynamic>(sql, new { });

            sql = @"insert into RecordingSettings (RecordingsLocalPath, OutputDeviceName, OutputDeviceFriendlyName, InputDeviceName, InputDeviceFriendlyName, 
                                IsInputDeviceEnabled, Quality, Fps, OutputFrameWidth, OutputFrameHeight)
                    values (@RecordingsLocalPath, @OutputDeviceName, @OutputDeviceFriendlyName, @InputDeviceName, @InputDeviceFriendlyName, 
                                @IsInputDeviceEnabled, @Quality, @Fps, @OutputFrameWidth, @OutputFrameHeight)";
            await _dataAccess.SaveData(sql, ConvertReactiveRecordingSettingsToNormal(recordingSettings));

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while trying to set recording settings");
            return false;
        }
    }

    /// <summary>
    /// Replaces the current general settings with the provided settings
    /// </summary>
    /// <returns>Whether the operation was successful or not</returns>
    public async Task<bool> SetGeneralSettings(ReactiveGeneralSettings generalSettings)
    {
        try
        {
            string sql = @"delete from GeneralSettings";
            await _dataAccess.SaveData<dynamic>(sql, new { });

            sql = @"insert into GeneralSettings (LaunchAtStartup, OnCloseKeepAlive, ShowSplashScreen)
                    values (@LaunchAtStartup, @OnCloseKeepAlive, @ShowSplashScreen)";
            await _dataAccess.SaveData(sql, ConvertReactiveGeneralSettingsToNormal(generalSettings));

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while trying to set recording settings");
            return false;
        }
    }

    /// <summary>
    /// Resets the current recording settings with the default values
    /// </summary>
    public async Task<bool> ResetRecordingSettings(int primaryScreenWidth, int primaryScreenHeight)
    {
        ReactiveRecordingSettings defaultSettings = WindowsRecorder.GetDefaultSettings(primaryScreenWidth, primaryScreenHeight);
        return await SetRecordingSettings(defaultSettings);
    }

    /// <summary>
    /// Resets the current general settings with the default values
    /// </summary>
    public async Task<bool> ResetGeneralSettings()
    {
        if (_config == null) return false;

        var generalSettingsSection = _config.GetSection("DefaultGeneralSettings");
        var defaultSettings = new ReactiveGeneralSettings
        {
            LaunchAtStartup = generalSettingsSection.GetValue<bool>("LaunchAtStartup"),
            OnCloseKeepAlive = generalSettingsSection.GetValue<bool>("OnCloseKeepAlive"),
            ShowSplashScreen = generalSettingsSection.GetValue<bool>("ShowSplashScreen")
        };
        return await SetGeneralSettings(defaultSettings);
    }

    /// <summary>
    /// Resets all the settings to their default values
    /// </summary>
    public async Task<bool> ResetAllSettings(int primaryScreenWidth, int primaryScreenHeight)
    {
        var resetTasks = new List<Task<bool>>
        {
            ResetRecordingSettings(primaryScreenWidth, primaryScreenHeight),
            ResetGeneralSettings()
        };

        await Task.WhenAll(resetTasks);

        bool allTasksSucceeded = resetTasks.All(t => t.Result);
        return allTasksSucceeded;
    }

    public static RecordingSettings ConvertReactiveRecordingSettingsToNormal(ReactiveRecordingSettings reactiveRecordingSettings)
    {
        return new RecordingSettings
        {
            RecordingsLocalPath = reactiveRecordingSettings.RecordingsLocalPath,
            OutputDeviceName = reactiveRecordingSettings.OutputDeviceName,
            OutputDeviceFriendlyName = reactiveRecordingSettings.OutputDeviceFriendlyName,
            InputDeviceName = reactiveRecordingSettings.InputDeviceName,
            InputDeviceFriendlyName = reactiveRecordingSettings.InputDeviceFriendlyName,
            IsInputDeviceEnabled = reactiveRecordingSettings.IsInputDeviceEnabled ? 1 : 0,
            Quality = reactiveRecordingSettings.Quality,
            Fps = reactiveRecordingSettings.Fps,
            OutputFrameWidth = reactiveRecordingSettings.OutputFrameWidth,
            OutputFrameHeight = reactiveRecordingSettings.OutputFrameHeight
        };
    }

    public static ReactiveRecordingSettings ConvertRecordingSettingsToReactive(RecordingSettings recordingSettings)
    {
        return new ReactiveRecordingSettings
        {
            RecordingsLocalPath = recordingSettings.RecordingsLocalPath,
            OutputDeviceName = recordingSettings.OutputDeviceName,
            OutputDeviceFriendlyName = recordingSettings.OutputDeviceFriendlyName,
            InputDeviceName = recordingSettings.InputDeviceName,
            InputDeviceFriendlyName = recordingSettings.InputDeviceFriendlyName,
            IsInputDeviceEnabled = recordingSettings.IsInputDeviceEnabled == 1,
            Quality = recordingSettings.Quality,
            Fps = recordingSettings.Fps,
            OutputFrameWidth = recordingSettings.OutputFrameWidth,
            OutputFrameHeight = recordingSettings.OutputFrameHeight
        };
    }

    public static GeneralSettings ConvertReactiveGeneralSettingsToNormal(ReactiveGeneralSettings reactiveGeneralSettings)
    {
        return new GeneralSettings
        {
            LaunchAtStartup = reactiveGeneralSettings.LaunchAtStartup ? 1 : 0,
            OnCloseKeepAlive = reactiveGeneralSettings.OnCloseKeepAlive ? 1 : 0,
            ShowSplashScreen = reactiveGeneralSettings.ShowSplashScreen ? 1 : 0
        };
    }

    public static ReactiveGeneralSettings ConvertGeneralSettingsToReactive(GeneralSettings generalSettings)
    {
        return new ReactiveGeneralSettings
        {
            LaunchAtStartup = generalSettings.LaunchAtStartup == 1,
            OnCloseKeepAlive = generalSettings.OnCloseKeepAlive == 1,
            ShowSplashScreen = generalSettings.ShowSplashScreen == 1
        };
    }
}
