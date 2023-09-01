using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors
{
    public static class WebDriver
    {
        public static Error EdgeOutOfDate => Error.Failure(
            code: nameof(EdgeOutOfDate),
            description: "AutoLectureRecorder relies on Microsoft Edge to work. You need to update Edge to the latest version. " +
                         "To do that usually you just need to open Microsoft Edge and it will be done for you. Once edge is updated " +
                         "restart AutoLectureRecorder and try to login again.");
        public static Error FailedToCreateWebDriver => Error.Failure(
            code: nameof(FailedToCreateWebDriver),
            description: "An unknown error occurred while trying to create an automatic web driver");
    }
}