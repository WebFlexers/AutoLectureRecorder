using AutoLectureRecorder.Services.WebDriver;
using Xunit.Abstractions;

namespace AutoLectureRecorder.WebDriver.UnitTests.WebDriver;

/// <summary>
/// Tests for UnipiEdgeWebDriver. Requires edge web driver in the tests output directory to work. Link:
/// https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/
/// Also put your academic account email (first line) and password (second line) in a file named 
/// academic_account.txt in the output directory of the tests.
/// This is done to avoid pushing sensitive info to source control
/// </summary>
public class UnipiEdgeWebDriverTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnipiEdgeWebDriverTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void LoginToMicrosoftTeams_1ShouldReturnTrueAndSuccessMessage()
    {
        var logger = XUnitLogger.CreateLogger<UnipiEdgeWebDriver>(_testOutputHelper);
        (bool, string) expected = (true, "success");

        string[] account = File.ReadAllLines("academic_account.txt");

        using var unipiWebDriver = new UnipiEdgeWebDriver(logger);
        unipiWebDriver.StartDriver(false, TimeSpan.FromSeconds(10));

        (bool, string) actual = unipiWebDriver.LoginToMicrosoftTeams(account[0], account[1]);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LoginToMicrosoftTeams_2ShouldReturnFalseAndInvalidCredentialsError()
    {
        var logger = XUnitLogger.CreateLogger<UnipiEdgeWebDriver>(_testOutputHelper);

        string[] account = File.ReadAllLines("academic_account.txt");

        using var unipiWebDriver = new UnipiEdgeWebDriver(logger);
        unipiWebDriver.StartDriver(false, TimeSpan.FromSeconds(10));

        (bool, string) actual = unipiWebDriver.LoginToMicrosoftTeams(account[0], "random_password_that_is_wrong_hopefully");

        Assert.False(actual.Item1);
    }

    // The test below is useful, but not deterministic, since the client will not always get throttle
    // and in that case this test fails

    //[Fact]
    //public void LoginToMicrosoftTeams_3ShouldReturnFalseAndTooManyRequestsError()
    //{
    //    var logger = XUnitLogger.CreateLogger<UnipiEdgeWebDriver>(_testOutputHelper);

    //    string[] account = File.ReadAllLines("academic_account.txt");

    //    using var unipiWebDriver = new UnipiEdgeWebDriver(logger);
    //    unipiWebDriver.StartDriver(false, TimeSpan.FromSeconds(10));

    //    bool IsThrottled = false;

    //    for (int i = 0; i < 10; i++)
    //    {
    //        (bool, string) actual = unipiWebDriver.LoginToMicrosoftTeams(account[0], "random_password_that_is_wrong_hopefully");
    //        Assert.False(actual.Item1);

    //        if (actual.Item2.Contains("Access Denied"))
    //        {
    //            IsThrottled = true;
    //            break;
    //        }
    //    }

    //    Assert.True(IsThrottled);
    //}
}