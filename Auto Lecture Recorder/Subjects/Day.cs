using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder.Subjects
{
    public class Day
    {
        string name;
        List<Lecture> lectures = new List<Lecture>();

        public Day(string name)
        {
            this.name = name;
        }

        public string Name { get => name;}
        public List<Lecture> Lectures { get => lectures; set => lectures = value; }
    }
}
