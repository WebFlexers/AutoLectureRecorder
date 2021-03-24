using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLectureRecorder.Selenium
{
    public interface IChrome
    {
        public ChromeBot ChromeBot { get; set; }
        public void LoadBot(ChromeBot bot); 
        public void TerminateBot();
    }
}
