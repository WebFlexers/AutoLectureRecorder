using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Resources.Themes;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using AutoLectureRecorder.Services.Recording;
using AutoLectureRecorder.Services.WebDriver.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;

namespace AutoLectureRecorder.Sections.MainMenu.RecordLectures;

public class RecordWindowViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly CompositeDisposable _disposables = new();
    public ViewModelActivator Activator { get; } = new();

    private readonly ILogger<RecordWindowViewModel> _logger;
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly IStudentAccountRepository _accountRepository;
    private readonly ISettingsRepository _settingsRepository;
    private IAlrWebDriver? _webDriver;

    [Reactive]
    public IRecorder Recorder { get; set; }
    [Reactive]
    public ReactiveScheduledLecture? LectureToRecord { get; set; }
    [Reactive]
    public string? WebViewSource { get; set; }
    [Reactive]
    public WindowState RecordWindowState { get; set; }

    private Task<(bool result, string resultMessage)>? _joinMeetingTask;

    public ReactiveCommand<Window, Unit> CloseWindowCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; }
    public ReactiveCommand<WebView2, Unit> JoinAndRecordMeetingCommand { get; }
    public ReactiveCommand<Unit, Unit> CleanupResourcesCommand { get; }

    public void Initialize(ReactiveScheduledLecture scheduledLecture)
    {
        LectureToRecord = scheduledLecture;
        WebViewSource = LectureToRecord.MeetingLink;
        RecordWindowState = WindowState.Maximized;
    }

    private WebView2? _webView2;
    private DateTime _recordingStartTime;

    private CancellationTokenSource? _recordCancellationTokenSource;
    private CancellationToken _recordCancellationToken;
    private CancellationTokenSource _joinMeetingCancellationTokenSource;
    private CancellationToken _joinMeetingCancellationToken;

    public RecordWindowViewModel(ILogger<RecordWindowViewModel> logger, IWebDriverFactory webDriverFactory,
        IStudentAccountRepository accountRepository, IRecorder recorder, ISettingsRepository settingsRepository)
    {
        ThemeManager.RefreshTheme();

        _logger = logger;
        _webDriverFactory = webDriverFactory;
        _accountRepository = accountRepository;
        _settingsRepository = settingsRepository;
        Recorder = recorder;

        CloseWindowCommand = ReactiveCommand.Create<Window>(window =>
        {
            window.Close();
        });
        ToggleWindowStateCommand = ReactiveCommand.Create(() =>
        {
            RecordWindowState = RecordWindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        });
        MinimizeWindowCommand = ReactiveCommand.Create(() => RecordWindowState = WindowState.Minimized);

        _joinMeetingCancellationTokenSource = new CancellationTokenSource();
        _joinMeetingCancellationToken = _joinMeetingCancellationTokenSource.Token;

        JoinAndRecordMeetingCommand = ReactiveCommand.CreateFromTask<WebView2>(async webview2 =>
        {
            _webView2 = webview2;

            webview2.CoreWebView2.PermissionRequested += CoreWebView2OnPermissionRequested;
            webview2.CoreWebView2.Profile.PreferredColorScheme = ThemeManager.CurrentColorTheme == ColorTheme.Dark
                ? CoreWebView2PreferredColorScheme.Dark
                : CoreWebView2PreferredColorScheme.Light;

            _joinMeetingTask = Task.Run(JoinAndRecordMeeting);
            (bool joinedMeeting, string resultMessage) = await _joinMeetingTask;

            if (joinedMeeting == false)
            {
                // TODO: Return to Dashboard, update the statistics and display an error message
                return;
            }

            try
            {
                await StartRecording();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to start recording");
                // TODO: Return to Dashboard, update the statistics and display an error message
                return;
            }

            var now = DateTime.Now;
            var endTime = LectureToRecord!.EndTime!.Value;
            var timeBeforeStoppingRecording = new TimeSpan(endTime.Hour, endTime.Minute, 0)
                                            .Subtract(new TimeSpan(now.Hour, now.Minute, now.Second));

            try
            {
                await Task.Delay(timeBeforeStoppingRecording, _recordCancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("The recording was cancelled manually");
            }

            Recorder.StopRecording();

            Window.GetWindow(_webView2!)!.Close();
        });

        bool resourcesCleaned = false;
        CleanupResourcesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (resourcesCleaned) return;

            if (_webView2 != null)
            {
                _webView2.CoreWebView2.PermissionRequested -= CoreWebView2OnPermissionRequested;
            }

            _joinMeetingCancellationTokenSource.Cancel();
            if (_joinMeetingTask != null)
            {
                var result = await _joinMeetingTask;
                _joinMeetingTask.Dispose();
            }

            _recordCancellationTokenSource?.Cancel();

            await JoinAndRecordMeetingCommand.IsExecuting
                .SkipWhile(isExecuting => isExecuting)
                .Take(1)
                .ToTask();
            
            // TODO: Make sure that the Action invoked when finishing the recording is done, before disposing the recorder
            // Wait until recorder finishes
            await this.WhenAnyValue(vm => vm.Recorder.IsRecording)
                .Do(isRecording => _logger.LogInformation("IsRecording: {val}", isRecording))
                .SkipWhile(isRecording => isRecording)
                .Take(1)
                .ToTask();

            _webDriver?.Dispose();

            resourcesCleaned = true;
        });

        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }

    private void CoreWebView2OnPermissionRequested(object? sender, CoreWebView2PermissionRequestedEventArgs e)
    {
        e.State = CoreWebView2PermissionState.Deny;
        _logger.LogInformation("Permission to access {permissionType} in webview denied", e.PermissionKind);
    }

    private async Task<(bool result, string resultMessage)> JoinAndRecordMeeting()
    {
        if (LectureToRecord == null || WebViewSource == null)
        {
            _logger.LogError("Lecture to record and/or WebViewSource were null");
            return (false, "Lecture to record and/or WebViewSource were null");
        }

        var studentAccount = await _accountRepository.GetStudentAccountAsync();
        if (studentAccount == null)
        {
            _logger.LogError("Student could not be retrieved before attempting to join meeting");
            return (false, "Student could not be retrieved before attempting to join meeting");
        }

        _webDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(90));
        if (_webDriver == null) return (false, "The web driver failed to be created");

        (bool joinedMeeting, string resultMessage) = _webDriver.JoinMeeting(studentAccount.EmailAddress, studentAccount.Password, 
            LectureToRecord.MeetingLink, LectureToRecord.CalculateLectureDuration(), _joinMeetingCancellationToken);

        if (joinedMeeting == false)
        {
            // TODO: Return to Dashboard, update the statistics and display an error message
            return (false, "todo error message");
        }

        return (true, "success");
    }

    private async Task StartRecording()
    {
        var recordingSettings = await _settingsRepository.GetRecordingSettings();

        if (recordingSettings == null)
        {
            recordingSettings = WindowsRecorder.GetDefaultSettings(1366, 768);
        }

        Recorder.RecordingDirectoryPath = Path.Combine(recordingSettings.RecordingsLocalPath, 
            $"Semester {LectureToRecord!.Semester}", 
            LectureToRecord.SubjectName);

        if (Directory.Exists(Recorder.RecordingDirectoryPath) == false)
        {
            Directory.CreateDirectory(recordingSettings.RecordingsLocalPath);
        }

        _recordingStartTime = DateTime.Now;
        Recorder.RecordingFileName = _recordingStartTime.ToString("yyyy-MM-d hh-mm");

        Recorder.ApplyRecordingSettings(recordingSettings);

        _recordCancellationTokenSource = new CancellationTokenSource();
        _recordCancellationToken = _recordCancellationTokenSource.Token;

        IntPtr windowHandle = new WindowInteropHelper(Window.GetWindow(_webView2!)!).Handle;

        Recorder.StartRecording(windowHandle, false)
            .OnRecordingComplete(() =>
            {
                if (LectureToRecord.WillAutoUpload)
                {
                    // TODO: Upload to the cloud
                }
            })
            .OnRecordingFailed(() =>
            {
                // TODO: Return to Dashboard, update the statistics and display an error message
            });
    }
}
