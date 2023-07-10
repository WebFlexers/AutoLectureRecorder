using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;

public static class ReactiveRecordingSettingsMapping
{
    public static RecordingSettings MapToSqliteModel(this ReactiveRecordingSettings input)
    {
        return new RecordingSettings(
            input.RecordingsLocalPath, 
            input.OutputDeviceName,
            input.OutputDeviceFriendlyName,
            input.InputDeviceName,
            input.InputDeviceFriendlyName,
            Convert.ToInt32(input.IsInputDeviceEnabled),
            input.Quality,
            input.Fps,
            input.OutputFrameWidth,
            input.OutputFrameHeight);
    }
}