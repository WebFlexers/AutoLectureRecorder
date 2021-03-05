using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AutoLectureRecorder.Structure
{
    public static class Schedule
    {
        private static Dictionary<string, List<Lecture>> lecturesByDay = new Dictionary<string, List<Lecture>>();
        // Property that returns a list of todays lectures
        public static List<Lecture> TodaysLectures
        {
            get
            {
                if (lecturesByDay.ContainsKey(Today))
                    return lecturesByDay[Today];
                else
                    return null;
            }
            private set { }
        }

        public static string Today
        {
            get
            {
                return DateTime.Now.ToString("dddd", CultureInfo.CreateSpecificCulture("en-US"));
            }
            private set { }
        }

        public static Dictionary<string, int> AllDaysIndexes { get; set; } = new Dictionary<string, int>();

        static Schedule()
        {
            AllDaysIndexes.Add("Monday", 0);
            AllDaysIndexes.Add("Tuesday", 1);
            AllDaysIndexes.Add("Wednesday", 2);
            AllDaysIndexes.Add("Thursday", 3);
            AllDaysIndexes.Add("Friday", 4);
            AllDaysIndexes.Add("Saturday", 5);
            AllDaysIndexes.Add("Sunday", 6);

            // Sample lectures
            string day = "Tuesday";
            string Name = "Papoutsi";
            TimeSpan startTime = TimeSpan.FromHours(8);
            TimeSpan endTime = TimeSpan.FromHours(12);
            string link = "https://teams.microsoft.com/l/team/19%3ae9a50d26030d499a91374868e9929d5a%40thread.tacv2/conversations?groupId=79b569eb-c74f-4cf9-ae09-620de80ee253&tenantId=d9c8dee3-558b-483d-b502-d31fa0cb24de";
            AddLecture(new Lecture(Name, startTime, endTime, link, true, day));
        }

        // Retrieve a list of the given day's lectures
        public static List<Lecture> GetLecturesByDay(string day)
        {
            if (lecturesByDay.ContainsKey(day))
                return lecturesByDay[day];
            else
                return null;
        }
        
        // Add the given lecture to the given day. 
        // If the day doesn't exist create it first and then add the lecture
        public static void AddLecture(Lecture lecture)
        {
            if (lecturesByDay.ContainsKey(lecture.Day))
                lecturesByDay[lecture.Day].Add(lecture);
            else
            {
                lecturesByDay.Add(lecture.Day, new List<Lecture>());
                lecturesByDay[lecture.Day].Add(lecture);
            }
        }

        public static void DeleteLecture(Lecture lecture)
        {
            lecturesByDay[lecture.Day].Remove(lecture);
        }
    }
}
