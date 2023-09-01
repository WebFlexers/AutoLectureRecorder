using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Domain.Errors;
using ErrorOr;
using MediatR;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Application.Login.Queries.LoginToMicrosoftTeams;

public class LoginToMicrosoftTeamsHandler : IRequestHandler<LoginToMicrosoftTeamsQuery, ErrorOr<Unit>>
{
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly IStudentAccountRepository _studentAccountRepository;

    public LoginToMicrosoftTeamsHandler(IWebDriverFactory webDriverFactory, 
        IStudentAccountRepository studentAccountRepository)
    {
        _webDriverFactory = webDriverFactory;
        _studentAccountRepository = studentAccountRepository;
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

            using var webDriver = errorOrWebDriver.Value;
            return webDriver.Login(request.AcademicEmailAddress, request.Password, cancellationToken: cancellationToken);
        }, cancellationToken);

        var errorOrLoggedIn = await loginTask;

        if (errorOrLoggedIn.IsError)
        {
            return errorOrLoggedIn.Errors;
        }
        
        await _studentAccountRepository.DeleteStudentAccount();
        await _studentAccountRepository.InsertStudentAccount(
            request.AcademicEmailAddress.Split("@")[0], 
            request.AcademicEmailAddress, 
            request.Password);
        
        return Unit.Default;
    }
}