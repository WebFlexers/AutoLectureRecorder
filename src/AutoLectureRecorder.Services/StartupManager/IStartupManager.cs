namespace AutoLectureRecorder.Services.StartupManager;

public interface IStartupManager
{
    /// <summary>
    /// Determines whether the app is configured to run on windows startup
    /// </summary>
    bool IsStartupLaunchEnabled(string appName);
    /// <summary>
    /// Creates a shortcut of the app in the startup folder. As file name
    /// provide only the name of the app and not the extension
    /// </summary>
    bool ModifyLaunchOnStartup(string fileDirectory, string fileName, bool shouldLaunchOnStartup);
}