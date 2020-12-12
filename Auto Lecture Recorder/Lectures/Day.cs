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
        public List<Lecture> Lectures { get; set; } = new List<Lecture>();

        public Day(string name)
        {
            this.Name = name;
        }

        
    }
}
