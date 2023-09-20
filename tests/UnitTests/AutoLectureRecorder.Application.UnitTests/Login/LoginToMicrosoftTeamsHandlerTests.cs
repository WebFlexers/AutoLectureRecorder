using System.Reactive;
using AutoLectureRecorder.Application.Common.Abstractions.Encryption;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Login.Queries.LoginToMicrosoftTeams;
using AutoLectureRecorder.Application.UnitTests.Login.TestsUtils;
using FluentAssertions;
using Moq;

namespace AutoLectureRecorder.Application.UnitTests.Login;

public class LoginToMicrosoftTeamsHandlerTests
{
    [Fact]
    public async Task LoginToMicrosoftTeamsHandler_WhenLoginSucceeds_ShouldReturnUnit()
    {
        // Arrange
        var loginToMicrosoftTeamsQuery = LoginToMicrosoftTeamsQueryUtils.CreateQuery();
        var webDriverFactoryMock = MockCreator.CreateWebDriverFactoryMockForHappyPath();
        var studentRepositoryMock = new Mock<IStudentAccountRepository>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var handler = new LoginToMicrosoftTeamsHandler(webDriverFactoryMock.Object,
            studentRepositoryMock.Object, encryptionServiceMock.Object);
        
        // Act
        var result = await handler.Handle(loginToMicrosoftTeamsQuery, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Unit>();
        studentRepositoryMock.Verify(x => x.DeleteStudentAccount(), Times.Once);
        studentRepositoryMock.Verify(x => x.InsertStudentAccount(
            It.IsAny<string>(), 
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<byte[]>()) ,Times.Once);
    }
    
    [Fact]
    public async Task LoginToMicrosoftTeamsHandler_WhenDriverFailsToBeCreated_ShouldReturnError()
    {
        // Arrange
        var loginToMicrosoftTeamsQuery = LoginToMicrosoftTeamsQueryUtils.CreateQuery();
        var webDriverFactoryMock = MockCreator.CreateWebDriverFactoryMockForFailedToCreateDriver();
        var studentRepositoryMock = new Mock<IStudentAccountRepository>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var handler = new LoginToMicrosoftTeamsHandler(webDriverFactoryMock.Object, 
            studentRepositoryMock.Object, encryptionServiceMock.Object);
        
        // Act
        var result = await handler.Handle(loginToMicrosoftTeamsQuery, default);

        // Assert
        result.IsError.Should().BeTrue();
        studentRepositoryMock.Verify(x => x.DeleteStudentAccount(), Times.Never);
        studentRepositoryMock.Verify(x => x.InsertStudentAccount(
            It.IsAny<string>(), 
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<byte[]>()), Times.Never);
    }
    
    [Fact]
    public async Task LoginToMicrosoftTeamsHandler_WhenLoginFails_ShouldReturnError()
    {
        // Arrange
        var loginToMicrosoftTeamsQuery = LoginToMicrosoftTeamsQueryUtils.CreateQuery();
        var webDriverFactoryMock = MockCreator.CreateWebDriverFactoryMockForFailedToLogin();
        var studentRepositoryMock = new Mock<IStudentAccountRepository>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var handler = new LoginToMicrosoftTeamsHandler(webDriverFactoryMock.Object,
            studentRepositoryMock.Object, encryptionServiceMock.Object);

        // Act
        var result = await handler.Handle(loginToMicrosoftTeamsQuery, default);

        // Assert
        result.IsError.Should().BeTrue();
        studentRepositoryMock.Verify(x => x.DeleteStudentAccount(), Times.Never);
        studentRepositoryMock.Verify(x => x.InsertStudentAccount(
            It.IsAny<string>(), 
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<byte[]>()), Times.Never);
    }
}