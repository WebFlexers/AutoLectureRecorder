namespace AutoLectureRecorder.Services.DataAccess;

public interface ISqliteDataAccess
{
    Task<List<T>> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName = "Default");
    Task SaveData<T>(string sqlStatement, T parameters, string connectionStringName = "Default");
}