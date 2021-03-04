using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLectureRecorder.Structure
{
    public class Lecture
    {
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string MeetingLink { get; set; }
        public bool IsLectureActive { get; set; }
        public bool IsAutoUploadActive { get; set; }
        public string Day { get; set; }

        public Lecture(string name, TimeSpan startTime, TimeSpan endTime, string link, bool isAutoUploadActive, string day)
        {
            Name = name;
            StartTime = startTime;
            EndTime = endTime;

            if (StartTime >= EndTime)
                throw new Exception("The end time must be greater than the start time");

            MeetingLink = link;
            IsAutoUploadActive = isAutoUploadActive;
            IsLectureActive = true;
            Day = day;
        }

    }
}
