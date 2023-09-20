using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Domain.SqliteModels;
using Microsoft.Extensions.Logging;

namespace AutoLectureRecorder.Infrastructure.Persistence;

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
    public async Task<ReactiveStudentAccount?> GetStudentAccount()
    {
        try
        {
            string sql = "select Id, RegistrationNumber, EmailAddress, EncryptedPassword, Entropy from StudentAccount";
            var result = await _dataAccess.LoadData<StudentAccount, dynamic>(
                    sql, new { }).ConfigureAwait(false);
            var studentAccount = result.FirstOrDefault();

            if (studentAccount == null) return null;

            return new ReactiveStudentAccount
            (
                registrationNumber: studentAccount.RegistrationNumber,
                emailAddress: studentAccount.EmailAddress,
                encryptedPassword: studentAccount.EncryptedPassword,
                entropy: studentAccount.Entropy
            );
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
    public async Task<bool> InsertStudentAccount(string registrationNumber, string academicEmailAddress, string encryptedPassword, 
        byte[] entropy)
    {
        try
        {
            var student = new StudentAccount
            (
                RegistrationNumber: registrationNumber,
                EmailAddress: academicEmailAddress,
                EncryptedPassword: encryptedPassword,
                Entropy: entropy
            );

            string sql = "insert into StudentAccount (RegistrationNumber, EmailAddress, EncryptedPassword, Entropy)" +
                         "values (@RegistrationNumber, @EmailAddress, @EncryptedPassword, @Entropy)";

            int rowsAffectedNumber = await _dataAccess.SaveData(sql, student).ConfigureAwait(false);

            if (rowsAffectedNumber == 1)
            {
                _logger.LogInformation("Successfully inserted student account");
                return true;
            }

            _logger.LogWarning("Failed to insert student account with registration number {Rn}", 
                registrationNumber);
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
    public async Task<bool> DeleteStudentAccount()
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
