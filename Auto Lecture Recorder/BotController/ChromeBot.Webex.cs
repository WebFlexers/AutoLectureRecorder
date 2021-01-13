using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder.BotController.Unipi
{
    partial class ChromeBot
    {
        public bool ConnectToWebex(string name, string email, string meetingUrl)
        {
            try
            {
                driver.Url = "https://unipi-gr.webex.com/unipi-gr/j.php?MTID=m48a783480ce5fd6fcdc549ceb681168b";

                var openAppAlert = driver.SwitchTo().Alert();
                openAppAlert.Dismiss();

                IWebElement joinFromBrowserBtn = driver.FindElement(By.Id("push_download_join_by_browser"));
                joinFromBrowserBtn.Click();

                IWebElement nameInputField = driver.FindElement(By.XPath("//input[contains(@placeholder, 'Your full name')]"));
                joinFromBrowserBtn.SendKeys(name);
                IWebElement emailInputField = driver.FindElement(By.XPath("//input[contains(@placeholder, 'Email address')]"));
                joinFromBrowserBtn.SendKeys(email);

                return true;
            }
            catch
            {
                return false;
            }           
        }
    }
}
