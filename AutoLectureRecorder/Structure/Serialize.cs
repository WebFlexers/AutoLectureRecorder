using AutoLectureRecorder.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static void SerializeUserData(string registrationNum, string password, List<string> microsoftTeams)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "user.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            object[] userData = { registrationNum, password, microsoftTeams };

            formatter.Serialize(stream, userData);

            stream.Close();
        }

        public static object[] DeserializeUserData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "user.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            object[] userData;
            if (stream.Length != 0)
            {
                userData = (object[])formatter.Deserialize(stream);
            }
            else
            {
                userData = null;
            }

            stream.Close();
            return userData;

        }

        public static void SerializeRecordingPath(string recordingPath)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "rec_path.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            formatter.Serialize(stream, recordingPath);

            stream.Close();
        }

        public static string DeserializeRecordingPath()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "rec_path.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            string recPath;
            if (stream.Length != 0)
            {
                recPath = (string)formatter.Deserialize(stream);
            }
            else
            {
                recPath = null;
            }

            stream.Close();
            return recPath;

        }

        public static void SerializeAudioOutput(string outputDevice)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "rec_output_device.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            formatter.Serialize(stream, outputDevice);

            stream.Close();
        }

        public static string DeserializeAudioOutput()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(DataDirectory, "rec_output_device.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            string outputDevice;
            Trace.WriteLine(stream.Length);
            if (stream.Length != 0)
            {
                outputDevice = (string)formatter.Deserialize(stream);
            }
            else
            {
                outputDevice = null;
            }

            stream.Close();
            return outputDevice;
        }
    }
}
