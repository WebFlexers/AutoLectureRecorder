using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;

public interface ISettingsRepository
{
    /// <summary>
    /// Fetches all settings related to recording
    /// </summary>
    Task<ReactiveRecordingSettings?> GetRecordingSettings();

    /// <summary>
    /// Fetches general settings like show at startup, or on close keep alive
    /// </summary>
    Task<ReactiveGeneralSettings?> GetGeneralSettings();

    /// <summary>
    /// Replaces the current recording settings with the provided settings
    /// </summary>
    /// <returns>Whether the operation was successful or not</returns>
    Task<bool> SetRecordingSettings(ReactiveRecordingSettings recordingSettings);

    /// <summary>
    /// Replaces the current general settings with the provided settings
    /// </summary>
    /// <returns>Whether the operation was successful or not</returns>
    Task<bool> SetGeneralSettings(ReactiveGeneralSettings generalSettings);

    /// <summary>
    /// Resets the current recording settings with the default values
    /// </summary>
    Task<bool> ResetRecordingSettings(int primaryScreenWidth, int primaryScreenHeight);

    /// <summary>
    /// Resets the current general settings with the default values
    /// </summary>
    Task<bool> ResetGeneralSettings();

    /// <summary>
    /// Resets all the settings to their default values
    /// </summary>
    Task<bool> ResetAllSettings(int primaryScreenWidth, int primaryScreenHeight);
}