using System.Reactive;
using ErrorOr;

namespace AutoLectureRecorder.Application.Common.Abstractions.WebAutomation.DownloadWebDriver;

public interface IWebDriverDownloader
{
    Task<ErrorOr<Unit>> Download(IProgress<float>? progress = null);
}