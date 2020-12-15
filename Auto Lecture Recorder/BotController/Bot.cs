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
using System.Threading;
using System.Windows.Forms;

namespace Auto_Lecture_Recorder.BotController
{
    class Bot
    {
        //IWebDriver driver;
        ChromeDriver chromeDriver;
        int timeToConnect = 1; //in minutes
        string email, AM, password;               

        public Bot()
        {
            ChromeDriverService driver = ChromeDriverService.CreateDefaultService();
            driver.HideCommandPromptWindow = true;
            chromeDriver = new ChromeDriver(driver, new ChromeOptions());
        }

        public void ConnectToTeamsChrome(string email, string AM, string password)
        {

            

            this.email = email;
            this.AM = AM;
            this.password = password;

            // Opens a certain url to the chrome browser
            while (true)
            {
                try
                {
                    chromeDriver.Manage().Window.Maximize();
                    chromeDriver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";
                    break;
                } 
                catch
                {
                    Thread.Sleep(500);
                }
            }
                                 
            while (true)
            {
                try
                {
                    IWebElement emailInputBox = chromeDriver.FindElement(By.Id("i0116"));
                    emailInputBox.SendKeys(email);
                    break;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
            

            //Click Next Button
            while (true)
            {
                try
                {
                    IWebElement nextEmailBtn = chromeDriver.FindElement(By.Id("idSIButton9"));
                    nextEmailBtn.Click();
                    break;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
            

            

            //UNIPI page
            while (true)
            {
                try
                {
                    IWebElement usernameUnipiInputBox = chromeDriver.FindElement(By.Id("username"));
                    usernameUnipiInputBox.SendKeys(AM);
                    IWebElement passwordUnipiInputBox = chromeDriver.FindElement(By.Id("password"));
                    passwordUnipiInputBox.SendKeys(password);

                    IWebElement loginUnipiBtn = chromeDriver.FindElement(By.Id("submitForm"));
                    loginUnipiBtn.Click();

                    break;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
            

            

            //No stay sign page
            while (true)
            {
                try
                {
                    IWebElement NoStaySignInBtn = chromeDriver.FindElement(By.Id("idBtn_Back"));
                    NoStaySignInBtn.Click();
                    break;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
            

            
            /*
            try
            {
                IWebElement UseWebAppBtn = chromeDriver.FindElement(By.ClassName("use-app-lnk"));
                UseWebAppBtn.Click();
            }
            catch (Exception e)
            {
                throw e;
            }   
            */
        }

        public void ConnectToMeeting(string name)
        {
            Thread.Sleep(2000);
            //team-XEIM20_21-[ΠΛΠΛΗ08]-
            IWebElement lessonCardBtn = chromeDriver.FindElement(By.XPath("//div[contains(@data-tid, 'ΜΕΤΑΓΛΩΤΤΙΣΤΕΣ')]"));
            lessonCardBtn.Click();

            bool isJoined = false;
            do
            {
                try
                {                  
                    Thread.Sleep(5000);                                                         
                                      
                    //Handle Allow-Block

                    //data-tid = prejoin-join-button

                    isJoined = true;
                }
                catch (Exception e)
                {
                    Thread.Sleep(36000 * timeToConnect);
                    chromeDriver.Navigate().Refresh();
                }

                if (!isJoined)
                    MessageBox.Show("Join btn not found");
            }
            while (!isJoined);
        }

        public bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class BrowserNotFoundException : Exception
    {
        public BrowserNotFoundException(string message)
            :base(message)
        {
            MessageBox.Show(message);
        }
                  
    }
}
