namespace AutoLectureRecorder.Application.Common.Abstractions.StartupManager;

public interface IStartupManager
{
    IObservable<bool> IsStartupEnabledObservable { get; }

    /// <summary>
    /// Determines whether the app is configured to run on windows startup
    /// </summary>
    bool IsStartupEnabled();

    /// <summary>
    /// Creates a shortcut of the app by modifying the registry
    /// </summary>
    /// <returns>Whether the operation was completed successfully</returns>
    bool ModifyLaunchOnStartup(bool shouldLaunchOnStartup);
}