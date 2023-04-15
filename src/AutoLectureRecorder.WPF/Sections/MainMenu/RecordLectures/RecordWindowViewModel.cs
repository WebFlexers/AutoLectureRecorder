using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Resources.Themes;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoLectureRecorder.Services.WebDriver.Interfaces;

namespace AutoLectureRecorder.Sections.MainMenu.RecordLectures;

public class RecordWindowViewModel : ReactiveObject, IScreen
{
    private readonly ILogger<RecordWindowViewModel> _logger;
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly IStudentAccountRepository _accountRepository;
    public RoutingState Router { get; } = new();

    [Reactive]
    public ReactiveScheduledLecture? LectureToRecord { get; set; }
    [Reactive]
    public string? WebViewSource { get; set; }
    [Reactive]
    public WindowState RecordWindowState { get; set; }

    private IAlrWebDriver? _webDriver;
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

    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    public RecordWindowViewModel(ILogger<RecordWindowViewModel> logger, IWebDriverFactory webDriverFactory,
        IStudentAccountRepository accountRepository)
    {
        _logger = logger;
        _webDriverFactory = webDriverFactory;
        _accountRepository = accountRepository;

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

        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;

        JoinAndRecordMeetingCommand = ReactiveCommand.CreateFromTask<WebView2>(async webview2 =>
        {
            webview2.CoreWebView2.Profile.PreferredColorScheme = ThemeManager.CurrentColorTheme == ColorTheme.Dark
                ? CoreWebView2PreferredColorScheme.Dark
                : CoreWebView2PreferredColorScheme.Light;

            _joinMeetingTask = Task.Run(JoinAndRecordMeeting);
            (bool result, string resultMessage) = await _joinMeetingTask;
            if (result == false)
            {
                // TODO: Return to Dashboard, update the statistics and display an error message
            } 
        });

        CleanupResourcesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            _cancellationTokenSource.Cancel();
            if (_joinMeetingTask != null)
            {
                await _joinMeetingTask;
            }
            _webDriver?.Dispose();
            _joinMeetingTask?.Dispose();
        });
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

        _webDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(5));

        (bool joinedMeeting, string resultMessage) = _webDriver.JoinMeeting(studentAccount.EmailAddress, studentAccount.Password, 
            LectureToRecord.MeetingLink, LectureToRecord.CalculateLectureDuration(), _cancellationToken);

        if (joinedMeeting == false)
        {
            // TODO: Return to Dashboard, update the statistics and display an error message
            return (false, "todo error message");
        }

        return (true, "success");
    }
}
