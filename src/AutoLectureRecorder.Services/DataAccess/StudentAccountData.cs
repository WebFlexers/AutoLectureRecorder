using AutoLectureRecorder.Data.Models;
using AutoLectureRecorder.Data.ReactiveModels;

namespace AutoLectureRecorder.Services.DataAccess;

public class StudentAccountData : IStudentAccountData
{
    private readonly ISqliteDataAccess _dataAccess;

    public StudentAccountData(ISqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task InsertStudentAccountAsync(string registrationNumber, string academicEmailAddress, string password)
    {
        StudentAccount student = new StudentAccount
        {
            RegistrationNumber = registrationNumber,
            EmailAddress = academicEmailAddress,
            Password = password
        };

        string sql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                     "values (@RegistrationNumber, @EmailAddress, @Password)";

        await _dataAccess.SaveData(sql, student).ConfigureAwait(false);
    }

    public async Task<ReactiveStudentAccount>? GetStudentAccountAsync()
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

    public async Task DeleteStudentAccountAsync()
    {
        string sql = "delete from StudentAccount";
        await _dataAccess.SaveData<dynamic>(sql, new { }).ConfigureAwait(false);
    }
}
