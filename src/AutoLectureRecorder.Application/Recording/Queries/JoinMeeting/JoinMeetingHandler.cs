using System.Reactive;
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

    public JoinMeetingHandler(IWebDriverFactory webDriverFactory, IWebDriverDownloader webDriverDownloader, 
        IStudentAccountRepository studentAccountRepository)
    {
        _webDriverFactory = webDriverFactory;
        _webDriverDownloader = webDriverDownloader;
        _studentAccountRepository = studentAccountRepository;
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
        
        var webDriver = errorOrWebDriver.Value;

        var errorOrJoinedMeeting = webDriver.JoinMeeting(studentAccount.EmailAddress, studentAccount.Password,
            query.MeetingLink, query.LectureDuration, cancellationToken);

        webDriver.Dispose();

        return errorOrJoinedMeeting;
    }
}