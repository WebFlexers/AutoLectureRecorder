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

namespace AutoLectureRecorder.Selenium
{
    partial class ChromeBot
    {
        const string cookieFileName = "profile.alr";

        public string teamsHomePagetUrl = "https://teams.microsoft.com/_#/school//?ctx=teamsGrid";
        public bool onMeeting = false;

        public bool AuthenticateUser(string AM, string password)
        {
            try
            {
                HideBrowser = true;
                StartDriver();

                //-----------------------------------Microsoft's Login Page-----------------------------------
                driver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";

                // Turn everything to lower case because capital letters create problems
                string temp = AM.ToLower();
                AM = temp;

                //Filling email at Microsoft's login page
                IWebElement emailInputBox = driver.FindElement(By.Id("i0116"));
                emailInputBox.SendKeys(AM + "@unipi.gr");

                //Click NextBtn at Microsoft's login page
                IWebElement nextEmailBtn = driver.FindElement(By.Id("idSIButton9"));
                nextEmailBtn.Click();

                SaveCookiesToList();
                Thread.Sleep(3000);

                //-----------------------------------Unipi page-----------------------------------
                //Filling UNIPI form
                IWebElement usernameUnipiInputBox = driver.FindElement(By.Id("username"));
                usernameUnipiInputBox.SendKeys(AM);
                IWebElement passwordUnipiInputBox = driver.FindElement(By.Id("password"));
                passwordUnipiInputBox.SendKeys(password);

                //Login button
                IWebElement loginUnipiBtn = driver.FindElement(By.Id("submitForm"));
                loginUnipiBtn.Click();

                SaveCookiesToList();
                Thread.Sleep(3000);

                //Stay sign in page
                IWebElement NoStaySignInBtn = driver.FindElement(By.Id("idSIButton9"));
                NoStaySignInBtn.Click();

                SaveCookiesToList();
                Thread.Sleep(3000);

                //Serialize cookieList
                SaveCookiesToFile(cookiesList);

                TerminateDriver();

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("An error occured while authenticating user: " + ex.Message);
                return false;
            }
        }

        public void GoToTeamsMenu()
        {
            StartDriver();
            LoadCookies(null, cookieFileName);
        }

        public void ConnectToMeetingByName(string name)
        {
            try
            {
                if (driver == null || !isDriverRunning)
                {
                    StartDriver();
                    LoadCookies(null, cookieFileName);
                    Thread.Sleep(3000);
                }
                else
                {
                    if (!driver.Url.Equals(teamsHomePagetUrl))
                    {
                        if (onMeeting) LeaveMeeting();
                        driver.Url = teamsHomePagetUrl;
                    }
                }
                Thread.Sleep(3000);

                //Clicking to specific team
                IWebElement lessonCardBtn = driver.FindElement(By.XPath("//div[contains(@data-tid, '" + name + "')]"));
                lessonCardBtn.Click();

                //Wait 20 minutes until Join button appears
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20*60));
                IWebElement joinBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(@data-tid, 'join-btn')]")));
                joinBtn.Click();

                IWebElement noAudioMicBtn = driver.FindElement(By.XPath("//button[contains(@track-summary, 'Continue in call/meetup without device access')]"));
                noAudioMicBtn.Click();

                IWebElement preJoinCallBtn = driver.FindElement(By.XPath("//button[contains(@data-tid, 'prejoin-join-button')]"));
                preJoinCallBtn.Click();

                onMeeting = true;
            }   
            catch (Exception ex)
            {            
                TerminateDriver();
                Console.WriteLine("An error occured while connecting to meeting: " + ex.Message);
                throw ex;
            }
        }

        private void LeaveMeeting()
        {
            if (!onMeeting) return;

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            IWebElement leaveCallBtn = driver.FindElement(By.Id("roster-button"));
            //try to click the button
            try
            {
                leaveCallBtn.Click();
                onMeeting = false;
            }
            //Hover mouse to make the tool bar appears
            catch
            {
                //Simulate mouse hover to toggle toolbar
                Actions actions = new Actions(driver);
                actions.MoveToElement(leaveCallBtn).Perform();
                leaveCallBtn.Click();

                onMeeting = false;
            }           
        }

        public int GetParticipantsNumber()
        {
            if (!onMeeting) return 0;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            int participants = 0;
            IWebElement showParticipantsBtn = driver.FindElement(By.Id("roster-button"));
            try
            {                                
                var participantsList = driver.FindElements(By.XPath("//li[contains(@data-tid, 'participantsInCall')]"));
                foreach (var p in participantsList)
                    participants++;

                return participants;
            }
            catch
            {
                //Simulatε mouse hover to toggle toolbar
                Actions actions = new Actions(driver);
                actions.MoveToElement(showParticipantsBtn).Perform();
                showParticipantsBtn.Click();

                //Creating a list with all the participants
                var participantsList = driver.FindElements(By.XPath("//li[contains(@data-tid, 'participantsInCall')]"));
                foreach (var p in participantsList)
                    participants++;

                return participants;
            }      
        }

        public List<string> GetMeetings()
        {
            try
            {
                if (driver == null || !isDriverRunning)
                {
                    StartDriver();
                    LoadCookies(null, cookieFileName);
                    Thread.Sleep(3000);
                }
                else
                {
                    if (!driver.Url.Equals(teamsHomePagetUrl))                   
                        driver.Url = teamsHomePagetUrl;                   
                }         
                
                List<string> meetingsList = new List<string>();
                //Find cards container
                Thread.Sleep(5000);
                IWebElement cardsContainer = driver.FindElement(By.XPath("//*[@id='favorite-teams-panel']"));
                //Retrieve number of cards
                int cardsNum = int.Parse(cardsContainer.GetAttribute("set-size"));
                Console.WriteLine("Number of cards: " + cardsNum);
                //Get text from cards
                Thread.Sleep(3000);
                var cardsList = driver.FindElements(By.XPath("//*[@class='team-name-text']"));
                foreach (var card in cardsList)
                    meetingsList.Add(card.Text);

                TerminateDriver();

                return meetingsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while getting meetings: " + ex.Message);
                TerminateDriver();
                return default;
            }            
        }
    }
}