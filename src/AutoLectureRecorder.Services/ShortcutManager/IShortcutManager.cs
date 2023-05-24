namespace AutoLectureRecorder.Services.ShortcutManager;

public interface IShortcutManager
{
    /// <summary>
    /// Creates a shortcut of the target path app to the shortcut path. The shortcut
    /// path must end with lnk
    /// </summary>
    /// <param name="targetPath">The path to the app to create a shortcut from</param>
    /// <param name="shortcutPath">The path of the shortcut (must end in lnk)</param>
    /// <param name="description">The description of the shortcut when hovered over</param>
    bool CreateShortcut(string targetPath, string shortcutPath, string description = "");
}
