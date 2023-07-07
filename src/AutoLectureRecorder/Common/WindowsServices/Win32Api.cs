using System;
using System.Runtime.InteropServices;

namespace AutoLectureRecorder.Common.WindowsServices;

public static class Win32Api
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [System.Security.SuppressUnmanagedCodeSecurity]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
}   