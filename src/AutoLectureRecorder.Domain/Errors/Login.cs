using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors
{
    public static class Login
    {
        public static Error LoginCancelled => Error.Custom((int)ExtraErrorTypes.Cancelled,
            code: nameof(LoginCancelled),
            description: "The login process was cancelled. If you want to continue, input your credentials and try again");

        public static Error NullDriver => Error.Custom((int)ExtraErrorTypes.NullArgument,
            code: nameof(NullDriver),
            description: "Attempted to login to microsoft teams with a null web driver");

        public static Error WrongCredentials(string description) => Error.Validation(
            code: nameof(WrongCredentials),
            description: description);
    }
}