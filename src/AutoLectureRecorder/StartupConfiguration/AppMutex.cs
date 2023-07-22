using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using AutoLectureRecorder.Common.WindowsServices;
using Serilog;

namespace AutoLectureRecorder.StartupConfiguration;

public class AppMutex
{
    private Mutex? _mutex;
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

    public bool IsAppRunning()
    {
        if (_mutexCreated) return false;
        
        // An instance is already running, bring it to the foreground
        var currentProcess = Process.GetCurrentProcess();
        var runningInstance = Process.GetProcessesByName(currentProcess.ProcessName)
            .FirstOrDefault(p => p.Id != currentProcess.Id);

        if (runningInstance != null)
        {
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", Pipes.ShowWindowPipe, 
                PipeDirection.Out);
                
            try
            {
                // Connect to the existing instance.
                pipeClient.Connect(500); // Timeout in milliseconds
                using StreamWriter writer = new StreamWriter(pipeClient);
                
                // Send a message to the existing instance.
                writer.WriteLine(Pipes.ShowWindowPipe);
                writer.Flush();
            }
            catch (TimeoutException ex)
            {
                Log.Logger.Error(ex, "An error occurred while trying to connect to the {pipe} pipe", 
                    Pipes.ShowWindowPipe);
            }
        }

        // Shutdown the new instance
        System.Windows.Application.Current.Shutdown();

        return true;
    }

    public void ReleaseMutex()
    {
        if (_mutexCreated)
        {
            try
            {
                _mutex?.ReleaseMutex();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName, 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        _mutex?.Dispose();
    }
}
