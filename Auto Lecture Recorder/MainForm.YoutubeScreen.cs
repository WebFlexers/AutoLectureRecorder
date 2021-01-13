using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder
{
    partial class MainForm
    {
        private void timerYoutubeProgress_Tick(object sender, EventArgs e)
        {
            progressBarYoutube.Value =  (int)youtubeUploader.CurrentProgress;
        }
    }
}
