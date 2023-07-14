using System.Data;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Options;
using Dapper;
using Microsoft.Data.Sqlite;

namespace AutoLectureRecorder.Infrastructure.Persistence;

public class SqliteDataAccess : ISqliteDataAccess
{
    private readonly string _connectionString;
    
    private SqliteConnection? _connectionWithTransaction;
    private SqliteTransaction? _transaction;
    private bool _useTransactions = false;

    public SqliteDataAccess()
    {
        _connectionString = SqliteOptions.ConnectionString;
    }

    public SqliteDataAccess(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<List<T>> LoadData<T, U>(
            string sqlStatement,
            U parameters)
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

    /// <inheritdoc/>
    public async Task<int> SaveData<T>(
        string sqlStatement,
        T parameters)
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

    /// <inheritdoc/>
    public void CommitPendingTransaction()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
        _connectionWithTransaction?.Dispose();
        _useTransactions = false;
    }

    /// <inheritdoc/>
    public void RollbackPendingTransaction()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _connectionWithTransaction?.Dispose();
        _useTransactions = false;
    }
}