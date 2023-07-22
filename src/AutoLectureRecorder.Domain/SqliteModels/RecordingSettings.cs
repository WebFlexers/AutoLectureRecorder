namespace AutoLectureRecorder.Domain.SqliteModels;

public record RecordingSettings
(
    string RecordingsLocalPath,
    string OutputDeviceName,
    string OutputDeviceFriendlyName,
    string InputDeviceName,
    string InputDeviceFriendlyName,
    int IsInputDeviceEnabled,
    int Quality,
    int Fps,
    int OutputFrameWidth,
    int OutputFrameHeight
)
{
    public RecordingSettings() : this(default!, default!, 
        default!, default!, default!,
        default!, default!, default!, default!, default!)
    {}
};
