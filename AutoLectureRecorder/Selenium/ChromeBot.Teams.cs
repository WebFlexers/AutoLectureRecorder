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

namespace AutoLectureRecorder.Selenium
{
    public partial class ChromeBot
    {
        static string cookieFileName; //"profile.alr";

        public string teamsHomePagetUrl = "https://teams.microsoft.com/_#/school//?ctx=teamsGrid";
        public bool onMeeting = false;

        public bool AuthenticateUser(string registrationNum, string password, ref string errorMessage)
        {            
            try
            {
                if (driver == null)
                   StartDriver();

                //-----------------------------------Microsoft's Login Page-----------------------------------
                driver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";

                // Turn everything to lower case because capital letters create problems
                string temp = registrationNum.ToLower();
                registrationNum = temp;

                // form-control ltr_override input ext-input text-box ext-text-box has-error ext-has-error

                //Filling email at Microsoft's login page
                var emailInputBox = driver.FindElement(By.Id("i0116"));
                emailInputBox.SendKeys(registrationNum + "@unipi.gr");

                //Click NextBtn at Microsoft's login page
                var nextEmailBtn = driver.FindElement(By.XPath("//input[@type='submit']"));
                nextEmailBtn.Click();

                //-----------------------------------Unipi page-----------------------------------

                //Filling UNIPI form
                var usernameUnipiInputBox = driver.FindElement(By.Id("username"));
                usernameUnipiInputBox.SendKeys(registrationNum);
                var passwordUnipiInputBox = driver.FindElement(By.Id("password"));
                passwordUnipiInputBox.SendKeys(password);

                //Login button
                var loginUnipiBtn = driver.FindElement(By.Id("submitForm"));
                loginUnipiBtn.Click();

                // Check for errors
                ImplicitWait(TimeSpan.FromSeconds(5));
                try
                {
                    var error = driver.FindElement(By.Id("msg"));
                    ImplicitWait(TimeSpan.FromSeconds(WAIT_SECONDS));
                    errorMessage = "Wrong registration number or password";
                    return false;
                }
                catch (NoSuchElementException e)
                {
                    ImplicitWait(TimeSpan.FromSeconds(WAIT_SECONDS));
                    //Click no on Stay signed in
                    var noStaySignIn = driver.FindElement(By.XPath("//*[@id='idBtn_Back']"));
                    noStaySignIn.Click();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TerminateDriver();
                errorMessage = "An error occured while authenticating user. Try again";
                Trace.WriteLine(errorMessage + ": " + ex.Message);
                return false;
            }
        }

        public bool ConnectToMeetingByName(string meetingName, string registrationNum, string password)
        {                      
            try
            {
                onMeeting = false;

                if (driver == null)
                    StartDriver();

                GoToTeams(registrationNum, password);

                //Clicking to specific team
                ImplicitWait(TimeSpan.FromSeconds(120));
                var lessonCardBtn = driver.FindElement(By.XPath("//div[contains(@data-tid, '" + meetingName + "')]"));
                lessonCardBtn.Click();

                //Disable the stupid turn on notifications popup
                var notificationsDisable = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div/button[2]"));
                notificationsDisable.Click();

                ImplicitWait(TimeSpan.FromSeconds(0));
                //Wait 30 minutes until Join button appears
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(30));
                var joinBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(@data-tid, 'join-btn')]")));
                joinBtn.Click();

                ImplicitWait(TimeSpan.FromSeconds(WAIT_SECONDS));
                var noAudioMicBtn = driver.FindElement(By.XPath("//button[contains(@track-summary, 'Continue in call/meetup without device access')]"));
                noAudioMicBtn.Click();

                var preJoinCallBtn = driver.FindElement(By.XPath("//button[contains(@data-tid, 'prejoin-join-button')]"));
                preJoinCallBtn.Click();

                onMeeting = true;
                return true;
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
                driver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";

                // Turn everything to lower case because capital letters create problems
                string temp = registrationNum.ToLower();
                registrationNum = temp;

                //Filling email at Microsoft's login page
                var emailInputBox = driver.FindElement(By.Id("i0116"));
                emailInputBox.SendKeys(registrationNum + "@unipi.gr");

                //Click NextBtn at Microsoft's login page
                var nextEmailBtn = driver.FindElement(By.XPath("//input[@type='submit']"));
                nextEmailBtn.Click();

                //-----------------------------------Unipi page-----------------------------------

                //Filling UNIPI form
                var usernameUnipiInputBox = driver.FindElement(By.Id("username"));
                usernameUnipiInputBox.SendKeys(registrationNum);
                var passwordUnipiInputBox = driver.FindElement(By.Id("password"));
                passwordUnipiInputBox.SendKeys(password);

                //Login button
                var loginUnipiBtn = driver.FindElement(By.Id("submitForm"));
                loginUnipiBtn.Click();

                //Click no on Stay signed in
                var noStaySignIn = driver.FindElement(By.XPath("//*[@id='idBtn_Back']"));
                noStaySignIn.Click();

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
                if (driver == null)
                {
                    return default;
                }     
                
                List<string> meetingsList = new List<string>();
                //Find cards container
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));               
                var cardsContainer = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='favorite-teams-panel']")));

                //Get text from cards
                var cardsList = cardsContainer.FindElements(By.XPath("//h1[@data-tid='team-name-text']"));
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