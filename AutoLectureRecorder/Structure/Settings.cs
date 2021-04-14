using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoLectureRecorder.Structure
{
    public static class Settings
    {
        public static string VideoDirectory { get; set; } = VideoDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Auto Lecture Recorder");
        public static string AudioOutputDevice { get; set; }

        static Settings()
        {
            var serializedVideoDirectory = Serialize.DeserializeRecordingPath();
            if (serializedVideoDirectory != null)
            {
                VideoDirectory = serializedVideoDirectory;
            }

            AudioOutputDevice = Serialize.DeserializeAudioOutput();
        }
    }
}
