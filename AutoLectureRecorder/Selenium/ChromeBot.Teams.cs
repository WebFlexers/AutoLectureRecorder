using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using SeleniumExtras.WaitHelpers;

namespace AutoLectureRecorder.Selenium
{
    public partial class ChromeBot
    {
        public string teamsHomePagetUrl = "https://teams.microsoft.com/_#/school//?ctx=teamsGrid";
        public bool onMeeting = false;

        private IWebElement WaitToFindElement(By locator)
        {
            return WaitToFindElement(locator, TimeSpan.FromSeconds(WAIT_SECONDS));
        }
        private IWebElement WaitToFindElement(By locator, TimeSpan waitTime)
        {
            WebDriverWait wait = new WebDriverWait(_driver, waitTime);
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(locator));
            return _driver.FindElement(locator);
        }

        private IWebElement WaitToFindEitherElement(By[] locators)
        {
            return WaitToFindEitherElement(locators, TimeSpan.FromSeconds(WAIT_SECONDS));
        }

        private IWebElement WaitToFindEitherElement(By[] locators, TimeSpan waitTime)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    return new WebDriverWait(_driver, waitTime).Until(AnyElementExists(locators));
                }
                catch (StaleElementReferenceException)
                {
                    Thread.Sleep(2000);
                }
            }
            return null;
        }

        private Func<IWebDriver, IWebElement> AnyElementExists(By[] locators)
        {
            return (driver) =>
            {
                foreach (By locator in locators)
                {
                    IReadOnlyCollection<IWebElement> elements = _driver.FindElements(locator);
                    if (elements.Any())
                    {
                        return elements.ElementAt(0);
                    }
                }

                return null;
            };
        }

        public bool AuthenticateUser(string registrationNum, string password, ref string errorMessage)
        {            
            try
            {
                if (_driver == null)
                   StartDriver();

                //-----------------------------------Microsoft's Login Page-----------------------------------
                _driver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";

                // Turn everything to lower case because capital letters create problems
                string temp = registrationNum.ToLower();
                registrationNum = temp;

                //Filling email at Microsoft's login page
                var emailInputBox = WaitToFindElement(By.Id("i0116"));
                emailInputBox.SendKeys(registrationNum + "@unipi.gr");

                //Click NextBtn at Microsoft's login page
                var nextEmailBtn = WaitToFindElement(By.XPath("//input[@type='submit']"));
                nextEmailBtn.Click();

                //-----------------------------------Unipi page-----------------------------------

                //Filling UNIPI form
                var usernameUnipiInputBox = WaitToFindElement(By.Id("username"));
                usernameUnipiInputBox.SendKeys(registrationNum);
                var passwordUnipiInputBox = WaitToFindElement(By.Id("password"));
                passwordUnipiInputBox.SendKeys(password);

                //Login button
                var loginUnipiBtn = _driver.FindElement(By.Id("loginButton"));
                loginUnipiBtn.Click();

                By[] locators = { By.Id("msg"), By.XPath("//*[@id='idBtn_Back']") };
                var foundElement = WaitToFindEitherElement(locators);
                if (string.IsNullOrWhiteSpace(foundElement.Text))
                {
                    Trace.WriteLine("The no button text is: " + foundElement.Text);
                    return true;
                }
                else
                {
                    errorMessage = foundElement.Text;
                    Trace.WriteLine("The error is: " + errorMessage);
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                TerminateDriver();
                errorMessage = "An unusual error occured while authenticating user. Try again";
                Trace.WriteLine(errorMessage + ": " + ex.Message);
                return false;
            }
        }

        public bool ConnectToMeetingByName(string meetingName, string registrationNum, string password)
        {                      
            try
            {
                onMeeting = false;

                if (_driver == null)
                    StartDriver();

                GoToTeams(registrationNum, password);

                //Disable the stupid turn on notifications popup
                var notificationsDisable = WaitToFindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div/button[2]"), TimeSpan.FromMinutes(2));
                notificationsDisable.Click();

                //Clicking to specific team
                var lessonCardBtn = WaitToFindElement(By.XPath("//div[contains(@data-tid, '" + meetingName + "')]"));
                lessonCardBtn.Click();

                var joinBtn = WaitToFindElement(By.XPath("//button[contains(@data-tid, 'join-btn')]"), TimeSpan.FromMinutes(30));
                joinBtn.Click();

                var noAudioMicBtn = WaitToFindElement(By.XPath("//button[contains(@track-summary, 'Continue in call/meetup without device access')]"));
                noAudioMicBtn.Click();

                var preJoinCallBtn = WaitToFindElement(By.XPath("//button[contains(@data-tid, 'prejoin-join-button')]"));
                preJoinCallBtn.Click();

                onMeeting = true;
                return true;
            }
            catch (StaleElementReferenceException)
            {
                TerminateDriver();
                StartDriver();
                return ConnectToMeetingByName(meetingName, registrationNum, password);
            }
            catch (NoSuchWindowException)
            {
                TerminateDriver();
                MessageBox.Show("Chrome closed unexpectedly. Try to reschedule the meeting");
                return false;
            }
            catch (WebDriverException ex)
            {
                TerminateDriver();
                if (ex.Message.Contains("chrome not reachable"))
                {                    
                    MessageBox.Show("Chrome closed unexpectedly. Try to reschedule the meeting");                    
                }
                return false;
            }
            catch (Exception ex)
            {            
                TerminateDriver();
                Trace.WriteLine("An error occured while connecting to meeting: " + ex.Message);
                Trace.WriteLine("Exception type: " + ex.GetType().ToString());
                return false;
            }
        }

        public int GetParticipantsNumber()
        {
            return 0;
        }

        public bool GoToTeams(string registrationNum, string password)
        {
            try
            {
                //-----------------------------------Microsoft's Login Page-----------------------------------
                _driver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";

                // Turn everything to lower case because capital letters create problems
                string temp = registrationNum.ToLower();
                registrationNum = temp;

                //Filling email at Microsoft's login page
                var emailInputBox = WaitToFindElement(By.Id("i0116"));
                emailInputBox.SendKeys(registrationNum + "@unipi.gr");

                //Click NextBtn at Microsoft's login page
                var nextEmailBtn = WaitToFindElement(By.XPath("//input[@type='submit']"));
                nextEmailBtn.Click();

                //-----------------------------------Unipi page-----------------------------------

                //Filling UNIPI form
                var usernameUnipiInputBox = WaitToFindElement(By.Id("username"));
                usernameUnipiInputBox.SendKeys(registrationNum);
                var passwordUnipiInputBox = WaitToFindElement(By.Id("password"));
                passwordUnipiInputBox.SendKeys(password);

                //Login button
                var loginUnipiBtn = WaitToFindElement(By.Id("submitForm"));
                loginUnipiBtn.Click();

                //Click no on Stay signed in
                var noStaySignIn = WaitToFindElement(By.XPath("//*[@id='idBtn_Back']"));
                noStaySignIn.Click();

                // Select microsoft account to continue by clicking on the account connected above
                var selectAcc = WaitToFindElement(By.XPath("//*[@class='table']"));
                selectAcc.Click();

                return true;
            }
            catch (Exception e)
            {
                TerminateDriver();
                Trace.WriteLine("Error on go to teams: " + e.Message);
                return false;
            }
        }

        public List<string> GetMeetings()
        {
            try
            {
                if (_driver == null)
                {
                    return default;
                }

                // Click no to the promt from user authentication
                var noButton = WaitToFindElement(By.XPath("//*[@id='idBtn_Back']"));
                noButton.Click();

                // Select microsoft account to continue by clicking on the account connected above
                var selectAcc = WaitToFindElement(By.XPath("//*[@class='table']"));
                selectAcc.Click();

                List<string> meetingsList = new List<string>();
                //Find cards container              
                var cardsContainer = WaitToFindElement(By.XPath("//*[@id='favorite-teams-panel']"), TimeSpan.FromSeconds(60));

                //Get text from cards
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(WAIT_SECONDS));
                var cardsList = wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//h1[@data-tid='team-name-text']"))); // cardsContainer.FindElements(By.XPath("//h1[@data-tid='team-name-text']"));

                foreach (var card in cardsList)
                {
                    Trace.WriteLine(card.Text);
                    meetingsList.Add(card.Text);
                }

                return meetingsList;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("An error occured while getting meetings: " + ex.Message);
                TerminateDriver();
                return default;
            }            
        }
    }
}