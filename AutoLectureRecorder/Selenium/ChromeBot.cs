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
using OpenQA.Selenium.Support.UI;
using System.Management;

namespace AutoLectureRecorder.Selenium
{
    public partial class ChromeBot
    {
        const int WAIT_SECONDS = 60; //seconds    

        IWebDriver _driver;

        List<string[]> cookiesList = new List<string[]>();

        //Settings
        public bool IsBrowserHidden { get; set; } = false;
        public bool IsBrowserMaximized { get; set; } = true;
        public bool IsCommandlineHidden { get; set; } = true;
        public IntPtr ChromeWindowHandle
        {
            get
            {
                Process[] chromeProcesses = GetChildProcesses(ProccessId);
                return chromeProcesses[1].MainWindowHandle;
            }
        }
        private static Process[] GetChildProcesses(int parentId)
        {
            var query = "Select * From Win32_Process Where ParentProcessId = "
                    + parentId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            var result = processList.Cast<ManagementObject>().Select(p =>
            Process.GetProcessById(Convert.ToInt32(p.GetPropertyValue("ProcessId")))).ToArray();

            return result;
        }

        public int ProccessId;

        private bool isDriverRunning { get => _driver != null; } 

        public void CloseFocusedBrowser() => _driver.Close();       
        public void RefreshCurrentPage() => _driver.Navigate().Refresh();

        public void TerminateDriver()
        {
            if (isDriverRunning)
            {
                _driver.Quit();
            }
        }

        public void StartDriver()
        {
            TerminateDriver();

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
            Console.WriteLine("Broswer versionQ: " +chromeOptions.BrowserVersion);
           if (IsBrowserHidden) chromeOptions.AddArgument("--headless");

            //Services
            if (IsCommandlineHidden) driverService.HideCommandPromptWindow = true;   
            
            _driver = new ChromeDriver(driverService, chromeOptions);

            ProccessId = driverService.ProcessId;

            if (IsBrowserMaximized) _driver.Manage().Window.Maximize();
        }        
       
    }
}
