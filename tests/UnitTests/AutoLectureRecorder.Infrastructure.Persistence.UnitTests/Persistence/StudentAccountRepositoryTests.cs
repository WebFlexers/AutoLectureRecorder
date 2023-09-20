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

        var insertStudentAccountSql = 
            "insert into StudentAccount (RegistrationNumber, EmailAddress, EncryptedPassword, Entropy)" +
            "values ('p19000', 'p19000@unipi.gr', 'a_completely_fake_password', @Entropy)";
        
        await _fixture.DataAccess.SaveData(insertStudentAccountSql, new
        {
            Entropy = new byte[] { 2,3,5,4,3,2,5,2,8,7,3,4,8,5,2,0,3,4,7,5,2,3,4 }
        });
        
        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        // Act
        var studentAccount = await studentData.GetStudentAccount();

        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        studentAccount?.RegistrationNumber.Should().Be("p19000");
        studentAccount?.EmailAddress.Should().Be("p19000@unipi.gr");
        studentAccount?.EncryptedPassword.Should().Be("a_completely_fake_password");
    }

    [Fact]
    public async Task CreateStudentAccount_ShouldInsertAccountToTheDb()
    {
        // Arrange
        await _fixture.DataAccess.BeginTransaction();
        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        // Act
        var result = await studentData.InsertStudentAccount("p19165", 
            "p19165@unipi.gr", "a_random_not_real_password", new byte[]
                {2,3,5,4,3,2,5,2,8,7,3,4,8,5,2,0,3,4,7,5,2,3,4});

        _fixture.DataAccess.RollbackPendingTransaction();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteStudentAccount_ShouldDeleteAllRows()
    {
        // Arrange
        var insertStudentAccountSql = 
            "insert into StudentAccount (RegistrationNumber, EmailAddress, EncryptedPassword, Entropy)" +
            "values ('p19000', 'p19000@unipi.gr', 'a_completely_fake_password', @Entropy)";
        
        await _fixture.DataAccess.SaveData(insertStudentAccountSql, new
        {
            Entropy = new byte[] { 2,3,5,4,3,2,5,2,8,7,3,4,8,5,2,0,3,4,7,5,2,3,4 }
        });

        var studentData = new StudentAccountRepository(_fixture.DataAccess, _logger);

        // Act
        var result = await studentData.DeleteStudentAccount();

        // Assert
        result.Should().BeTrue();
    }
}