using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors
{
    public static class WebDriver
    {
        public static Error FailedToCreateWebDriver => Error.Failure(
            code: nameof(FailedToCreateWebDriver),
            description: "Failed to create the web driver");
    }
}