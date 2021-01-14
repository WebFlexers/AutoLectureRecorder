using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using System.Windows.Forms;
using Auto_Lecture_Recorder;

using Auto_Lecture_Recorder.Lectures;


namespace Auto_Lecture_Recorder
{
    public static class Serializer
    {
        private static readonly string SAVE_LOCATION = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                                                    "AutoLectureRecorder");

        static Serializer()
        {
            Directory.CreateDirectory(SAVE_LOCATION);
        }

        public static void SerializeWeekLectures(Dictionary<string, Lectures.Day> week)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "lectures.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            formatter.Serialize(stream, week);

            stream.Close();
        }

        public static Dictionary<string, Lectures.Day> DeserializeWeekLectures()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "lectures.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            Dictionary<string, Lectures.Day> week;
            if (stream.Length != 0)
            {
                week = (Dictionary<string, Lectures.Day>)formatter.Deserialize(stream);
            }
            else
            {
                week = null;
            }

            stream.Close();
            return week;

        }

        public static void SerializeYoutubePlaylists(Dictionary<string, Playlist> playlists)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "youtube_playlists.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            List<string> playlistsNames = new List<string>();

            foreach (KeyValuePair<string, Playlist> keyValue in playlists)
            {
                playlistsNames.Add(keyValue.Key);
            }

            formatter.Serialize(stream, playlistsNames);

            stream.Close();
        }

        public static void DeserializeYoutubePlaylists(Youtube.YoutubeUploader youtubeUploader)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "youtube_playlists.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            List<string> playlistsNames;

            if (stream.Length != 0)
            {
                playlistsNames = (List<string>)formatter.Deserialize(stream);
                foreach (string name in playlistsNames)
                {
                    youtubeUploader.CreatePlaylist(name);
                }
            }

            stream.Close();
        }

        public static void SerializeRegistrationInfo(List<string> registrationInfo)
        {
            if (registrationInfo.Count == 2)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "registration_info.alr"), FileMode.OpenOrCreate, FileAccess.Write);

                formatter.Serialize(stream, registrationInfo);

                stream.Close();
            }
            else
            {
                throw new Exception("Wrong list length. Registration info list must contain " +
                                    "only registration number and password");
            }
        }

        public static List<string> DeserializeRegistrationInfo()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "registration_info.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            List<string> registrationInfo;
            if (stream.Length != 0)
            {
                registrationInfo = (List<string>)formatter.Deserialize(stream);
                if (registrationInfo.Count == 0)
                    registrationInfo = null;
            }
            else
            {
                registrationInfo = null;
            }

            stream.Close();
            return registrationInfo;
        }

        public enum Settings
        {
            MinimumParticipants = 0,
            InputDevice = 1,
            OutputDevice = 2,
            InputEnabled = 3,
            OutputEnabled = 4,
            Fps = 5,
            Quality = 6,
            YoutubeEnabled = 7
        }

        public static void SerializeSettings(int minParticipants, string inputDevice, string outputDevice,
                                             bool inputEnabled, bool outputEnabled, int fps, int quality,
                                             bool youtubeEnabled)
        {
            object[] settings = new object[8];
            settings[(int)Settings.MinimumParticipants] = minParticipants;
            settings[(int)Settings.InputDevice] = inputDevice;
            settings[(int)Settings.OutputDevice] = outputDevice;
            settings[(int)Settings.InputEnabled] = inputEnabled;
            settings[(int)Settings.OutputEnabled] = outputEnabled;
            settings[(int)Settings.Fps] = fps;
            settings[(int)Settings.Quality] = quality;
            settings[(int)Settings.YoutubeEnabled] = youtubeEnabled;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "settings.alr"), FileMode.OpenOrCreate, FileAccess.Write);

            formatter.Serialize(stream, settings);

            stream.Close();
        }

        public static object[] DeserializeSettings()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(SAVE_LOCATION, "settings.alr"), FileMode.OpenOrCreate, FileAccess.Read);

            object[] settings;
            if (stream.Length != 0)
            {
                settings = (object[])formatter.Deserialize(stream);
            }
            else
            {
                settings = null;
            }

            stream.Close();
            return settings;
        } 
    }
}
