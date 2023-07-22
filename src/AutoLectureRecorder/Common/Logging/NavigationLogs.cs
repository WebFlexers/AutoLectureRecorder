using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Common.Logging;

public static partial class NavigationLogs
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Information,
        Message = "Navigated to {navigationTarget}")]
    public static partial void LogSuccessfulNavigation(this ILogger logger, string navigationTarget);
    
    [LoggerMessage(EventId = 1001, Level = LogLevel.Warning,
        Message = "Failed to navigate back, since there was no view model left on the stack")]
    public static partial void LogFailedBackNavigation(this ILogger logger);
    
    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning,
        Message = "Failed to navigate forward, since there was no view model left on the stack")]
    public static partial void LogFailedForwardNavigation(this ILogger logger);
}