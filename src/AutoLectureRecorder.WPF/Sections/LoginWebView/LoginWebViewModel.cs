using AutoLectureRecorder.Services.DataAccess;
using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.Login;
using AutoLectureRecorder.WPF.Sections.MainMenu;
using AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;
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
    private readonly IStudentAccountData _studentAccountData;
    private readonly ILogger<LoginWebViewModel> _logger;
    private readonly IViewModelFactory _viewModelFactory;

    public string? UrlPathSegment => nameof(LoginWebViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> LoginToMicrosoftTeamsCommand { get; private set; }

    public LoginWebViewModel(ILogger<LoginWebViewModel> logger, IScreenFactory screenFactory, IViewModelFactory viewModelFactory, 
                             IWebDriverFactory webDriverFactory, IStudentAccountData studentAccountData)
    {
        _logger = logger;
        HostScreen = screenFactory.GetMainWindowViewModel();
        _viewModelFactory = viewModelFactory;
        _webDriverFactory = webDriverFactory;
        _studentAccountData = studentAccountData;
        MessageBus.Current.SendMessage<bool>(true, "MainWindowTopMost");
        MessageBus.Current.Listen<(string, string)>("StudentAccount").Subscribe(account =>
        {
            _academicEmailAddress = account.Item1;
            _password = account.Item2;
        });

        LoginToMicrosoftTeamsCommand = ReactiveCommand.CreateFromTask(LoginToMicrosoftTeams);
    }

    [Reactive]
    public string WebViewSource { get; set; } = UnipiEdgeWebDriver.MICROSOFT_TEAMS_AUTH_URL;

    private string _academicEmailAddress = "";
    private string _password = "";
    private async Task LoginToMicrosoftTeams()
    {
        IWebDriver? webDriver = null;

        (bool, string) loginResult = await Task.Run(() =>
        {
            webDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(17));
            return webDriver.LoginToMicrosoftTeams(_academicEmailAddress, _password);
        });

        _logger.LogInformation("Login Result: {result}, Message: {message}", loginResult.Item1, loginResult.Item2);

        if (loginResult.Item1)
        {
            await _studentAccountData.DeleteStudentAccount();
            await _studentAccountData.InsertStudentAccount(_academicEmailAddress.Split("@")[0], _academicEmailAddress, _password);
            HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(MainMenuViewModel)));
        }
        else
        {
            HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));
            MessageBus.Current.SendMessage(loginResult.Item2, "LoginErrorMessage");
            webDriver?.Dispose();
        }

        MessageBus.Current.SendMessage<bool>(false, "MainWindowTopMost");
    }
}
