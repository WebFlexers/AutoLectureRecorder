using System.Reactive;
using AutoLectureRecorder.Application.Common.Abstractions.Encryption;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation.DownloadWebDriver;
using AutoLectureRecorder.Domain.Errors;
using ErrorOr;

namespace AutoLectureRecorder.Application.Recording.Queries.JoinMeeting;

public class JoinMeetingHandler : MediatR.IRequestHandler<JoinMeetingQuery, ErrorOr<Unit>>
{
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly IWebDriverDownloader _webDriverDownloader;
    private readonly IStudentAccountRepository _studentAccountRepository;
    private readonly IEncryptionService _encryptionService;

    public JoinMeetingHandler(IWebDriverFactory webDriverFactory, IWebDriverDownloader webDriverDownloader, 
        IStudentAccountRepository studentAccountRepository, IEncryptionService encryptionService)
    {
        _webDriverFactory = webDriverFactory;
        _webDriverDownloader = webDriverDownloader;
        _studentAccountRepository = studentAccountRepository;
        _encryptionService = encryptionService;
    }
    
    public async Task<ErrorOr<Unit>> Handle(JoinMeetingQuery query, CancellationToken cancellationToken)
    {
        var downloadDriverTask = _webDriverDownloader.Download();
        var getStudentAccountTask = _studentAccountRepository.GetStudentAccount();

        await Task.WhenAll(downloadDriverTask, getStudentAccountTask);

        if (downloadDriverTask.Result.IsError)
        {
            return downloadDriverTask.Result.Errors;
        }

        if (getStudentAccountTask.Result is null)
        {
            return Errors.StudentAccount.StudentAccountNotFound;
        }
        
        var studentAccount = getStudentAccountTask.Result;

        var errorOrWebDriver = _webDriverFactory.CreateUnipiEdgeWebDriver(true, TimeSpan.FromSeconds(55));
        if (errorOrWebDriver.IsError)
        {
            return errorOrWebDriver.Errors;
        }
        
        using var webDriver = errorOrWebDriver.Value;

        var password = _encryptionService.Decrypt(studentAccount.EncryptedPassword!, studentAccount.Entropy!);

        var errorOrJoinedMeeting = webDriver.JoinMeeting(studentAccount.EmailAddress, password,
            query.MeetingLink, query.LectureDuration, cancellationToken);

        password = null;

        return errorOrJoinedMeeting;
    }
}