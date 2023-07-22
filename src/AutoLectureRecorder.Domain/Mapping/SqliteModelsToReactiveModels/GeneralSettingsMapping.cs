using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.SqliteModelsToReactiveModels;

public static class GeneralSettingsMapping
{
    public static ReactiveGeneralSettings MapToReactive(this GeneralSettings input)
    {
        return new ReactiveGeneralSettings(Convert.ToBoolean(input.OnCloseKeepAlive));
    }
}