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
        IWebDriver driver;

        int timeToConnect = 1; //in minutes
        string email, AM, password;               

        public void ConnectToTeamsChrome(string email, string AM, string password)
        {           
            this.email = email;
            this.AM = AM;
            this.password = password;

            //Opens a certain url to the chrome browser                      
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";                     

            Thread.Sleep(1000); //Wait 10 seconds

            IWebElement emailInputBox = driver.FindElement(By.Id("i0116"));
            emailInputBox.SendKeys(email);

            //Click Next Button
            IWebElement nextEmailBtn = driver.FindElement(By.Id("idSIButton9"));
            nextEmailBtn.Click();

            Thread.Sleep(5000);

            //UNIPI page
            IWebElement usernameUnipiInputBox = driver.FindElement(By.Id("username"));
            usernameUnipiInputBox.SendKeys(AM);
            IWebElement passwordUnipiInputBox = driver.FindElement(By.Id("password"));
            passwordUnipiInputBox.SendKeys(password);

            IWebElement loginUnipiBtn = driver.FindElement(By.Id("submitForm"));
            loginUnipiBtn.Click();

            Thread.Sleep(1000);

            //No stay sign page
            IWebElement NoStaySignInBtn = driver.FindElement(By.Id("idBtn_Back"));
            NoStaySignInBtn.Click();

            Thread.Sleep(3000);

            try
            {
                IWebElement UseWebAppBtn = driver.FindElement(By.ClassName("use-app-lnk"));
                UseWebAppBtn.Click();
            }
            catch (Exception e)
            {
                return;
            }                                                                                    
        }

        public void ConnectToMeeting(string name)
        {
            Thread.Sleep(2000);
            //team-XEIM20_21-[ΠΛΠΛΗ08]-
            IWebElement lessonCardBtn = driver.FindElement(By.XPath("//div[contains(@data-tid, 'ΜΕΤΑΓΛΩΤΤΙΣΤΕΣ')]"));
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
                    driver.Navigate().Refresh();
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
