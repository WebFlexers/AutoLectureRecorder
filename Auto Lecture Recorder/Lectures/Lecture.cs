using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder.Lectures
{
    public class Lecture
    {
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Platform { get; set; }

        public Lecture(string name, TimeSpan startTime, TimeSpan endTime, string platform)
        {
            // Set name
            Name = name;

            // Sets the lecture start time
            StartTime = startTime;
            // Sets the licture finish time
            EndTime = endTime;

            // Set platform
            Platform = platform;
        }

        // Checks if the end time is bigger than the start time
        public static bool timeIsValid(TimeSpan startTime, TimeSpan endTime)
        {
            return endTime > startTime;
        }

        
    }
}
