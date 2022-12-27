using AutoLectureRecorder.Services.WebDriver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace AutoLectureRecorder.UnitTests.Services.WebDriver;

/// <summary>
/// Tests for UnipiEdgeWebDriver. Requires edge web driver in the tests output directory to work. Link:
/// https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/
/// Also put your academic account password in a file named academic_account_password.txt in the output directory of tests.
/// This is done to avoid pushing sensitive info to source control
/// </summary>
public class UnipiEdgeWebDriverTests
{
    [Fact]
    public void LoginToMicrosoftTeams_ShouldReturnTrueAndSuccessMessage()
    {
        var logger = CreateLogger<UnipiEdgeWebDriver>();
        (bool, string) expected = (true, "success");

        using var unipiWebDriver = new UnipiEdgeWebDriver(logger, false, TimeSpan.FromSeconds(10));

        string password = File.ReadAllText("academic_account_password.txt");
        (bool, string) actual = unipiWebDriver.LoginToMicrosoftTeams("P19165@unipi.gr", password);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LoginToMicrosoftTeams_ShouldReturnFalseAndInvalidCredentialsError()
    {
        var logger = CreateLogger<UnipiEdgeWebDriver>();
        bool expected = false;

        using var unipiWebDriver = new UnipiEdgeWebDriver(logger, false, TimeSpan.FromSeconds(10));

        (bool, string) actual = unipiWebDriver.LoginToMicrosoftTeams("P19165@unipi.gr", "random_password_that_is_wrong_hopefully");

        Assert.Equal(expected, actual.Item1);
    }

    private ILogger<T> CreateLogger<T>()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        return factory!.CreateLogger<T>();
    }
}
