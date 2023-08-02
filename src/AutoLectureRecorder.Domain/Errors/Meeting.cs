using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors
{
    public static class Meeting
    {
        public static Error NullMeetingLink => Error.Custom((int)ExtraErrorTypes.NullArgument,
            code: nameof(NullMeetingLink),
            description: "The meeting link was null");
        
        public static Error NullDriver => Error.Custom((int)ExtraErrorTypes.NullArgument,
            code: nameof(NullDriver),
            description: "Attempted to join microsoft teams meeting with a null web driver");
        
        public static Error JoinMeetingCancelled => Error.Custom((int)ExtraErrorTypes.Cancelled,
            code: nameof(JoinMeetingCancelled),
            description: "The process of joining the Microsoft Teams meeting was cancelled by the user");

        public static Error LinkNotSupported => Error.Unexpected(
            code: nameof(LinkNotSupported),
            description: "The meeting link that was provided is not supported. Links that point to microsoft teams" +
                         "are not supported yet. Use a direct meeting link instead");
        
        public static Error MeetingDidntStart => Error.NotFound(
            code: nameof(MeetingDidntStart),
            description: "The meeting didn't start in time");
    }
}