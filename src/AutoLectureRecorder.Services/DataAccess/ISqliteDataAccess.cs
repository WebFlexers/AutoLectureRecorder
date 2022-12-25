namespace AutoLectureRecorder.Services.DataAccess;

public interface ISqliteDataAccess
{
    List<T> LoadData<T, U>(string sqlStatement, U parameters, string? connectionStringName = null);
    void SaveData<T>(string sqlStatement, T parameters, string? connectionStringName = null);
}