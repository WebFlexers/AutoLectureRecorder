using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Services.DataAccess.Repositories;

public class StudentAccountRepository : IStudentAccountRepository
{
    private readonly ISqliteDataAccess _dataAccess;
    private readonly ILogger<StudentAccountRepository> _logger;

    public StudentAccountRepository(ISqliteDataAccess dataAccess, ILogger<StudentAccountRepository> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
    }

    /// <summary>
    /// Gets the student account from the database asynchronously
    /// </summary>
    public async Task<ReactiveStudentAccount?> GetStudentAccountAsync()
    {
        try
        {
            string sql = "select * from StudentAccount";
            var result = await _dataAccess.LoadData<StudentAccount, dynamic>(sql, new { }).ConfigureAwait(false);
            var studentAccount = result.FirstOrDefault();

            if (studentAccount == null) return null;

            return new ReactiveStudentAccount
            {
                Id = studentAccount.Id,
                RegistrationNumber = studentAccount.RegistrationNumber,
                EmailAddress = studentAccount.EmailAddress,
                Password = studentAccount.Password
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to get student account");
            return null;
        }
    }

    /// <summary>
    /// Insert a new student account to the database asynchronously
    /// </summary>
    public async Task<bool> InsertStudentAccountAsync(string registrationNumber, string academicEmailAddress, string password)
    {
        try
        {
            var student = new StudentAccount
            {
                RegistrationNumber = registrationNumber,
                EmailAddress = academicEmailAddress,
                Password = password
            };

            string sql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                         "values (@RegistrationNumber, @EmailAddress, @Password)";

            int rowsAffectedNumber = await _dataAccess.SaveData(sql, student).ConfigureAwait(false);

            if (rowsAffectedNumber == 1)
            {
                _logger.LogInformation("Successfully inserted student account");
                return true;
            }

            _logger.LogWarning("Failed to insert student account with registration number {rn}", registrationNumber);
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to insert student account");
            return false;
        }
    }

    /// <summary>
    /// Wipe out the student account table asynchronously
    /// </summary>
    public async Task<bool> DeleteStudentAccountAsync()
    {
        try
        {
            string sql = "delete from StudentAccount";
            await _dataAccess.SaveData<dynamic>(sql, new { }).ConfigureAwait(false);
            _logger.LogInformation("Successfully deleted student account");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to delete student account");
            return false;
        }
    }
}
