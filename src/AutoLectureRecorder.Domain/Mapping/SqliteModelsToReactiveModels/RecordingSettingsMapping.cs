using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;

public static class RecordingSettingsMapping
{
    public static ReactiveRecordingSettings MapToReactive(this RecordingSettings input)
    {
        return new ReactiveRecordingSettings(
            input.RecordingsLocalPath, 
            input.OutputDeviceName,
            input.OutputDeviceFriendlyName,
            input.InputDeviceName,
            input.InputDeviceFriendlyName,
            Convert.ToBoolean(input.IsInputDeviceEnabled),
            input.Quality,
            input.Fps,
            input.OutputFrameWidth,
            input.OutputFrameHeight);
    }
}