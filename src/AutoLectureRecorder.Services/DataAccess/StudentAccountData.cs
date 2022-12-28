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

    public async Task InsertStudentAccount(string registrationNumber, string academicEmailAddress, string password)
    {
        StudentAccount student = new StudentAccount
        {
            RegistrationNumber = registrationNumber,
            EmailAddress = academicEmailAddress,
            Password = password
        };

        string sql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                     "values (@RegistrationNumber, @EmailAddress, @Password)";

        await _dataAccess.SaveData(sql, student);
    }

    public async Task<ReactiveStudentAccount>? GetStudentAccount()
    {
        string sql = "select * from StudentAccount";
        var result = await _dataAccess.LoadData<StudentAccount, dynamic>(sql, new { });
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

    public async Task DeleteStudentAccount()
    {
        string sql = "delete from StudentAccount";
        await _dataAccess.SaveData<dynamic>(sql, new { });
    }
}
