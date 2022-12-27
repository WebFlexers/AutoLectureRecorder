using AutoLectureRecorder.Services.WebDriver;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace AutoLectureRecorder.WPF.Sections.WebView;

public class WebViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly ILogger<WebViewModel> _logger;

    public string? UrlPathSegment => nameof(WebViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> LoginToMicrosoftTeamsCommand { get; private set; }

    public WebViewModel(ILogger<WebViewModel> logger, IScreenFactory screenFactory, IWebDriverFactory webDriverFactory)
    {
        HostScreen = screenFactory.GetMainWindowViewModel();
        _webDriverFactory = webDriverFactory;

        LoginToMicrosoftTeamsCommand = ReactiveCommand.CreateFromTask(LoginToMicrosoftTeams);
        _logger = logger;
    }

    [Reactive]
    public string WebViewSource { get; set; } = UnipiEdgeWebDriver.MICROSOFT_TEAMS_AUTH_URL;

    private async Task LoginToMicrosoftTeams()
    {
        await Task.Run(() =>
        {
            var webDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(15));
            (bool, string) loginResult = webDriver.LoginToMicrosoftTeams("p19165@unipi.gr", "nhy6514236798awdsM");

            if (loginResult.Item1)
            {
                _logger.LogInformation($"{loginResult.Item2}");
            }
            else
            {
                _logger.LogInformation($"{loginResult.Item2}");
            }
        });

    }
}
