using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using AutoLectureRecorder.Application.Recording.Commands.RecordMeeting;
using AutoLectureRecorder.Application.Recording.Queries.JoinMeeting;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Domain.Errors;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Resources.Themes;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using ReactiveUI;
using Unit = System.Reactive.Unit;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;

namespace AutoLectureRecorder.Pages.RecordLecture;

public class RecordWindowViewModel : RoutableViewModelHost
{

    private readonly ILogger<RecordWindowViewModel> _logger;

    public string? WebViewSource { get; } = @"https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";

    private WindowState _recordWindowState;
    public WindowState RecordWindowState
    {
        get => _recordWindowState; 
        set => this.RaiseAndSetIfChanged(ref _recordWindowState, value);
    }
    
    public ReactiveCommand<Window, Unit> CloseWindowCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; }
    
    public ReactiveCommand<WebView2?, Unit> CleanupBeforeExitCommand { get; }
    public ReactiveCommand<WebView2, Unit> JoinAndRecordMeetingCommand { get; }

    // This is used to wrap everything up and avoid memory leaks if the user manually closes the recording window
    private bool _isWindowReadyToClose = true;
    private bool IsWindowReadyToClose
    {
        get => _isWindowReadyToClose;
        set => this.RaiseAndSetIfChanged(ref _isWindowReadyToClose, value);
    }

    public RecordWindowViewModel(ILogger<RecordWindowViewModel> logger, IThemeManager themeManager, 
        ISender mediatorSender, INavigationService navigationService) : base(navigationService)
    {
        themeManager.RefreshTheme();

        _logger = logger;
        
        var cancellationTokenSource = new CancellationTokenSource();

        CleanupBeforeExitCommand = ReactiveCommand.CreateFromTask<WebView2?, Unit>(async webview2 =>
        {
            cancellationTokenSource.Cancel();

            if (webview2?.CoreWebView2 != null)
            {
                webview2.CoreWebView2.PermissionRequested -= CoreWebView2OnPermissionRequested;
            }

            await this.WhenAnyValue(vm => vm.IsWindowReadyToClose)
                .SkipWhile(isWindowReadyToClose => isWindowReadyToClose == false)
                .Take(1)
                .ToTask();

            return Unit.Default;
        });

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

        var navigationParameters = navigationService.GetNavigationParameters(typeof(RecordWindowViewModel));
        var lectureToRecord = (ReactiveScheduledLecture)navigationParameters!
            [NavigationParameters.RecordLecture.LectureToRecord];
        
        JoinAndRecordMeetingCommand = ReactiveCommand.CreateFromTask<WebView2>(async webview2 =>
        {
            IsWindowReadyToClose = false;
            
            webview2.CoreWebView2.PermissionRequested += CoreWebView2OnPermissionRequested;
            webview2.CoreWebView2.Profile.PreferredColorScheme = themeManager.CurrentColorTheme == ColorTheme.Dark
                ? CoreWebView2PreferredColorScheme.Dark
                : CoreWebView2PreferredColorScheme.Light;

            await webview2.CoreWebView2.Profile.ClearBrowsingDataAsync();
            
            // Used to avoid "Tried to access object of a different thread" exception
            var uiDispatcher = Dispatcher.CurrentDispatcher;
            
            ErrorOr<Unit> errorOrSuccessfullyRecordedLecture = await Task.Run(async () =>
            {
                if (lectureToRecord.MeetingLink is null)
                {
                    return Errors.Meeting.NullMeetingLink;
                }

                var joinMeetingQuery = new JoinMeetingQuery(lectureToRecord.MeetingLink, lectureToRecord.LectureDuration);
                var errorOrSuccessfullyJoinedMeeting = await mediatorSender
                    .Send(joinMeetingQuery, cancellationTokenSource.Token);

                if (errorOrSuccessfullyJoinedMeeting.IsError)
                {
                    return errorOrSuccessfullyJoinedMeeting.Errors;
                }

                IntPtr windowHandle = uiDispatcher.Invoke(() =>
                    new WindowInteropHelper(Window.GetWindow(webview2)!).Handle);

                // TODO: Update dashboard on record meeting failed and/or success
                var recordMeetingCommand = new RecordMeetingCommand(lectureToRecord.SubjectName!,
                    lectureToRecord.Semester, lectureToRecord.EndTime, windowHandle);

                var errorOrSuccessfullyRecordedLecture = await mediatorSender
                    .Send(recordMeetingCommand, cancellationTokenSource.Token);

                return errorOrSuccessfullyRecordedLecture;
            });
            
            /*if (errorOrSuccessfullyRecordedLecture.IsError)
            {
                // TODO: Return to Dashboard, update the statistics and display an error message
                return;
            }*/

            IsWindowReadyToClose = true;

            Window.GetWindow(webview2)!.Close();
        });
    }

    private void CoreWebView2OnPermissionRequested(object? sender, CoreWebView2PermissionRequestedEventArgs e)
    {
        e.State = CoreWebView2PermissionState.Deny;
        _logger.LogInformation("Permission to access {PermissionType} in webview denied", e.PermissionKind);
    }
}
