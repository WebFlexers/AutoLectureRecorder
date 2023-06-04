using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reactive.Linq;

namespace AutoLectureRecorder.Services.StartupManager;

#pragma warning disable CA1416 // Validate platform compatibility
public class WindowsStartupManager : IStartupManager
{
    private readonly byte[] StartupEnabledValue = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    private string StartupEnabledValueString => BitConverter.ToString(StartupEnabledValue).Replace("-", "");
    private readonly byte[] StartupDisabledValue = new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    private string StartupDisabledValueString => BitConverter.ToString(StartupDisabledValue).Replace("-", "");
    private const string MainStartupRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string ApprovedStartupRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

    private readonly string _appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nameof(AutoLectureRecorder)}.exe");
    private readonly string _appName;

    private readonly ILogger<WindowsStartupManager> _logger;

    public WindowsStartupManager(ILogger<WindowsStartupManager> logger)
    {
        _logger = logger;

        _appName = Path.GetFileNameWithoutExtension(_appPath);
    }

    public IObservable<bool> IsStartupEnabledObservable =>
        Observable.Interval(TimeSpan.FromMilliseconds(500))
            .Select(_ => IsStartupEnabled())
            .DistinctUntilChanged();
    
    /// <summary>
    /// Determines whether the app is configured to run on windows startup
    /// </summary>
    public bool IsStartupEnabled()
    {
        bool mainStartupRegistryIsSet = false;
        bool isStartupApproved = false;

        // Determine whether the startup has been configured
        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(MainStartupRegistryPath))
        {
            if (key != null)
            {
                string[] startupAppNames = key.GetValueNames();
                foreach (string startupAppName in startupAppNames)
                {
                    if (startupAppName.Equals(_appName, StringComparison.OrdinalIgnoreCase))
                    {
                        mainStartupRegistryIsSet = true;
                        break;
                    }
                }
            }
        }

        // Determine whether startup is enabled or disabled (except for in app changes this
        // also detects changes from other places, like the task manager or windows settings)
        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(ApprovedStartupRegistryPath))
        {
            if (key == null)
            {
                return mainStartupRegistryIsSet;
            }

            string[] startupAppNames = key.GetValueNames();
            foreach (string startupAppName in startupAppNames)
            {
                if (startupAppName.Equals(_appName, StringComparison.OrdinalIgnoreCase) == false) continue;
                
                var value = (byte[])key.GetValue(startupAppName)!;
                if (value.First() == StartupEnabledValue.First())
                {
                    isStartupApproved = true;
                }

                break;
            }
        }

        return mainStartupRegistryIsSet && isStartupApproved;
    }

    /// <summary>
    /// Creates a shortcut of the app in the startup folder
    /// </summary>
    /// <returns>Whether the operation was completed successfuly</returns>
    public bool ModifyLaunchOnStartup(bool shouldLaunchOnStartup)
    {
        try
        {
            if (shouldLaunchOnStartup == IsStartupEnabled()) return false;

            if (shouldLaunchOnStartup)
            {
                // Set the main startup registry key
                // TODO: Execute this on startup the first time the app is launched
                using RegistryKey mainStartupKey = Registry.CurrentUser.OpenSubKey(MainStartupRegistryPath, true)!;
                mainStartupKey.SetValue(_appName, $"{_appPath} -background");
                mainStartupKey.Close();

                // Set the approved registry key to true
                using RegistryKey approvedStartupKey = Registry.CurrentUser.OpenSubKey(ApprovedStartupRegistryPath)!;
                var valueNames = approvedStartupKey.GetValueNames();

                if (valueNames.Any(name => name.Equals(_appName, StringComparison.OrdinalIgnoreCase))) 
                {
                    return SetRegistryValue(ApprovedStartupRegistryPath, _appName, StartupEnabledValueString);
                }
            }
            else
            {
                using RegistryKey mainStartupKey = Registry.CurrentUser.OpenSubKey(MainStartupRegistryPath, true)!;
                var mainValueNames = mainStartupKey.GetValueNames();

                // If the main startup key doesn't even exist we don't need to do anything else
                if (mainValueNames.Any(name => name.Equals(_appName, StringComparison.OrdinalIgnoreCase)) == false)
                {
                    return true;
                }

                // Set the approved registry key to false
                return SetRegistryValue(ApprovedStartupRegistryPath, _appName, StartupDisabledValueString);
            }
            return true;
        }
        catch (Exception ex)
        {
            var modificationType = shouldLaunchOnStartup ? "enable" : "disable";
            _logger.LogError(ex, @"An error occurred while trying to {modificationType} booting on startup", modificationType);
            return false;
        }
    }

    private bool SetRegistryValue(string keyPath, string valueName, string hexValueData)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "reg.exe",
            Arguments = $"add HKCU\\{keyPath} /v {valueName} /t REG_BINARY /d {hexValueData} /f",
            CreateNoWindow = true
        };

        try
        {
            Process.Start(startInfo)?.WaitForExit();
            if (hexValueData.StartsWith("02"))
            {
                _logger.LogInformation("Successfully enabled startup");
            }
            else
            {
                _logger.LogInformation("Successfully disabled startup");
            }
            return true;
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while trying to change registry value");
            return false;
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility
