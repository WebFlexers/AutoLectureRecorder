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

namespace AutoLectureRecorder.Selenium
{
    public partial class ChromeBot
    {
        const int waitTime = 30; //seconds    

        IWebDriver driver;

        List<string[]> cookiesList = new List<string[]>();
        public string WindowHandle { get => driver.CurrentWindowHandle; }

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
            try
            {
                if (!isDriverRunning)
                {
                    isDriverRunning = true;

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
                    //Arguments
                    chromeOptions.AddArgument("--disable-extensions");
                    chromeOptions.AddArgument("--disable-default-apps");
                    if (HideBrowser == true) chromeOptions.AddArgument("--headless");
                    //Services
                    if (HideCommandLine == true) driverService.HideCommandPromptWindow = true;

                    driver = new ChromeDriver(driverService, chromeOptions);

                    WaitForSeconds(waitTime);
                    if (MaximizeBrowser == true)
                        driver.Manage().Window.Maximize();
                }
            } 
            catch 
            {
                isDriverRunning = false;
                if (driver != null)
                    CloseFocusedBrowser();
                TerminateDriver();
            }
            
        }        
        
        private void SaveCookiesToList()
        {
            //Adding cookies to cookiesList   
            foreach (Cookie tempCookie in driver.Manage().Cookies.AllCookies)
            {
                string[] tempList = { tempCookie.Name, tempCookie.Value, tempCookie.Domain, tempCookie.Expiry.ToString() };
                cookiesList.Add(tempList);
                Console.WriteLine(tempCookie.Name + ", " + tempCookie.Expiry);
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
                Console.WriteLine("Cookies saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while saving cookie: " + ex.Message);
            }
        }

        private void LoadCookies(string url, string filename)
        {
            if (driver == null || !isDriverRunning) return;
            if (!File.Exists(cookieFileName)) return;

            try
            {
                //Deserialize cookies file
                Stream stream = new FileStream(filename, FileMode.Open);
                IFormatter formatter = new BinaryFormatter();
                List<string[]> new_cookiesList = (List<string[]>)formatter.Deserialize(stream);
                stream.Close();

                ClearBrowserCache();
                Thread.Sleep(2000);

                if (string.IsNullOrEmpty(url)) driver.Url = teamsHomePagetUrl;
                else driver.Url = url;

                //Load cookies
                Cookie cookie;
                foreach (string[] tmpCkInfo in new_cookiesList)
                {
                    cookie = new Cookie(tmpCkInfo[0], tmpCkInfo[1]);
                    driver.Manage().Cookies.AddCookie(cookie);
                }

                RefreshCurrentPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while loading cookie: " + ex.Message);
            }
        }

        private void ClearBrowserCache()
        {
            driver.Manage().Cookies.DeleteAllCookies(); //delete all cookies
            Thread.Sleep(7000); //wait 7 seconds to clear cookies
        }

    }
}
