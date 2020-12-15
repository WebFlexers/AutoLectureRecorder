using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder.Lectures
{
    public class Day
    {
        public string Name { get; }
        public List<Lecture> Lectures { get; set; }

        public Day(string name)
        {
            this.Name = name;
            Lectures = new List<Lecture>();
        }

        /* Gets the lecture that will be kept and disables all the conflicting ones */
        public void DisableConflictingLectures(Lecture keptLecture)
        {
            // Find the conflicting lectures and disable them
            if (keptLecture.Active)
            {
                foreach (Lecture otherLecture in Lectures)
                {
                    if (keptLecture != otherLecture && otherLecture.Active)
                    {
                        if ((otherLecture.StartTime >= keptLecture.StartTime && otherLecture.StartTime <= keptLecture.EndTime) ||
                            (otherLecture.StartTime <= keptLecture.StartTime && otherLecture.EndTime >= keptLecture.StartTime))
                        {
                            otherLecture.Active = false;
                            otherLecture.CheckBox.Unckeck();
                        }
                    }
                }
            }
        }
    }
}
