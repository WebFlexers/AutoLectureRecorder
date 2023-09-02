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
        catch (WebDriverException ex)
        {
            _logger.LogError(ex, "A WebDriverException occurred while trying to start the Web Driver");
            return Errors.WebDriver.EdgeOutOfDate;
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
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Login.LoginCancelled;
            
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

            _driver.Url = "http://www.onepixelwebsite.com";

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            _driver.Url = urlWithoutOpenAppPrompt;

            // The first page's content will vary depending on whether the provided link is a direct meeting link
            // or a link that points to a microsoft team
            var nextElement = _driver.FindElement(
                By.XPath($"//button[@data-tid='{ElementNames.JoinMeeting.DirectLinkJoinOnWebId}' or " +
                                        $"@id='{ElementNames.JoinMeeting.TeamsLinkJoinOnWebId}']"));

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            bool isDirectMeetingLink = nextElement.GetAttribute("data-tid") == ElementNames.JoinMeeting.DirectLinkJoinOnWebId;

            if (isDirectMeetingLink)
            {
                nextElement.Click();
                
                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                
                _driver.FindElement(By.XPath(
                    $"//button[@translate-once='{ElementNames.JoinMeeting.ContinueWithoutAudioTranslateOnce}']"))
                    .Click();

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

                var dismissNotificationsButtons = _driver
                    .FindElements(By.XPath("//button[@title='Don’t ask again']"));
                
                if (dismissNotificationsButtons.Count > 0)
                {
                    dismissNotificationsButtons.First().Click();
                }

                _driver.Manage().Timeouts().ImplicitWait = _initialImplicitWait.Value;

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                // Switch to the frame containing the pre join meeting panel
                var iframeDiv1 =
                    _driver.FindElement(By.ClassName("experience-container-root"));
                var iframe1 = iframeDiv1.FindElement(By.TagName("iframe"));
                _driver.SwitchTo().Frame(iframe1);
                
                _driver.FindElement(By.XPath($"//a[@data-tid='{ElementNames.JoinMeeting.PreJoinSignInDataTid}']"))
                    .Click();
                
                _driver.SwitchTo().DefaultContent();

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                
                var errorOrLoggedIn = Login(academicEmailAddress, password, 
                    false, cancellationToken);
                if (errorOrLoggedIn.IsError)
                {
                    return errorOrLoggedIn.Errors;
                }

                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

                _driver.FindElement(By.Id(ElementNames.JoinMeeting.DontStaySignedInButtonId)).Click();
                
                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
                
                _driver.FindElement(
                    By.XPath($"//button[@translate-once='{ElementNames.JoinMeeting.ContinueWithoutAudioTranslateOnce}']"))
                    .Click();
                
                if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
            }
            else
            {
                return Errors.Meeting.LinkNotSupported;
            }

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            try
            {
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                _driver.FindElement(By.XPath
                        ($"//button[@title='{ElementNames.JoinMeeting.DismissNotificationButtonTitle}']"))
                    .Click();
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
            
            _driver.Manage().Timeouts().ImplicitWait = _initialImplicitWait.Value;
            
            // Switch to the frame containing the pre join meeting panel
            var iframeDiv2 =
                _driver.FindElement(By.ClassName("experience-container-root"));
            var iframe2 = iframeDiv2.FindElement(By.TagName("iframe"));
            _driver.SwitchTo().Frame(iframe2);
            
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            try
            {
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                var dismissCameraContainer = _driver.FindElement(
                    By.XPath($"//div[@class='{ElementNames.JoinMeeting.TurnOnCameraPopupContainerClass}']"));
                
                dismissCameraContainer.FindElement(
                    By.XPath($"//div[@class='{ElementNames.JoinMeeting.TurnOnCameraDismissSvgClass}']"))
                    .Click();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to click dismiss on the turn camera on popup");
            }
            
            _driver.Manage().Timeouts().ImplicitWait = _initialImplicitWait.Value;

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
            
            _driver.FindElement(By.Id(ElementNames.JoinMeeting.JoinMeetingButtonId)).Click();
            
            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;

            _driver.SwitchTo().DefaultContent();

            if (cancellationToken is { IsCancellationRequested: true }) return Errors.Meeting.JoinMeetingCancelled;
            
            _logger.LogInformation("Successfully joined meeting");
            
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
        var wait = new WebDriverWait(_driver, _driver!.Manage().Timeouts().ImplicitWait);
        wait.Until(driver => ((IJavaScriptExecutor)driver)
            .ExecuteScript("return document.readyState").Equals("complete"));
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
