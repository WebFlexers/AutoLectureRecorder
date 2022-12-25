using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AutoLectureRecorder.Services.DataAccess;

public class SqliteDataAccess : ISqliteDataAccess
{
    private readonly IConfiguration _config;

    public SqliteDataAccess(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Executes the given sql query with the given parameters and returns the result
    /// </summary>
    /// <typeparam name="T">Type of object to return</typeparam>
    /// <typeparam name="U">Type of parameters (usually dynamic)</typeparam>
    /// <param name="sqlStatement">The sql statement</param>
    /// <param name="parameters">The parameters</param>
    /// <param name="connectionStringName">The connection string name (e.g. Default)</param>
    /// <returns>A list of the given object type with the result of the query</returns>
    public List<T> LoadData<T, U>(
            string sqlStatement,
            U parameters,
            string? connectionStringName = null)
    {
        if (connectionStringName == null)
        {
            connectionStringName = "Default";
        }

        string connectionString = _config.GetConnectionString(connectionStringName)!;

        using IDbConnection connection = new SqliteConnection(connectionString);

        var rows = connection.Query<T>(
            sqlStatement,
            parameters);

        return rows.ToList();
    }

    /// <summary>
    /// Executes the given sql query with the given parameters
    /// </summary>
    /// <typeparam name="T">Type of parameters (usually dynamic)</typeparam>
    /// <param name="sqlStatement">The sql statement</param>
    /// <param name="parameters">The parameters</param>
    /// <param name="connectionStringName">The connection string name. The default is "Default"</param>
    /// <returns>A list of the given object type with the result of the query</returns>
    public void SaveData<T>(
        string sqlStatement,
        T parameters,
        string? connectionStringName = null)
    {
        if (connectionStringName == null)
        {
            connectionStringName = "Default";
        }

        string connectionString = _config.GetConnectionString(connectionStringName)!;

        using IDbConnection connection = new SqliteConnection(connectionString);

        connection.Execute(
            sqlStatement,
            parameters);
    }
}
