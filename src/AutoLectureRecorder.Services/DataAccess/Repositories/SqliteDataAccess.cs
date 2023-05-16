using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AutoLectureRecorder.Services.DataAccess.Repositories;

public class SqliteDataAccess : ISqliteDataAccess
{
    private readonly IConfiguration? _config;
    private readonly string _connectionString;

    private SqliteConnection? _connectionWithTransaction;
    private SqliteTransaction? _transaction;
    private bool _useTransactions = false;

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
        if (_useTransactions == false)
        {
            using IDbConnection connection = new SqliteConnection(_connectionString);

            var rows = await connection.QueryAsync<T>(
                sqlStatement,
                parameters).ConfigureAwait(false);

            return rows.ToList();
        }

        var rowsWithTransaction = await _connectionWithTransaction.QueryAsync<T>(
            sqlStatement,
            parameters).ConfigureAwait(false);

        return rowsWithTransaction.ToList();
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
        if (_useTransactions == false)
        {
            using IDbConnection connection = new SqliteConnection(_connectionString);

            return await connection.ExecuteAsync(
                sqlStatement,
                parameters).ConfigureAwait(false);
        }

        return await _connectionWithTransaction.ExecuteAsync(
            sqlStatement,
            parameters).ConfigureAwait(false);
    }

    public async Task BeginTransaction()
    {
        _connectionWithTransaction?.Dispose();
        _transaction?.Dispose();

        _connectionWithTransaction = new SqliteConnection(_connectionString);
        await _connectionWithTransaction.OpenAsync().ConfigureAwait(false);
        _transaction = _connectionWithTransaction.BeginTransaction();
        _useTransactions = true;
    }

    /// <summary>
    /// Commits the pending transaction if it exists and disposes
    /// of the unmanaged resources
    /// </summary>
    public void CommitPendingTransaction()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
        _connectionWithTransaction?.Dispose();
        _useTransactions = false;
    }

    /// <summary>
    /// Rolls back the pending transaction if it exists and disposes
    /// of the unmanaged resources
    /// </summary>
    public void RollbackPendingTransaction()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _connectionWithTransaction?.Dispose();
        _useTransactions = false;
    }
}
