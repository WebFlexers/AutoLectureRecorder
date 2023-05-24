using AutoLectureRecorder.WindowsServices;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.ShortcutManager;

public class WindowsShortcutManagerTests
{
    private readonly ILogger<WindowsShortcutManager> _logger;

    public WindowsShortcutManagerTests(ITestOutputHelper testOutputHelper)
    {
        _logger = XUnitLogger.CreateLogger<WindowsShortcutManager>(testOutputHelper);
    }

    [Fact]
    public void CreateShortcut_ShouldCreateFileShortcut()
    {
        var shortcutManager = new WindowsShortcutManager(_logger);

        var shortcutDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Shortcuts");
        Directory.CreateDirectory(shortcutDirectory);   
   
        var targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(AutoLectureRecorder.Data)}.dll");
        var shortcutFilePath = Path.Combine(shortcutDirectory, $"{nameof(AutoLectureRecorder.Data)}.dll");

        if (File.Exists(shortcutFilePath))
        {
            File.Delete(shortcutFilePath);
        }

        shortcutManager.CreateShortcut(targetFilePath, shortcutFilePath, "TestDescription");

        Assert.True(File.Exists(shortcutFilePath));
    }
}
