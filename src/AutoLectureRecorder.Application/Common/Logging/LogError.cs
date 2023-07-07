using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Application.Common.Logging;

public static partial class NavigationLogs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information,
        Message = "Error: '{errorCode} occurred: {description}'")]
    public static partial void EfficientlyLogInformation(this ILogger logger, string description, string errorCode);
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "{description}")]
    public static partial void EfficientlyLogInformation(this ILogger logger, string description);
    
    [LoggerMessage(EventId = 2, Level = LogLevel.Warning,
        Message = "Error: '{errorCode} occurred: {description}'")]
    public static partial void EfficientlyLogWarning(this ILogger logger, string description, string errorCode);
    [LoggerMessage(EventId = 3, Level = LogLevel.Warning,
        Message = "{description}")]
    public static partial void EfficientlyLogWarning(this ILogger logger, string description);
    
    [LoggerMessage(EventId = 4, Level = LogLevel.Error,
        Message = "Error: '{errorCode} occurred: {description}'")]
    public static partial void EfficientlyLogError(this ILogger logger, Exception ex, string description, string errorCode);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error,
        Message = "Error: '{errorCode} occurred: {description}'")]
    public static partial void EfficientlyLogError(this ILogger logger, string description, string errorCode);
    [LoggerMessage(EventId = 6, Level = LogLevel.Error,
        Message = "{description}")]
    public static partial void EfficientlyLogError(this ILogger logger, string description);
    
    
}