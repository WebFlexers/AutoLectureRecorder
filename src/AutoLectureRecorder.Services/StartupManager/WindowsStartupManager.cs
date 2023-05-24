using AutoLectureRecorder.Services.ShortcutManager;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Services.StartupManager;

public class WindowsStartupManager : IStartupManager
{
    private readonly ILogger<WindowsStartupManager> _logger;
    private readonly IShortcutManager _shortcutManager;

    public WindowsStartupManager(ILogger<WindowsStartupManager> logger, IShortcutManager shortcutManager)
    {
        _logger = logger;
        _shortcutManager = shortcutManager;
    }

    /// <summary>
    /// Determines whether the app is configured to run on windows startup. Only pass
    /// the app name and not the extension.
    /// </summary>
    public bool IsStartupLaunchEnabled(string appName)
    {
        var startupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        var shortcutPath = Path.Combine(startupDirectory, $"{appName}.lnk");

        var shorcutExists = File.Exists(shortcutPath);
        if (shorcutExists == false) return false;

        FileAttributes attributes = File.GetAttributes(shortcutPath);

        return attributes.HasFlag(FileAttributes.Hidden) == false;
    }

    /// <summary>
    /// Creates a shortcut of the app in the startup folder. As file name
    /// provide only the name of the app and not the extension
    /// </summary>
    public bool ModifyLaunchOnStartup(string appDirectory, string appName, bool shouldLaunchOnStartup)
    {
        try
        {
            var isStartupLaunchEnabled = IsStartupLaunchEnabled(appName);
            if (isStartupLaunchEnabled && shouldLaunchOnStartup) return true;
            if (isStartupLaunchEnabled == false && shouldLaunchOnStartup == false) return true;

            var targetPath = Path.Combine(appDirectory, $"{appName}.exe");
            var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                $"{appName}.lnk");

            if (shouldLaunchOnStartup)
            {
                _shortcutManager.CreateShortcut(targetPath, shortcutPath);
                RemoveHiddenFlag(shortcutPath);
                return true;
            }
            else
            {
                if (isStartupLaunchEnabled == false) return true;

                AddHiddenFlag(shortcutPath);
                return true;
            }
        }
        catch (Exception ex)
        {
            var modificationType = shouldLaunchOnStartup ? "enable" : "disable";
            _logger.LogError(ex, @"An error occurred while trying to {modificationType} Launch On Startup 
                for file directory: {directory} and file name {fileName}", modificationType, appDirectory, appName);
            return false;
        }
    }

    /// <summary>
    /// Removes the hidden flag to enable startup
    /// </summary>
    private void RemoveHiddenFlag(string filePath)
    {
        FileAttributes attributes = File.GetAttributes(filePath);
        
        if (attributes.HasFlag(FileAttributes.Hidden))
        {
            attributes &= ~FileAttributes.Hidden;
            File.SetAttributes(filePath, attributes);
        }
    }

    /// <summary>
    /// Adds the hidden flag to disable startup
    /// </summary>
    private void AddHiddenFlag(string filePath)
    {
        FileAttributes attributes = File.GetAttributes(filePath);

        if (attributes.HasFlag(FileAttributes.Hidden) == false)
        {
            attributes |= FileAttributes.Hidden;
            File.SetAttributes(filePath, attributes);
        }
    }
}
