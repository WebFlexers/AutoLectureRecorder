using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutoLectureRecorder.Services.WebDriver;

public class UnipiEdgeWebDriver : IWebDriver, IDisposable
{
    public const string MICROSOFT_TEAMS_AUTH_URL = @"https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";
    private EdgeDriver? _driver;
    private readonly ILogger<UnipiEdgeWebDriver> _logger;


    public UnipiEdgeWebDriver(ILogger<UnipiEdgeWebDriver> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a web driver backed by the Edge browser.
    /// </summary>
    /// <param name="useWebView">Whether to use the integrated microsoft EdgeWebView2 or not</param>
    /// <param name="implicitWaitTime">The time to wait when searching for elements before throwing exception</param>
    /// <param name="debuggerAddress">The debugger address that will host the WebView2</param>
    public void StartDriver(bool useWebView, TimeSpan implicitWaitTime, string debuggerAddress = "localhost:9222")
    {
        EdgeOptions edgeOptions = new EdgeOptions();
        var driverService = EdgeDriverService.CreateDefaultService();
        driverService.HideCommandPromptWindow = true;

        if (useWebView)
        {
            edgeOptions.UseWebView = true;
            edgeOptions.DebuggerAddress = debuggerAddress;
        }

        _driver = new EdgeDriver(driverService, edgeOptions);
        _driver.Manage().Timeouts().ImplicitWait = implicitWaitTime;
    }

    /// <summary>
    /// Login to Microsoft Teams through the web driver using the provided credentials
    /// </summary>
    /// <param name="academicEmailAddress"></param>
    /// <param name="password"></param>
    public (bool result, string resultMessage) LoginToMicrosoftTeams(string academicEmailAddress, string password)
    {
        if (_driver == null)
        {
            throw new NullReferenceException("Attempted to login to microsoft teams with a null web driver");
        }

        try
        {
            _driver.Url = MICROSOFT_TEAMS_AUTH_URL;

            // Turn email characters to lower, because capital letters fail the login proccess 
            _driver.FindElement(By.Id("i0116")).SendKeys(academicEmailAddress.ToLower());

            // Submit to go to University login page
            _driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Split the academic email address to get the registration number for login
            _driver.FindElement(By.Id("username")).SendKeys(academicEmailAddress.Split("@")[0]);

            _driver.FindElement(By.Id("password")).SendKeys(password);

            _driver.FindElement(By.Id("loginButton")).Click();

            // class: banner banner-danger banner-dismissible -> Wrong credentials in unipi auth page
            // class: mdc-card p-4 w-lg-66 m-auto -> Too many requests in unipi auth page
            // class: row text-title -> Successful login
            var unipiBadCredentialsErrorElement = _driver.FindElement(
                By.XPath("//div[@class='banner banner-danger banner-dismissible' or @class='mdc-card p-4 w-lg-66 m-auto' or @class='row text-title']"));

            var resultMessage = unipiBadCredentialsErrorElement.Text;
            var tagName = unipiBadCredentialsErrorElement.TagName;
            var className = unipiBadCredentialsErrorElement.GetAttribute("class");
            _logger.LogDebug("TagName: {tag}", tagName);
            _logger.LogDebug("ClassName: {class}", className);

            if (unipiBadCredentialsErrorElement.GetAttribute("class").Trim().Equals("row text-title"))
            {
                _logger.LogInformation("Successful login of {email}", academicEmailAddress);
                return (true, "success");
            }

            _logger.LogWarning("Authentication failed. Error message: {message}", resultMessage);

            return (false, resultMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError("Login failed with exception: {ex}", ex);
            return (false, "Login failed. Check your credentials and try again");
        }
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
