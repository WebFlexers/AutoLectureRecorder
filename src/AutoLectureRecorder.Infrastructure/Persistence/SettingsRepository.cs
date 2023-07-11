using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;
using AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Infrastructure.Persistence;

public class SettingsRepository : ISettingsRepository
{
    private readonly ISqliteDataAccess _dataAccess;
    private readonly ILogger<SettingsRepository>? _logger;
    
    public SettingsRepository(ISqliteDataAccess dataAccess, ILogger<SettingsRepository> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
    }

    /// <summary>
    /// Fetches all settings related to recording
    /// </summary>
    public async Task<ReactiveRecordingSettings?> GetRecordingSettings()
    {
        try
        {
            string sql = @"select * from RecordingSettings";

            var recordingSettings = await _dataAccess.LoadData<RecordingSettings, dynamic>(
                    sql, new { }).ConfigureAwait(false);

            return recordingSettings.First().MapToReactive();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while trying to fetch recording settings");
            return null;
        }
    }

    /// <summary>
    /// Fetches all the general settings
    /// </summary>
    public async Task<ReactiveGeneralSettings?> GetGeneralSettings()
    {
        try
        {
            string sql = @"select * from GeneralSettings";

            var generalSettings = await _dataAccess.LoadData<GeneralSettings, dynamic>(
                    sql, new { })
                .ConfigureAwait(false);

            return generalSettings.First().MapToReactive();
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

            sql = @"insert into RecordingSettings (RecordingsLocalPath, OutputDeviceName, OutputDeviceFriendlyName, 
                               InputDeviceName, InputDeviceFriendlyName, IsInputDeviceEnabled, Quality, Fps, 
                               OutputFrameWidth, OutputFrameHeight) 
                    values (@RecordingsLocalPath, @OutputDeviceName, @OutputDeviceFriendlyName, 
                            @InputDeviceName, @InputDeviceFriendlyName, @IsInputDeviceEnabled, @Quality, @Fps, 
                            @OutputFrameWidth, @OutputFrameHeight)";
            await _dataAccess.SaveData(sql, recordingSettings.MapToSqliteModel());

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

            sql = @"insert into GeneralSettings (OnCloseKeepAlive) 
                    values (@OnCloseKeepAlive)";
            await _dataAccess.SaveData(sql, generalSettings.MapToSqliteModel());

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
    public async Task<bool> ResetRecordingSettings(int primaryScreenWidth, int primaryScreenHeight, IRecorder recorder)
    {
        ReactiveRecordingSettings defaultSettings = recorder.GetDefaultSettings(primaryScreenWidth, primaryScreenHeight);
        return await SetRecordingSettings(defaultSettings);
    }

    /// <summary>
    /// Resets the current general settings with the default values
    /// </summary>
    public async Task<bool> ResetGeneralSettings()
    {
        var defaultSettings = new ReactiveGeneralSettings(onCloseKeepAlive: true);
        return await SetGeneralSettings(defaultSettings);
    }

    /// <summary>
    /// Resets all the settings (both recording and general) to their default values
    /// </summary>
    public async Task<bool> ResetAllSettings(int primaryScreenWidth, int primaryScreenHeight, IRecorder recorder)
    {
        var resetTasks = new List<Task<bool>>
        {
            ResetRecordingSettings(primaryScreenWidth, primaryScreenHeight, recorder),
            ResetGeneralSettings()
        };

        await Task.WhenAll(resetTasks);

        bool allTasksSucceeded = resetTasks.All(t => t.Result);
        return allTasksSucceeded;
    }
}
