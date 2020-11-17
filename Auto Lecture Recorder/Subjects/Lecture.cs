using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder.Subjects
{
    public class Lecture
    {
        private string name;
        private string platform; // e.g. MicrosoftTeams
        // Instantiate at current time (Constructor will change the time)
        private DateTime lectureStart = DateTime.Now;
        private DateTime lectureFinish = DateTime.Now;

        // Indicates if this subject will be recorded
        bool selected;

        public Lecture(string name, int startHour, int startMinute, int finishHour, int finishMinute, string platform)
        {
            // Set name
            this.name = name;

            // Sets the lecture start time
            TimeSpan tsStart = new TimeSpan(startHour, startMinute, 0);
            lectureStart = lectureStart.Date + tsStart;
            // Sets the licture finish time
            TimeSpan tsFinish = new TimeSpan(finishHour, finishMinute, 0);
            lectureFinish = lectureFinish.Date + tsFinish;

            // Set platform
            if (platform.Equals("Webex") || platform.Equals("MicrosoftTeams"))
            {
                this.platform = platform;
            }
            else
            {
                throw new System.InvalidOperationException("Auto Lecture Recorder only supports Webex and MicrosoftTeams");
            }
        }

        public string Name { get => name; set => name = value; }
        public DateTime LectureStart { get => lectureStart; set => lectureStart = value; }
        public DateTime LectureFinish { get => lectureFinish; set => lectureFinish = value; }
        public string Platform { get => platform; set => platform = value; }
        public bool Selected { get => selected; set => selected = value; }
    }
}
