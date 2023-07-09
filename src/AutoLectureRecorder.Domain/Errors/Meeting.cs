using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors
{
    public static class Meeting
    {
        public static Error NullDriver => Error.Custom((int)ExtraErrorTypes.NullArgument,
            code: nameof(NullDriver),
            description: "Attempted to join microsoft teams meeting with a null web driver");
        
        public static Error JoinMeetingCancelled => Error.Custom((int)ExtraErrorTypes.Cancelled,
            code: nameof(JoinMeetingCancelled),
            description: "The proccess of joinning the Microsoft Teams meeting was cancelled by the user");

        public static Error MeetingDidntStart => Error.NotFound(
            code: nameof(MeetingDidntStart),
            description: "The meeting didn't start in time");
    }
}