using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder.BotController.Unipi
{
    partial class ChromeBot
    {
        IWebDriver driver;

        public bool HideBrowser = false;
        public void CloseFocusedBrowser() => driver.Close();
        public void TerminateDriver()
        {
            if (driver != null) 
                driver.Quit();
        }
        public void RefreshCurrentPage() => driver.Navigate().Refresh();        
        public void WaitForSeconds(int sec) => driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(sec);

        public void StartDriver()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            //Disable mic
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.media_stream_mic", 2);
            //Disable camera
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.media_stream_camera", 2);
            //Other Options
            chromeOptions.AddAdditionalCapability("browser", "Chrome", true);
            chromeOptions.AddAdditionalCapability("browser_version", "70.0", true);
            chromeOptions.AddAdditionalCapability("os", "Windows", true);
            chromeOptions.AddAdditionalCapability("os_version", "10", true);
            if(HideBrowser)
                chromeOptions.AddArgument("--headless");
            //Hide cmd
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            //Start driver
            driver = new ChromeDriver(driverService, chromeOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(waitTime);
            driver.Manage().Window.Maximize();
        }

        public bool AuthenticateUser(string AM, string password)
        {
            bool isEmailValid = false;
            bool areUnipiCredsValid = false;

            try
            {
                //Microsoft's Login Page
                driver.Url = "https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=5e3ce6c0-2b1f-4285-8d4b-75ee78787346&redirect_uri=https%3A%2F%2Fteams.microsoft.com%2Fgo&state=19b0dc60-3d5f-467f-9ee1-3849f5ae7e58&&client-request-id=75367383-e3e7-480f-a14f-faf664ccea61&x-client-SKU=Js&x-client-Ver=1.0.9&nonce=29792698-73cd-457e-977e-e23d8843a8f0&domain_hint=";
                

                //Filling email at Microsoft's login page
                IWebElement emailInputBox = driver.FindElement(By.Id("i0116"));
                emailInputBox.SendKeys(AM + "@unipi.gr");
                

                //Click NextBtn at Microsoft's login page
                IWebElement nextEmailBtn = driver.FindElement(By.Id("idSIButton9"));
                nextEmailBtn.Click();
                //Check if email error showed at Microsoft's login page
                try
                {
                    IWebElement usernameError = driver.FindElement(By.Id("usernameError"));                    
                }
                catch
                {
                    isEmailValid = true;
                }
                

                //Unipi page
                //Filling UNIPI form
                IWebElement usernameUnipiInputBox = driver.FindElement(By.Id("username"));
                usernameUnipiInputBox.SendKeys(AM);
                IWebElement passwordUnipiInputBox = driver.FindElement(By.Id("password"));
                passwordUnipiInputBox.SendKeys(password);
                

                //Login button
                IWebElement loginUnipiBtn = driver.FindElement(By.Id("submitForm"));
                loginUnipiBtn.Click();               

                //Check if error showed at Unipi's login page
                try
                {
                    IWebElement unipiCredsError = driver.FindElement(By.XPath("//div[contains(@class, 'alert alert-danger')]"));
                }
                catch
                {
                    areUnipiCredsValid = true;
                }                

                if (isEmailValid && areUnipiCredsValid)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
