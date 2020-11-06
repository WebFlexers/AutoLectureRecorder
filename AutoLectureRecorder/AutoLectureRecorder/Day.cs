using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLectureRecorder
{
    public class Day
    {
        string name;
        List<Subject> subjects = new List<Subject>();

        public Day(string name, List<Subject> subjects)
        {
            this.name = name;
            this.subjects = subjects;
        }

        public string Name { get => name; set => name = value; }
        public List<Subject> Subjects { get => subjects; set => subjects = value; }
    }
}
