namespace AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;

public interface ISqliteDataAccess
{
    /// <summary>
    /// Executes the given sql query with the given parameters and returns the result
    /// </summary>
    /// <typeparam name="T">Type of object to return</typeparam>
    /// <typeparam name="U">Type of parameters (usually dynamic)</typeparam>
    /// <param name="sqlStatement">The sql statement</param>
    /// <param name="parameters">The parameters</param>
    /// <param name="connectionStringName">The connection string name (e.g. Default)</param>
    /// <returns>A list of the given object type with the result of the query</returns>
    Task<List<T>> LoadData<T, U>(
        string sqlStatement,
        U parameters,
        string connectionStringName = "Default");

    /// <summary>
    /// Executes the given sql query with the given parameters
    /// </summary>
    /// <typeparam name="T">Type of parameters (usually dynamic)</typeparam>
    /// <param name="sqlStatement">The sql statement</param>
    /// <param name="parameters">The parameters</param>
    /// <param name="connectionStringName">The connection string name. The default is "Default"</param>
    /// <returns>An int indicating the number of rows affected</returns>
    Task<int> SaveData<T>(
        string sqlStatement,
        T parameters,
        string connectionStringName = "Default");

    Task BeginTransaction();

    /// <summary>
    /// Commits the pending transaction if it exists and disposes
    /// of the unmanaged resources
    /// </summary>
    void CommitPendingTransaction();

    /// <summary>
    /// Rolls back the pending transaction if it exists and disposes
    /// of the unmanaged resources
    /// </summary>
    void RollbackPendingTransaction();
}