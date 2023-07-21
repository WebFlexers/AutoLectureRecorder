namespace AutoLectureRecorder.Domain.SqliteModels;

public class RecordingsStorageDirectory
{
    public string Path { get; set; }

    public RecordingsStorageDirectory(string path)
    {
        Path = path;
    }
}