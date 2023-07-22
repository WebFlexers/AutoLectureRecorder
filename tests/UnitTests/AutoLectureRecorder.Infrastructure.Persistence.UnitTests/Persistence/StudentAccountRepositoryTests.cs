using AutoLectureRecorder.Infrastructure.Persistence.UnitTests.TestUtils.Fixture;
using CommonTestsUtils.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoLectureRecorder.Infrastructure.Persistence.UnitTests.Persistence;

[Collection("DatabaseCollection")]
public class StudentAccountRepositoryTests
{
    private readonly DbInitializerFixture _fixture;
    private readonly ILogger<StudentAccountRepository> _logger;

    public StudentAccountRepositoryTests(ITestOutputHelper output, DbInitializerFixture fixture)
    {
        _fixture = fixture;
        _logger = XUnitLogger.CreateLogger<StudentAccountRepository>(output);
    }

    [Fact]
    public async void GetStudentAccount_ShouldGetTheStudentAccount()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();

        var insertStudentAccountSql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                                      "values ('p19000', 'p19000@unipi.gr', 'a_completely_fake_password')";
        await _fixture.DataAccess.SaveData(insertStudentAccountSql, new { });
        
        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        // Act
        var studentAccount = await studentData.GetStudentAccount();

        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        studentAccount?.RegistrationNumber.Should().Be("p19000");
        studentAccount?.EmailAddress.Should().Be("p19000@unipi.gr");
        studentAccount?.Password.Should().Be("a_completely_fake_password");
    }

    [Fact]
    public async Task CreateStudentAccount_ShouldInsertAccountToTheDb()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        // Act
        var result = await studentData.InsertStudentAccount("p19165", 
            "p19165@unipi.gr", "a_random_not_real_password");

        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteStudentAccount_ShouldDeleteAllRows()
    {
        // Arrange
        var insertStudentAccountSql = "insert into StudentAccount (RegistrationNumber, EmailAddress, Password)" +
                                            "values ('p19000', 'p19000@unipi.gr', 'a_completely_fake_password')";
        await _fixture.DataAccess.SaveData(insertStudentAccountSql, new { });

        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        // Act
        var result = await studentData.DeleteStudentAccount();

        // Assert
        result.Should().BeTrue();
    }
}