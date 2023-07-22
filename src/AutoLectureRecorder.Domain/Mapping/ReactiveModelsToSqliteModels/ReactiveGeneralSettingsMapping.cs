using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Domain.Mapping.ReactiveModelsToSqliteModels;

public static class ReactiveGeneralSettingsMapping
{
    public static GeneralSettings MapToSqliteModel(this ReactiveGeneralSettings input)
    {
        return new GeneralSettings(Convert.ToInt32(input.OnCloseKeepAlive));
    }
}