using AutoLectureRecorder.Services.DataAccess.Repositories;
using AutoLectureRecorder.UnitTests.Fixture;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.UnitTests.Services.DataAccess;

[Collection("DatabaseCollection")]
public class StudentAccountRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<StudentAccountRepository> _logger;

    public StudentAccountRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _output = output;
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<StudentAccountRepository>(output);
    }

    [Fact]
    public async void GetStudentAccount_ShouldGetTheStudentAccount()
    {
        await _fixture.DataAccess.BeginTransaction();

        var insertStudentAccountSql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                                      "values ('p19000', 'p19000@unipi.gr', 'a_completely_fake_password')";
        await _fixture.DataAccess.SaveData(insertStudentAccountSql, new { });

        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        var studentAccount = await studentData.GetStudentAccountAsync();

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.Equal("p19000", studentAccount?.RegistrationNumber);
        Assert.Equal("p19000@unipi.gr", studentAccount?.EmailAddress);
        Assert.Equal("a_completely_fake_password", studentAccount?.Password);
    }

    [Fact]
    public async Task CreateStudentAccount_ShouldInsertAccountToTheDb()
    {
        await _fixture.DataAccess.BeginTransaction();
        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        var result = await studentData.InsertStudentAccountAsync("p19165", 
            "p19165@unipi.gr", "a_random_not_real_password");

        _fixture.DataAccess.RollbackPendingTransaction();

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteStudentAccount_ShouldDeleteAllRows()
    {
        var insertStudentAccountSql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                                            "values ('p19000', 'p19000@unipi.gr', 'a_completely_fake_password')";
        await _fixture.DataAccess.SaveData(insertStudentAccountSql, new { });

        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        var result = await studentData.DeleteStudentAccountAsync();

        Assert.True(result);
    }
}
