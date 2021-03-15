using AutoLectureRecorder.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AutoLectureRecorder
{
    public static class Serialize
    {
        public static string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                    "AutoLectureRecorder");
        static Serialize()
        {
            Directory.CreateDirectory(DataDirectory);
        }

        public static void SerializeWeekLectures(Dictionary<string, List<Lecture>> lecturesByDay)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "lectures.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            formatter.Serialize(stream, lecturesByDay);

            stream.Close();
        }

        public static Dictionary<string, List<Lecture>> DeserializeWeekLectures()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "lectures.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            Dictionary<string, List<Lecture>> lecturesByDay;
            if (stream.Length != 0)
            {
                lecturesByDay = (Dictionary<string, List<Lecture>>)formatter.Deserialize(stream);
            }
            else
            {
                lecturesByDay = new Dictionary<string, List<Lecture>>();
            }

            stream.Close();
            return lecturesByDay;

        }

        public static void SerializeUserData(string registrationNum, string password)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "user.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            string[] userData = { registrationNum, password };

            formatter.Serialize(stream, userData);

            stream.Close();
        }

        public static string[] DeserializeUserData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "user.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            string[] userData;
            if (stream.Length != 0)
            {
                userData = (string[])formatter.Deserialize(stream);
            }
            else
            {
                userData = null;
            }

            stream.Close();
            return userData;

        }
    }
}
