﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoLectureRecorder.Structure
{
    public static class Settings
    {
        public static string VideoDirectory { get; set; }

        static Settings()
        {
            if (VideoDirectory == null)
            {
                VideoDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Auto Lecture Recorder");
            }    
        }
    }
}
