using AutoLectureRecorder.Services.DataAccess;
using Microsoft.Extensions.Configuration;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

public class StudentAccountDataTests
{
    [Fact]
    public async void CreateStudentAccount_ShouldInsertAccountToTheDb()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var studentData = new StudentAccountRepository(dataAccess);

        await studentData.InsertStudentAccountAsync("p19165", "p19165@unipi.gr", "a_random_not_real_password");
    }

    [Fact]
    public async void DeleteStudentAccount_ShouldDeleteAllRows()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var studentData = new StudentAccountRepository(dataAccess);

        await studentData.DeleteStudentAccountAsync();
    }

    [Fact]
    public async void GetStudentAccount_ShouldGetTheStudentAccount()
    {
        var dataAccess = new SqliteDataAccess(DataAccessMockHelper.CreateConfiguration());
        var studentData = new StudentAccountRepository(dataAccess);

        await studentData.DeleteStudentAccountAsync();
        await studentData.InsertStudentAccountAsync("p19165", "p19165@unipi.gr", "a_random_not_real_password");
        var studentAccount = await studentData.GetStudentAccountAsync()!;

        Assert.Equal("p19165", studentAccount.RegistrationNumber);
        Assert.Equal("p19165@unipi.gr", studentAccount.EmailAddress);
        Assert.Equal("a_random_not_real_password", studentAccount.Password);
    }
}
