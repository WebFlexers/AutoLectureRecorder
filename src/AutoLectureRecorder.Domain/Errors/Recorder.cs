using ErrorOr;

namespace AutoLectureRecorder.Domain.Errors;

public static partial class Errors
{
    public static class Recorder
    {
        public static Error NoVideosFound => Error.NotFound(
            code: nameof(NoVideosFound),
            description: "No videos found in the specified directories");
        
    }
}
