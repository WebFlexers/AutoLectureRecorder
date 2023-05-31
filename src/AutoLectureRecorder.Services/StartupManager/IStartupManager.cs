namespace AutoLectureRecorder.Services.StartupManager;

public interface IStartupManager
{
    /// <summary>
    /// Observes changes in startup behaviour
    /// </summary>
    /// <returns></returns>
    IObservable<bool> IsStartupEnabledObservable { get; }

    /// <summary>
    /// Determines whether the app is configured to run on windows startup
    /// </summary>
    bool IsStartupEnabled();

    /// <summary>
    /// Creates a shortcut of the app in the startup folder
    /// </summary>
    /// <returns>Whether the operation was completed successfuly</returns>
    bool ModifyLaunchOnStartup(bool shouldLaunchOnStartup);
}