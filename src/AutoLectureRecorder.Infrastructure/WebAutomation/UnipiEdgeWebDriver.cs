using System.Reactive;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Domain.Errors;
using AutoLectureRecorder.Infrastructure.WebAutomation.WebElementNames;
using ErrorOr;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace AutoLectureRecorder.Infrastructure.WebAutomation;

public class UnipiEdgeWebDriver : IAlrWebDriver
{
    public const string MicrosoftTeamsAuthUrl = @"https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";
    private EdgeDriver? _driver;
    private readonly ILogger<UnipiEdgeWebDriver> _logger;

    public UnipiEdgeWebDriver(ILogger<UnipiEdgeWebDriver> logger)
    {
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public ErrorOr<Unit> StartDriver(bool useWebView, TimeSpan implicitWaitTime, 
        string debuggerAddress)
    {
        try
        {
            EdgeOptions edgeOptions = new EdgeOptions();
            var driverPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AutoLectureRecorder", "WebDriver");
            var driverService = EdgeDriverService.CreateDefaultService(driverPath);
            driverService.HideCommandPromptWindow = true;

            edgeOptions.AddArgument("disable-notifications");

            if (useWebView)
            {
                edgeOptions.UseWebView = true;
                edgeOptions.DebuggerAddress = debuggerAddress;
            }

            _driver = new EdgeDriver(driverService, edgeOptions);
            _driver.Manage().Timeouts().ImplicitWait = implicitWaitTime;

            return Unit.Default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to start the Web Driver");
            return Errors.WebDriver.FailedToCreateWebDriver;
        }
    }
    
    /// <inheritdoc/>
    public ErrorOr<Unit> Login(string academicEmailAddress, string password, bool resolveInitialUrlAutomatically = true,
        CancellationToken? cancellationToken = null)
    {
        if (_driver == null)
        {
            return Errors.Login.NullDriver;
        }

        try
        {
            if (resolveInitialUrlAutomatically)
            {
                _driver.Url = MicrosoftTeamsAuthUrl;
            }

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Login.LoginCancelled;

            // Turn email characters to lower, because capital letters fail the login process 
            _driver.FindElement(By.Id(ElementNames.Login.MicrosoftEmailFieldId)).SendKeys(academicEmailAddress.ToLower());
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Login.LoginCancelled;

            // Submit to go to University login page
            _driver.FindElement(By.XPath(ElementNames.Login.MicrosoftSubmitButtonXPath)).Click();
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Login.LoginCancelled;

            return EnterUniversityCredentials(academicEmailAddress, password, cancellationToken);
        }
        catch (Exception ex)
        {
            var errorDescription = "An error occurred while trying to login to microsoft teams";
            _logger.LogError(ex, "{ErrorDescription}", errorDescription);
            return Error.Unexpected(description: errorDescription);
        }
    }

    private ErrorOr<Unit> EnterUniversityCredentials(string academicEmailAddress, string password,
        CancellationToken? cancellationToken = null)
    {
        // Split the academic email address to get the registration number for login
        _driver!.FindElement(By.Id(ElementNames.Login.UnipiUsernameId)).SendKeys(academicEmailAddress.Split("@")[0]);

        _driver.FindElement(By.Id(ElementNames.Login.UnipiPasswordId)).SendKeys(password);
        if (cancellationToken is { IsCancellationRequested: true }) return Errors.Login.LoginCancelled;

        _driver.FindElement(By.Id(ElementNames.Login.UnipiSubmitButtonId)).Click();
        if (cancellationToken is { IsCancellationRequested: true }) return Errors.Login.LoginCancelled;

        // class: banner banner-danger banner-dismissible -> Wrong credentials in unipi auth page
        // class: mdc-card p-4 w-lg-66 m-auto -> Too many requests in unipi auth page
        // class: row text-title -> Successful login
        var loginResultElement = _driver.FindElement(
            By.XPath($"//div[@class='{ElementNames.Login.UnipiWrongCredentialsClass}' or " +
                           $"@class='{ElementNames.Login.UnipiTooManyRequestsClass}' or " +
                           $"@class='{ElementNames.Login.UnipiSuccessfulLoginClass}']"));

        var resultMessage = loginResultElement.Text;
        var tagName = loginResultElement.TagName;
        var className = loginResultElement.GetAttribute("class");
        _logger.LogDebug("TagName: {Tag}", tagName);
        _logger.LogDebug("ClassName: {Class}", className);

        if (loginResultElement.GetAttribute("class").Trim().Equals(ElementNames.Login.UnipiSuccessfulLoginClass))
        {
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Login.LoginCancelled;

            _logger.LogInformation("Successful login of {Email}", academicEmailAddress);
            return Unit.Default;
        }

        return Errors.Login.WrongCredentials(resultMessage);
    }
    
    private TimeSpan? _initialImplicitWait;

    /// <inheritdoc/>
    public ErrorOr<Unit> JoinMeeting(string academicEmailAddress, string password,
        string meetingLink, TimeSpan meetingDuration, CancellationToken? cancellationToken = null)
    {
        if (_driver == null)
        {
            return Errors.Meeting.NullDriver;
        }

        if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

        // Store the implicit wait time to revert to it after changing it
        _initialImplicitWait ??= _driver.Manage().Timeouts().ImplicitWait;
        
        try
        {
            _driver.Url = meetingLink;
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            // To handle the open microsoft teams app popup we can't use the selenium popup manager, because this one is OS level.
            // Instead when the page loads a new url is generated which doesn't show the popup if navigated to.
            // Refreshing the page still opens the popup. New tab isn't available in WebView2 in WPF. So to avoid the popup we
            // navigate to a very lightweight website, like onepixelwebsite.com and then to the new url
            WaitUntilLoaded();
            var urlWithoutOpenAppPrompt = _driver.Url;
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            _driver.Url = "http://www.onepixelwebsite.com/";

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            _driver.Url = urlWithoutOpenAppPrompt;

            // id: openTeamsClientInBrowser -> Exists when the link is a meeting link
            // data-tid: joinOnWeb -> Exists when the link is a direct meeting link
            var nextElement = _driver.FindElement(
                By.XPath("//button[@data-tid='joinOnWeb' or @id='openTeamsClientInBrowser']"));

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            bool isDirectMeetingLink = nextElement.GetAttribute("data-tid") == "joinOnWeb";
            bool isTeamLink = nextElement.GetAttribute("id") == "openTeamsClientInBrowser";

            if (isDirectMeetingLink)
            {
                nextElement.Click();
                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                nextElement = _driver.FindElement(
                    By.XPath("//button[@translate-once='getUserMedia_continue_button' or @id='loginButton']"));

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                if (nextElement.GetAttribute("id").Trim() == "loginButton")
                {
                    EnterUniversityCredentials(academicEmailAddress, password, cancellationToken);
                }

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                _driver.FindElement(
                    By.XPath("//button[@translate-once='getUserMedia_continue_button']")).Click();

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

                var dismissNotificationsButtons = _driver
                    .FindElements(By.XPath("//button[@title='Don’t ask again']"));
                
                if (dismissNotificationsButtons.Count > 0)
                {
                    dismissNotificationsButtons.First().Click();
                }

                _driver.Manage().Timeouts().ImplicitWait = _initialImplicitWait.Value;

                // Here we will have to check whether we are signed in or not
                // class: anonymous-pre-join-footer -> Exists when the user is NOT logged in
                // class: ExperienceContainerWrap-experience-container-ffb71058-4663-4e21-b94b-750cc5c43f2f ->
                //        Refers to the iframe containing the meeting. Exists when the user is logged in
                nextElement = _driver.FindElement(
                    By.XPath("//div[@class='anonymous-pre-join-footer' or @class='experience-container-root']"));

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                bool userIsNotLoggedIn = nextElement.GetAttribute("class") == "anonymous-pre-join-footer";

                if (userIsNotLoggedIn)
                {
                    var signInAnchor = nextElement.FindElement(By.TagName("a"));
                    signInAnchor.Click();

                    if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                    nextElement = _driver.FindElement(
                        By.XPath($"//div[@id='username' or @data-test-id='{academicEmailAddress}']"));

                    if (nextElement.GetAttribute("id").Trim() == "username")
                    {
                        var errorOrLoggedIn = Login(academicEmailAddress, password, 
                            false, cancellationToken);
                        
                        if (errorOrLoggedIn.IsError) return errorOrLoggedIn.Errors;
                    }
                    else if (nextElement.GetAttribute("data-test-id").Trim() == academicEmailAddress)
                    {
                        nextElement.Click();
                        if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                        EnterUniversityCredentials(academicEmailAddress, password, cancellationToken);
                    }

                    if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                    _driver.FindElement(By.Id("idSIButton9")).Click();

                    _driver.FindElement(
                        By.XPath("//button[@translate-once='getUserMedia_continue_button']")).Click();
                    if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                }
            }
            else if (isTeamLink)
            {
                // class: powerbar-profile fadeable -> Exists in the meeting page if the user is already logged in
                // class: form-group col-md-24 -> Exists in the login page if the user is NOT already logged in
                // id: username -> Exists in the unipi login page if we are redirected there
                // data-test-id: '{academicEmailAddress}' -> Exists in the screen where the logged in account is selected
                WaitUntilLoaded();
                nextElement = _driver.FindElement(
                    By.XPath($"//div[@class='powerbar-profile fadeable' or @class='form-group col-md-24' or @id='username' or @data-test-id='{academicEmailAddress}']"));

                var id = nextElement.GetAttribute("id");
                var dataTestId = nextElement.GetAttribute("data-test-id");
                var classNameTrimmed = nextElement.GetAttribute("class").Trim();
                _logger.LogDebug("JoinMeeting: Stage 1 ClassName -> {class}", classNameTrimmed);
                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                if (classNameTrimmed.Equals("form-group col-md-24"))
                {
                    var errorOrLoggedIn = Login(academicEmailAddress, password, 
                        false, cancellationToken);
                    
                    if (errorOrLoggedIn.IsError) return errorOrLoggedIn.Errors;
                    
                    _driver.FindElement(By.Id("idSIButton9")).Click();
                }
                else if (id.Trim().Equals("username"))
                {
                    EnterUniversityCredentials(academicEmailAddress, password, cancellationToken);

                    if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                    _driver.FindElement(By.Id("idSIButton9")).Click();
                }
                else if (dataTestId != null && dataTestId.Trim() == academicEmailAddress)
                {
                    nextElement.Click();
                    if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                    EnterUniversityCredentials(academicEmailAddress, password, cancellationToken);

                    if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                    _driver.FindElement(By.Id("idSIButton9")).Click();
                }

                // Get this element to make sure the page has loaded before reducing the implicit wait
                // to find the notifications popup and disable it only if it exists
                _driver.FindElement(By.XPath("//div[@class='powerbar-profile fadeable']"));
                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                var maximumWaitDateTime = DateTime.UtcNow.Add(meetingDuration);

                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                bool clickedJoinButton = false;
                do
                {
                    try
                    {
                        if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                        _driver.FindElement(By.XPath("//button[contains(@data-tid, 'join-btn')]")).Click();
                        clickedJoinButton = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogTrace(ex, "Couldn't find join button. Waiting...");
                    }
                } while (DateTime.UtcNow < maximumWaitDateTime);

                // TODO: Check for multiple multiple Join buttons and click the latest one (rare)
                if (clickedJoinButton == false) return Errors.Meeting.MeetingDidntStart;
            }

            _driver.Manage().Timeouts().ImplicitWait = _initialImplicitWait.Value;

            // Continue without audio or video
            var iframeDiv =
                _driver.FindElement(By.ClassName("experience-container-root"));
            var iframe = iframeDiv.FindElement(By.TagName("iframe"));
            _driver.SwitchTo().Frame(iframe);
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.MeetingDidntStart;

            _driver.FindElement(By.Id("prejoin-join-button")).Click();
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.MeetingDidntStart;

            _driver.SwitchTo().DefaultContent();

            try
            {
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                _driver.FindElement(By.XPath("//button[@title='Dismiss']")).Click();
            }
            catch (NoSuchElementException ex)
            {
                _logger.LogWarning("Dismiss button popup not found. Moving on...");
            }
            catch (ElementClickInterceptedException ex)
            {
                _logger.LogWarning("Dismiss button popup was intercepted. Ignoring it...");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to click the Dismiss popup button");
            }

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.MeetingDidntStart;

            const string successMessage = "Successfully joined meeting";
            _logger.LogInformation(successMessage);
            return Unit.Default;
        }
        catch (Exception ex)
        {
            var error = Error.Unexpected(
                description: "Failed to join the meeting. Check your internet connection. Also if you have " +
                             "changed your academic password logout and login again");
            _logger.LogError(ex, "{ErrorDescription}", error.Description);
            return error;
        }
    }

    private void WaitUntilLoaded()
    {
        var wait = new WebDriverWait(_driver, _driver.Manage().Timeouts().ImplicitWait);
        wait.Until(driver => ((IJavaScriptExecutor)driver)
            .ExecuteScript("return document.readyState").Equals("complete"));
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
