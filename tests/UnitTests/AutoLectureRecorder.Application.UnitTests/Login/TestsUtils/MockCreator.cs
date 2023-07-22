using System.Reactive;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using ErrorOr;
using Moq;

namespace AutoLectureRecorder.Application.UnitTests.Login.TestsUtils;

public static class MockCreator
{
    public static Mock<IWebDriverFactory> CreateWebDriverFactoryMockForHappyPath()
    {
        var mockWebDriver = new Mock<IAlrWebDriver>();
        mockWebDriver.Setup(wd => wd.Login(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()))
            .Returns(Unit.Default);
        
        var mockWebDriverFactory = new Mock<IWebDriverFactory>();
        mockWebDriverFactory.Setup(f => f.CreateUnipiEdgeWebDriver(
                It.IsAny<bool>(), 
                It.IsAny<TimeSpan>(), 
                It.IsAny<string>()))
            .Returns(new AlrWebDriverWrapper(mockWebDriver.Object));

        return mockWebDriverFactory;
    }
    
    public static Mock<IWebDriverFactory> CreateWebDriverFactoryMockForFailedToCreateDriver()
    {
        var mockWebDriver = new Mock<IAlrWebDriver>();
        mockWebDriver.Setup(wd => wd.Login(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()))
            .Returns(Unit.Default);
        
        var mockWebDriverFactory = new Mock<IWebDriverFactory>();
        mockWebDriverFactory.Setup(f => f.CreateUnipiEdgeWebDriver(
                It.IsAny<bool>(), 
                It.IsAny<TimeSpan>(), 
                It.IsAny<string>()))
            .Returns(new Error());

        return mockWebDriverFactory;
    }
    
    public static Mock<IWebDriverFactory> CreateWebDriverFactoryMockForFailedToLogin()
    {
        var mockWebDriver = new Mock<IAlrWebDriver>();
        mockWebDriver.Setup(wd => wd.Login(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()))
            .Returns(new Error());
        
        var mockWebDriverFactory = new Mock<IWebDriverFactory>();
        mockWebDriverFactory.Setup(f => f.CreateUnipiEdgeWebDriver(
                It.IsAny<bool>(), 
                It.IsAny<TimeSpan>(), 
                It.IsAny<string>()))
            .Returns(new AlrWebDriverWrapper(mockWebDriver.Object));

        return mockWebDriverFactory;
    }
}