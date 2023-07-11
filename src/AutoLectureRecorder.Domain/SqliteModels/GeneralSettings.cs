namespace AutoLectureRecorder.Domain.SqliteModels;

public record GeneralSettings(int OnCloseKeepAlive)
{
    public GeneralSettings() : this(0) { }
};