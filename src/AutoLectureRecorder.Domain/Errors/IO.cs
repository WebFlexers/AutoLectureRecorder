using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;


public static partial class Errors
{
    public static class IO
    {
        public static Error FileDoesNotExist(string fileName) => Error.NotFound(
            code: nameof(FileDoesNotExist), 
            description: $"The file {fileName} does not exist");

        public static Error FailedToDownloadError(string fileName) => Error.Failure(
            code: nameof(FailedToDownloadError),
            description: $"Failed to download file {fileName}");

        public static Error FailedToDeleteFileOrDirectory(string fileOrDirectoryName) => Error.Failure(
            code: nameof(FailedToDeleteFileOrDirectory),
            description: $"Failed to delete file or directory {fileOrDirectoryName}");

        public static Error FailedToWriteToFile(string fileName) => Error.Failure(
            code: nameof(FailedToWriteToFile),
            description: $"Failed to write to file: {fileName}");
    }
}