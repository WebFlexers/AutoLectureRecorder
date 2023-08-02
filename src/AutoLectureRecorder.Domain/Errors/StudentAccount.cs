using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors {

    public static class StudentAccount
    {
        public static Error StudentAccountNotFound => Error.Failure(
            code: nameof(StudentAccountNotFound),
            description: "Student account was null");
    }
}
