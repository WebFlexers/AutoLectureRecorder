using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AutoLectureRecorder.Services.DataAccess;

public class SqliteDataAccess : ISqliteDataAccess
{
    private readonly IConfiguration? _config;
    private readonly string _connectionString;

    public SqliteDataAccess(IConfiguration config)
    {
        _config = config;
        _connectionString = config.GetConnectionString("Default")!;
    }

    public SqliteDataAccess(string connectionString)
    {
        _connectionString = connectionString;
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
    public async Task<List<T>> LoadData<T, U>(
            string sqlStatement,
            U parameters,
            string connectionStringName = "Default")
    {
        string connectionString = _connectionString;

        using IDbConnection connection = new SqliteConnection(connectionString);

        var rows = await connection.QueryAsync<T>(
            sqlStatement,
            parameters).ConfigureAwait(false);

        return rows.ToList();
    }

    /// <summary>
    /// Executes the given sql query with the given parameters
    /// </summary>
    /// <typeparam name="T">Type of parameters (usually dynamic)</typeparam>
    /// <param name="sqlStatement">The sql statement</param>
    /// <param name="parameters">The parameters</param>
    /// <param name="connectionStringName">The connection string name. The default is "Default"</param>
    /// <returns>An int indicating the number of rows affected</returns>
    public async Task<int> SaveData<T>(
        string sqlStatement,
        T parameters,
        string connectionStringName = "Default")
    {
        string connectionString = _connectionString;

        using IDbConnection connection = new SqliteConnection(connectionString);

        return await connection.ExecuteAsync(
            sqlStatement,
            parameters).ConfigureAwait(false);
    }
}
