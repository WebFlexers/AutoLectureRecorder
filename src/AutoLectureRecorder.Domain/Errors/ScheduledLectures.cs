using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors
{
    public static class ScheduledLectures
    {
        public static Error FailedToCreate => Error.Failure(
            code: nameof(FailedToCreate),
            description: "Failed to create a scheduled lecture");

        public static Error FailedToUpdate => Error.Failure(
            code: nameof(FailedToUpdate),
            description: "Failed to update a scheduled lecture");
    }
}