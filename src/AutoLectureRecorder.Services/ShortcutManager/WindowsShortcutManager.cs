using AutoLectureRecorder.Services.ShortcutManager;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AutoLectureRecorder.WindowsServices;

// More information at: https://answers.microsoft.com/en-us/windows/forum/all/how-to-create-a-desktop-shortcut-in-c-visual/9eb48ee4-d672-4025-8cf0-bddbe19c05ac

public class WindowsShortcutManager : IShortcutManager
{
    private readonly ILogger<WindowsShortcutManager> _logger;

    public WindowsShortcutManager(ILogger<WindowsShortcutManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a shortcut of the target path app to the shortcut path. The shortcut
    /// path must end with lnk
    /// </summary>
    /// <param name="targetPath">The path to the app to create a shortcut from</param>
    /// <param name="shortcutPath">The path of the shortcut (must end in lnk)</param>
    /// <param name="description">The description of the shortcut when hovered over</param>
    public bool CreateShortcut(string targetPath, string shortcutPath, string description = "")
    {
        try
        {
            IShellLink link = (IShellLink)new ShellLink();

            link.SetDescription(description);
            link.SetPath(targetPath);

            IPersistFile file = (IPersistFile)link;

            file.Save(shortcutPath, false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, @"An error occured while trying to create a shortcut for target path {target}
                to shortcut path {shortcut}", targetPath, shortcutPath);
            return false;
        }
    }

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}