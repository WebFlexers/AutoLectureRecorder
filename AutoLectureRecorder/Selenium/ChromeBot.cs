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
        const int WAIT_SECONDS = 60; //seconds    

        IWebDriver driver;

        List<string[]> cookiesList = new List<string[]>();

        //Settings
        public bool IsBrowserHidden { get; set; } = false;
        public bool IsBrowserMaximized { get; set; } = true;
        public bool IsCommandlineHidden { get; set; } = true;

        private bool isDriverRunning { get => driver != null; }

        public void CloseFocusedBrowser() => driver.Close();       
        public void RefreshCurrentPage() => driver.Navigate().Refresh();        
        public void ImplicitWait(int seconds) => driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);

        public void TerminateDriver()
        {
            if (isDriverRunning)
            {
                driver.Quit();
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
            chromeOptions.AddArgument("no-sandbox");

            if (IsBrowserHidden) chromeOptions.AddArgument("--headless");

            //Services
            if (IsCommandlineHidden) driverService.HideCommandPromptWindow = true;   
            
            driver = new ChromeDriver(driverService, chromeOptions);

            ImplicitWait(WAIT_SECONDS);

            if (IsBrowserMaximized) driver.Manage().Window.Maximize();
        }        
       
    }
}
