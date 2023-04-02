namespace AutoLectureRecorder.Services.DataAccess.Interfaces;

public interface ISqliteDataAccess
{
    Task<List<T>> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName = "Default");
    Task<int> SaveData<T>(string sqlStatement, T parameters, string connectionStringName = "Default");
}