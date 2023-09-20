using AutoLectureRecorder.Application.Common.Abstractions.Encryption;
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
    private readonly IEncryptionService _encryptionService;

    public LoginToMicrosoftTeamsHandler(IWebDriverFactory webDriverFactory, 
        IStudentAccountRepository studentAccountRepository, IEncryptionService encryptionService)
    {
        _webDriverFactory = webDriverFactory;
        _studentAccountRepository = studentAccountRepository;
        _encryptionService = encryptionService;
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

        var entropy = _encryptionService.GenerateRandomEntropy();
        var encryptedPassword = _encryptionService.Encrypt(request.Password, entropy);
        
        await _studentAccountRepository.DeleteStudentAccount();
        await _studentAccountRepository.InsertStudentAccount(
            request.AcademicEmailAddress.Split("@")[0], 
            request.AcademicEmailAddress, 
            encryptedPassword,
            entropy);

        entropy = null;
        encryptedPassword = null;
        
        return Unit.Default;
    }
}