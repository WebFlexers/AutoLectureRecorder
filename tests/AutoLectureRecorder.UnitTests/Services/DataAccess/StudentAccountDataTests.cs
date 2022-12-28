using AutoLectureRecorder.Services.DataAccess;
using Microsoft.Extensions.Configuration;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

public class StudentAccountDataTests
{
    [Fact]
    public async void CreateStudentAccount_ShouldInsertAccountToTheDb()
    {
        var dataAccess = new SqliteDataAccess(CreateConfiguration());
        var studentData = new StudentAccountData(dataAccess);

        await studentData.InsertStudentAccount("p19165", "p19165@unipi.gr", "a_random_not_real_password");
    }

    [Fact]
    public async void DeleteStudentAccount_ShouldDeleteAllRows()
    {
        var dataAccess = new SqliteDataAccess(CreateConfiguration());
        var studentData = new StudentAccountData(dataAccess);

        await studentData.DeleteStudentAccount();
    }

    [Fact]
    public async void GetStudentAccount_ShouldGetTheStudentAccount()
    {
        var dataAccess = new SqliteDataAccess(CreateConfiguration());
        var studentData = new StudentAccountData(dataAccess);

        await studentData.DeleteStudentAccount();
        await studentData.InsertStudentAccount("p19165", "p19165@unipi.gr", "a_random_not_real_password");
        var studentAccount = await studentData.GetStudentAccount()!;

        Assert.Equal("p19165", studentAccount.RegistrationNumber);
        Assert.Equal("p19165@unipi.gr", studentAccount.EmailAddress);
        Assert.Equal("a_random_not_real_password", studentAccount.Password);
    }

    private IConfiguration CreateConfiguration()
    {
        Dictionary<string, string?>? inMemorySettings = new Dictionary<string, string?> 
        {
            {"Main", "Dictionary Main Value"},
            {"ConnectionStrings:Default", "Data Source=.\\AutoLectureRecorderDB.db;"},
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
}
