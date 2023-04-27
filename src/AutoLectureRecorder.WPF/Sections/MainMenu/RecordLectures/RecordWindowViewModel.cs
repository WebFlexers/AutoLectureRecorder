using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Resources.Themes;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;

namespace AutoLectureRecorder.Sections.MainMenu.RecordLectures;

public class RecordWindowViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly ILogger<RecordWindowViewModel> _logger;
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly IStudentAccountRepository _accountRepository;
    private readonly IRecorder _recorder;

    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public bool IsRecordingFinished { get; set; }
    [Reactive]
    public ReactiveScheduledLecture? LectureToRecord { get; set; }
    [Reactive]
    public string? WebViewSource { get; set; }
    [Reactive]
    public WindowState RecordWindowState { get; set; }

    private IAlrWebDriver? _webDriver;
    private Task<(bool result, string resultMessage)>? _joinMeetingTask;

    private readonly CompositeDisposable _disposables = new();

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
        IStudentAccountRepository accountRepository, IRecorder recorder)
    {
        _logger = logger;
        _webDriverFactory = webDriverFactory;
        _accountRepository = accountRepository;
        _recorder = recorder;
        _recorder.DisposeWith(_disposables);

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
            await _joinMeetingTask.DisposeWith(_disposables);
            (bool joinedMeeting, string resultMessage) = await _joinMeetingTask;

            if (joinedMeeting == false)
            {
                // TODO: Return to Dashboard, update the statistics and display an error message
                return;
            }

            var recordingStarted = StartRecording();

            if (recordingStarted == false)
            {
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

            _recorder.StopRecording();
        });

        CleanupResourcesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_webView2 != null)
            {
                _webView2.CoreWebView2.PermissionRequested -= CoreWebView2OnPermissionRequested;
            }

            _joinMeetingCancellationTokenSource.Cancel();
            if (_joinMeetingTask != null)
            {
                var result = await _joinMeetingTask;
            }

            _recordCancellationTokenSource?.Cancel();
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
        _webDriver.DisposeWith(_disposables);

        (bool joinedMeeting, string resultMessage) = _webDriver.JoinMeeting(studentAccount.EmailAddress, studentAccount.Password, 
            LectureToRecord.MeetingLink, LectureToRecord.CalculateLectureDuration(), _joinMeetingCancellationToken);

        if (joinedMeeting == false)
        {
            // TODO: Return to Dashboard, update the statistics and display an error message
            return (false, "todo error message");
        }

        return (true, "success");
    }

    private bool StartRecording()
    {
        // TODO: Get directory path from the database instead of hardcoded documents
        var recordingDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "AutoLectureRecorder",
            $"Semester {LectureToRecord!.Semester}",
            LectureToRecord.SubjectName);

        if (Directory.Exists(recordingDirectory) == false)
        {
            Directory.CreateDirectory(recordingDirectory);
        }

        _recordingStartTime = DateTime.Now;
        _recorder.RecordingDirectoryPath = recordingDirectory;
        _recorder.RecordingFileName = _recordingStartTime.ToString("yyyy-MM-d hh-mm");

        try
        {
            _recordCancellationTokenSource = new CancellationTokenSource();
            _recordCancellationToken = _recordCancellationTokenSource.Token;

            IntPtr windowHandle = 
                new WindowInteropHelper(Window.GetWindow(_webView2!)!).Handle;

            _recorder.StartRecording(windowHandle, false)
                .OnRecordingComplete(async () =>
                {
                    IsRecordingFinished = true;
                    if (LectureToRecord.WillAutoUpload)
                    {
                        // TODO: Upload to the cloud
                    }
                })
                .OnRecordingFailed(() =>
                {
                    IsRecordingFinished = true;
                    // TODO: Return to Dashboard, update the statistics and display an error message
                });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to start recording");
            return false;
        }
    }
}
