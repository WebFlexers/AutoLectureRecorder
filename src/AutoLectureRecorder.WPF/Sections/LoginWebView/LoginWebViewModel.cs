using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.Login;
using AutoLectureRecorder.WPF.Sections.MainMenu;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace AutoLectureRecorder.WPF.Sections.LoginWebView;

public class LoginWebViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly ILogger<LoginWebViewModel> _logger;
    private readonly IViewModelFactory _viewModelFactory;

    public string? UrlPathSegment => nameof(LoginWebViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> LoginToMicrosoftTeamsCommand { get; private set; }

    public LoginWebViewModel(ILogger<LoginWebViewModel> logger, IScreenFactory screenFactory, IViewModelFactory viewModelFactory,IWebDriverFactory webDriverFactory)
    {
        _logger = logger;
        HostScreen = screenFactory.GetMainWindowViewModel();
        _viewModelFactory = viewModelFactory;
        _webDriverFactory = webDriverFactory;

        LoginToMicrosoftTeamsCommand = ReactiveCommand.CreateFromTask(LoginToMicrosoftTeams);
    }

    [Reactive]
    public string WebViewSource { get; set; } = UnipiEdgeWebDriver.MICROSOFT_TEAMS_AUTH_URL;

    private async Task LoginToMicrosoftTeams()
    {
        IWebDriver? webDriver = null;

        (bool, string) loginResult = await Task.Run(() =>
        {
            webDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(17));
            return webDriver.LoginToMicrosoftTeams("p19165@unipi.gr", "wrong_pass_mate");
        });

        _logger.LogInformation("Login Result: {result}, Message: {message}", loginResult.Item1, loginResult.Item2);

        if (loginResult.Item1)
        {
            HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(MainMenuViewModel)));
        }
        else
        {
            HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));
            MessageBus.Current.SendMessage(loginResult.Item2, "LoginErrorMessage");
            webDriver?.Dispose();
        }
    }
}
