using System.Reactive;
using AutoLectureRecorder.Application.Login.Queries;
using AutoLectureRecorder.Application.UnitTests.Login.TestsUtils;
using FluentAssertions;

namespace AutoLectureRecorder.Application.UnitTests.Login;

public class LoginToMicrosoftTeamsHandlerTests
{
    [Fact]
    public async Task LoginToMicrosoftTeamsHandler_WhenLoginSucceeds_ShouldReturnUnit()
    {
        // Arrange
        var loginToMicrosoftTeamsQuery = LoginToMicrosoftTeamsQueryUtils.CreateQuery();
        var webDriverFactoryMock = MockCreator.CreateWebDriverFactoryMockForHappyPath();
        var handler = new LoginToMicrosoftTeamsHandler(webDriverFactoryMock.Object);
        
        // Act
        var result = await handler.Handle(loginToMicrosoftTeamsQuery, default);

        // Assert
        // TODO: Student account is added to repository  
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Unit>();
    }
    
    [Fact]
    public async Task LoginToMicrosoftTeamsHandler_WhenDriverFailsToBeCreated_ShouldReturnError()
    {
        // Arrange
        var loginToMicrosoftTeamsQuery = LoginToMicrosoftTeamsQueryUtils.CreateQuery();
        var webDriverFactoryMock = MockCreator.CreateWebDriverFactoryMockForFailedToCreateDriver();
        var handler = new LoginToMicrosoftTeamsHandler(webDriverFactoryMock.Object);
        
        // Act
        var result = await handler.Handle(loginToMicrosoftTeamsQuery, default);

        // Assert
        // TODO: Student account is not added to repository  
        result.IsError.Should().BeTrue();
    }
    
    [Fact]
    public async Task LoginToMicrosoftTeamsHandler_WhenLoginFails_ShouldReturnError()
    {
        // Arrange
        var loginToMicrosoftTeamsQuery = LoginToMicrosoftTeamsQueryUtils.CreateQuery();
        var webDriverFactoryMock = MockCreator.CreateWebDriverFactoryMockForFailedToLogin();
        var handler = new LoginToMicrosoftTeamsHandler(webDriverFactoryMock.Object);

        // Act
        var result = await handler.Handle(loginToMicrosoftTeamsQuery, default);

        // Assert
        // TODO: Student account is not added to repository 
        result.IsError.Should().BeTrue();
    }
}