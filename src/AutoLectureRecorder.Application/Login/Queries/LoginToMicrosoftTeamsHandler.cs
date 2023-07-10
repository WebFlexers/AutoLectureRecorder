using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Domain.Errors;
using ErrorOr;
using MediatR;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Application.Login.Queries;

public class LoginToMicrosoftTeamsHandler : IRequestHandler<LoginToMicrosoftTeamsQuery, ErrorOr<Unit>>
{
    private readonly IWebDriverFactory _webDriverFactory;

    public LoginToMicrosoftTeamsHandler(IWebDriverFactory webDriverFactory)
    {
        _webDriverFactory = webDriverFactory;
    }
    
    public async Task<ErrorOr<Unit>> Handle(LoginToMicrosoftTeamsQuery request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Errors.Login.LoginCancelled;
        }

        var loginTask = Task.Run(() =>
        {
            var errorOrWebDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(40));
            if (errorOrWebDriver.IsError)
            {
                return errorOrWebDriver.Errors;
            }

            var webDriver = errorOrWebDriver.Value;
            return webDriver.Login(request.AcademicEmailAddress, request.Password, cancellationToken: cancellationToken);
        }, cancellationToken);

        var errorOrLoggedIn = await loginTask;

        if (errorOrLoggedIn.IsError)
        {
            return errorOrLoggedIn.Errors;
        }

        // TODO: Do these when the repository is ready
        /*if (isSuccessful)
        {
            await _studentAccountData.DeleteStudentAccount();
            await _studentAccountData.InsertStudentAccount(_academicEmailAddress.Split("@")[0], _academicEmailAddress, _password);
            HostScreen.Router.NavigateAndReset.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(MainMenuViewModel)));
        }*/

        // TODO: Replace with result message
        return Unit.Default;
    }
}