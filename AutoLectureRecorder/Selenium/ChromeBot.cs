using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AutoLectureRecorder.Selenium
{
    public partial class ChromeBot
    {
        const int waitTime = 60; //seconds    

        IWebDriver driver;

        List<string[]> cookiesList = new List<string[]>();

        //Settings
        public bool HideBrowser = false;
        public bool MaximizeBrowser = true;
        public bool HideCommandLine = true;

        private bool isDriverRunning = false;

        public void CloseFocusedBrowser() => driver.Close();       
        public void RefreshCurrentPage() => driver.Navigate().Refresh();        
        public void WaitForSeconds(int sec) => driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(sec);

        public void TerminateDriver()
        {
            if (driver != null)
            {
                driver.Quit();
                isDriverRunning = false;
            }
        }

        public void StartDriver()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            var driverService = ChromeDriverService.CreateDefaultService();

            //Disable mic
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.media_stream_mic", 2);
            //Disable camera
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.media_stream_camera", 2);
            //General Options
            chromeOptions.AddAdditionalCapability("browser", "Chrome", true);
            chromeOptions.AddAdditionalCapability("browser_version", "70.0", true);
            chromeOptions.AddAdditionalCapability("os", "Windows", true);
            chromeOptions.AddAdditionalCapability("os_version", "10", true);
            chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
            //Arguments
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--disable-default-apps");
            chromeOptions.AddArguments("--window-size=1920,1080");
            chromeOptions.AddArguments("--disable-gpu");
            chromeOptions.AddArguments("--disable-extensions");
            chromeOptions.AddArguments("--proxy-server='direct://'");
            chromeOptions.AddArguments("--proxy-bypass-list=*");
            if (HideBrowser == true) chromeOptions.AddArgument("--headless");
            //Services
            if (HideCommandLine == true) driverService.HideCommandPromptWindow = true;   
            
            driver = new ChromeDriver(driverService, chromeOptions);
            isDriverRunning = true;

            WaitForSeconds(waitTime);
            if(MaximizeBrowser == true)
                driver.Manage().Window.Maximize();
        }        
        
        private void SaveCookiesToList()
        {
            //Adding cookies to cookiesList   
            foreach (Cookie tempCookie in driver.Manage().Cookies.AllCookies)
            {
                string[] tempList = { tempCookie.Name, tempCookie.Value, tempCookie.Domain, tempCookie.Expiry.ToString() };
                cookiesList.Add(tempList);
                Trace.WriteLine(tempCookie.Name + ", " + tempCookie.Expiry);
            }
        }      

        private void SaveCookiesToFile(List<string[]> cookiesList)      
        {           
            try
            {
                if (File.Exists(cookieFileName))
                    File.Delete(cookieFileName);
                //Save cookie by serializing it
                FileStream stream = File.Create(cookieFileName);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, cookiesList);
                stream.Close();
                Trace.WriteLine("Cookies saved successfully!");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("An error occured while saving cookie: " + ex.Message);
            }
        }

        private void LoadCookies(string url, string filename)
        {
            if (driver == null || !isDriverRunning) return;
            if (!File.Exists(filename)) return;

            try
            {
                //Deserialize cookies file
                Stream stream = new FileStream(filename, FileMode.Open);
                IFormatter formatter = new BinaryFormatter();
                List<string[]> new_cookiesList = (List<string[]>)formatter.Deserialize(stream);
                stream.Close();

                ClearBrowserCache();
                // Thread.Sleep(2000);

                if (string.IsNullOrEmpty(url)) driver.Url = teamsHomePagetUrl;
                else driver.Url = url;

                //Load cookies
                foreach (string[] tmpCkInfo in new_cookiesList)
                {
                    driver.Manage().Cookies.AddCookie(new Cookie(tmpCkInfo[0], tmpCkInfo[1]));
                }

                RefreshCurrentPage();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("An error occured while loading cookie: " + ex.Message);
            }
        }

        private void ClearBrowserCache()
        {
            driver.Manage().Cookies.DeleteAllCookies(); //delete all cookies
            // Thread.Sleep(7000); //wait 7 seconds to clear cookies
        }
    }
}
