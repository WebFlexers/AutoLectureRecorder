using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows;

namespace AutoLectureRecorder;

public class AppMutex
{
    private Mutex _mutex;
    private bool _mutexCreated;

    public void InitializeMutex()
    {
        // Use a global mutex to stop the app from running more than once
        string mutexId = $"Global\\{GetType().GUID}";

        MutexAccessRule allowEveryoneRule = new(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            MutexRights.FullControl,
            AccessControlType.Allow);
        MutexSecurity securitySettings = new();
        securitySettings.AddAccessRule(allowEveryoneRule);

        _mutex = new Mutex(initiallyOwned: true, mutexId, out _mutexCreated);
        _mutex.SetAccessControl(securitySettings);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    [System.Security.SuppressUnmanagedCodeSecurity]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public void HandleDuplicateAppInstance()
    {
        if (_mutexCreated == false)
        {
            // An instance is already running, bring it to the foreground
            var currentProcess = Process.GetCurrentProcess();
            var runningInstance = Process.GetProcessesByName(currentProcess.ProcessName)
                .FirstOrDefault(p => p.Id != currentProcess.Id);

            if (runningInstance != null)
            {
                ShowWindow(runningInstance.MainWindowHandle, 5);
                SetForegroundWindow(runningInstance.MainWindowHandle);
            }

            // Shutdown the new instance
            Application.Current.Shutdown();
        }
    }

    public void ReleaseMutex()
    {
        if (_mutexCreated)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName, 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        _mutex.Dispose();
    }
}
