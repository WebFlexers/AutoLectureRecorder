using AutoLectureRecorder.Services.StartupManager;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.StartupManager;

public class WindowsStartupManagerTests
{
    private readonly ILogger<WindowsStartupManager> _logger;

    public WindowsStartupManagerTests(ITestOutputHelper testOutputHelper)
    {
        _logger = XUnitLogger.CreateLogger<WindowsStartupManager>(testOutputHelper);
    }

    [Fact]
    public void IsStartupLaunchEnabled_ShouldReturnTrue() 
    {
        var startupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        var shortcutAlreadyExists = File.Exists(Path.Combine(startupDirectory,
            $"AutoLectureRecorder.lnk"));

        var shortcutManagerMock = new WindowsShortcutManagerMock();
        var mockFilePathWithExtension = Path.Combine(startupDirectory, "AutoLectureRecorder.lnk");

        if (shortcutAlreadyExists == false)
        {     
            shortcutManagerMock.CreateShortcut("", mockFilePathWithExtension);
        }

        var startupManager = new WindowsStartupManager(_logger, shortcutManagerMock);
        
        Assert.True(startupManager.IsStartupLaunchEnabled(nameof(AutoLectureRecorder)));

        if (shortcutAlreadyExists == false)
        {
            File.Delete(mockFilePathWithExtension);
        } 
    }

    [Fact]
    public void ModifyLaunchOnStartup_ShouldEnableLaunchOnStartup()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var startupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        var backupsDirectory = Path.Combine(currentDirectory, "StartupManagerBackups");
        var shortcutPath = Path.Combine(startupDirectory, $"{nameof(AutoLectureRecorder.Data)}.lnk");

        Directory.CreateDirectory(backupsDirectory);
        RevertableFileIO files = new(backupsDirectory);

        if (File.Exists(shortcutPath))
        {
            files.DeleteFile(shortcutPath);
        }

        var shortcutManagerMock = new WindowsShortcutManagerMock();
        var startupManager = new WindowsStartupManager(_logger, shortcutManagerMock);
        
        startupManager.ModifyLaunchOnStartup(currentDirectory, nameof(AutoLectureRecorder.Data), true);

        Assert.True(File.Exists(shortcutPath));

        File.Delete(shortcutPath);

        files.RestoreBackup();
    }

    [Fact]
    public void ModifyLaunchOnStartup_ShouldDisableLaunchOnStartup()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var startupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        var backupsDirectory = Path.Combine(currentDirectory, "StartupManagerBackups");
        var shortcutPath = Path.Combine(startupDirectory, $"{nameof(AutoLectureRecorder.Data)}.lnk");

        Directory.CreateDirectory(backupsDirectory);
        RevertableFileIO files = new(backupsDirectory);

        var shortcutManagerMock = new WindowsShortcutManagerMock();

        if (File.Exists(shortcutPath) == false)
        {
            shortcutManagerMock.CreateShortcut("", shortcutPath);
        }
        else
        {
            files.BackupFile(shortcutPath);
        }

        var startupManager = new WindowsStartupManager(_logger, shortcutManagerMock);
        startupManager.ModifyLaunchOnStartup(currentDirectory, nameof(AutoLectureRecorder.Data), false);

        Assert.False(File.Exists(shortcutPath));

        files.RestoreBackup();
    }
}
