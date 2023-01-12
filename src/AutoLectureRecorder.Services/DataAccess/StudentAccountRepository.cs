using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;

public class StudentAccountRepository : IStudentAccountRepository
{
    private readonly ISqliteDataAccess _dataAccess;

    public StudentAccountRepository(ISqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    /// <summary>
    /// Insert a new student account to the database asynchronously
    /// </summary>
    public async Task InsertStudentAccountAsync(string registrationNumber, string academicEmailAddress, string password)
    {
        var student = new StudentAccount
        {
            RegistrationNumber = registrationNumber,
            EmailAddress = academicEmailAddress,
            Password = password
        };

        string sql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                     "values (@RegistrationNumber, @EmailAddress, @Password)";

        await _dataAccess.SaveData(sql, student).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the student account from the database asynchronously
    /// </summary>
    public async Task<ReactiveStudentAccount?> GetStudentAccountAsync()
    {
        string sql = "select * from StudentAccount";
        var result = await _dataAccess.LoadData<StudentAccount, dynamic>(sql, new { }).ConfigureAwait(false);
        var studentAccount = result.FirstOrDefault();

        if (result.Count == 0)
        {
            return null;
        }

        return new ReactiveStudentAccount
        {
            Id = studentAccount.Id,
            RegistrationNumber = studentAccount.RegistrationNumber,
            EmailAddress = studentAccount.EmailAddress,
            Password = studentAccount.Password
        };
    }

    /// <summary>
    /// Wipe out the student account table asynchronously
    /// </summary>
    public async Task DeleteStudentAccountAsync()
    {
        string sql = "delete from StudentAccount";
        await _dataAccess.SaveData<dynamic>(sql, new { }).ConfigureAwait(false);
    }
}
