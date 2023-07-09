using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoLectureRecorder.Application.Login;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Core.Abstractions;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Resources.Themes;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using MediatR;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Pages.Login;

public class LoginWebViewModel : RoutableViewModel
{
    private string _webViewSource = @"https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";
    public string WebViewSource
    {
        get => _webViewSource;
        set => this.RaiseAndSetIfChanged(ref _webViewSource, value);
    }
    public ReactiveCommand<WebView2, Unit> LoginToMicrosoftTeamsCommand { get; }

    public LoginWebViewModel(INavigationService navigationService, IWindowFactory windowFactory, 
        IThemeManager themeManager, ISender mediatorSender) : base(navigationService)
    {
        var loginCancellationTokenSource = new CancellationTokenSource();
        var loginCancellationToken = loginCancellationTokenSource.Token;

        var parameters = NavigationService.GetNavigationParameters(typeof(LoginWebViewModel));
        Debug.Assert(parameters is not null);

        string academicEmailAddress = (string)parameters[NavigationParameters.LoginWebViewModel.AcademicEmailAddress];
        string password = (string)parameters[NavigationParameters.LoginWebViewModel.Password];

        var mainWindow = System.Windows.Application.Current.MainWindow!;
        var isWindowFullScreen = mainWindow.WindowState == WindowState.Maximized;
        var overlayWindow = windowFactory.CreateTransparentOverlayWindow(mainWindow, isWindowFullScreen, () =>
        {
            loginCancellationTokenSource.Cancel();
            return Task.CompletedTask;
        });
        overlayWindow.Show();
        
        LoginToMicrosoftTeamsCommand = ReactiveCommand.CreateFromTask<WebView2>(async webview2 =>
        {
            webview2.CoreWebView2.Profile.PreferredColorScheme = themeManager.CurrentColorTheme == ColorTheme.Dark
                ? CoreWebView2PreferredColorScheme.Dark
                : CoreWebView2PreferredColorScheme.Light;

            await webview2.CoreWebView2.Profile.ClearBrowsingDataAsync();
            var loginQuery = new LoginToMicrosoftTeamsQuery(academicEmailAddress, password);
            var errorOrLoggedIn = await mediatorSender.Send(loginQuery, loginCancellationToken);

            errorOrLoggedIn.Match(_ =>
            {
                // TODO: Navigate to main menu
                return Unit.Default;
            }, errors =>
            {
                var loginParameters = new Dictionary<string, object>
                {
                    { NavigationParameters.LoginViewModel.ErrorMessage, errors.First().Description }
                };
                NavigationService.NavigateAndReset(typeof(LoginViewModel), HostNames.MainWindowHost, loginParameters);
                return Unit.Default;
            });
            
            overlayWindow.Close();
        });
    }
}
