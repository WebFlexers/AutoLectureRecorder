using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.Login;
using AutoLectureRecorder.Sections.MainMenu;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.Services.WebDriver.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.Sections.LoginWebView;

public class LoginWebViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly IStudentAccountRepository _studentAccountData;
    private readonly ILogger<LoginWebViewModel> _logger;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IWindowFactory _windowFactory;

    private readonly Window? _overlayWindow;

    public ViewModelActivator Activator { get; } = new();
    public string UrlPathSegment => nameof(LoginWebViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> LoginToMicrosoftTeamsCommand { get; }

    private readonly CancellationTokenSource _loginCancellationTokenSource;
    private readonly CancellationToken _loginCancellationToken;
    public LoginWebViewModel(ILogger<LoginWebViewModel> logger, IScreenFactory screenFactory, IViewModelFactory viewModelFactory,
                             IWindowFactory windowFactory, IWebDriverFactory webDriverFactory, IStudentAccountRepository studentAccountData)
    {
        _logger = logger;
        HostScreen = screenFactory.GetMainWindowViewModel();
        _viewModelFactory = viewModelFactory;
        _windowFactory = windowFactory;
        _webDriverFactory = webDriverFactory;
        _studentAccountData = studentAccountData;

        _loginCancellationTokenSource = new CancellationTokenSource();
        _loginCancellationToken = _loginCancellationTokenSource.Token;

        MessageBus.Current.SendMessage(true, PubSubMessages.UpdateWindowTopMost);

        MessageBus.Current.Listen<(string, string)>(PubSubMessages.FillLoginCredentials)
        .Subscribe(credentials =>
        {
            _academicEmailAddress = credentials.Item1;
            _password = credentials.Item2;
        });

        LoginToMicrosoftTeamsCommand = ReactiveCommand.CreateFromTask(LoginToMicrosoftTeams);

        var mainWindow = Application.Current.MainWindow!;
        var isWindowFullScreen = mainWindow.WindowState == WindowState.Maximized;
        _overlayWindow = _windowFactory.CreateTransparentOverlayWindow(mainWindow, isWindowFullScreen, () =>
        {
            _loginCancellationTokenSource.Cancel();
            return Task.CompletedTask;
        });
        _overlayWindow.Show();
    }

    private void NavigateToLoginAfterCancel()
    {
        HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));
        MessageBus.Current.SendMessage("The login process was cancelled. If you want to continue, input your credentials and try again", 
            PubSubMessages.UpdateLoginErrorMessage);
    }

    [Reactive]
    public string WebViewSource { get; set; } = UnipiEdgeWebDriver.MicrosoftTeamsAuthUrl;

    private string _academicEmailAddress = "";
    private string _password = "";
    private IAlrWebDriver? _webDriver;
    private Task<(bool result, string resultMessage)>? _loginTask;

    private async Task LoginToMicrosoftTeams()
    {
        if (_loginCancellationToken.IsCancellationRequested)
        {
            NavigateToLoginAfterCancel();
            _overlayWindow?.Close();
            return;
        }

        _loginTask = Task.Run(() =>
        {
            _webDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(17));
            return _webDriver.LoginToMicrosoftTeams(_academicEmailAddress, _password, _loginCancellationToken);
        }, _loginCancellationToken);

        (bool isSuccessful, string resultMessage) = await _loginTask;

        _logger.LogInformation("Login Result: {result}, Message: {message}", isSuccessful, resultMessage);

        _overlayWindow?.Close();

        if (isSuccessful)
        {
            await _studentAccountData.DeleteStudentAccountAsync();
            await _studentAccountData.InsertStudentAccountAsync(_academicEmailAddress.Split("@")[0], _academicEmailAddress, _password);
            HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(MainMenuViewModel)));
        }
        else
        {
            HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));
            MessageBus.Current.SendMessage(resultMessage, PubSubMessages.UpdateLoginErrorMessage);
        }

        _webDriver?.Dispose();
        _loginTask.Dispose();

        MessageBus.Current.SendMessage(false, PubSubMessages.UpdateWindowTopMost);
    }
}
