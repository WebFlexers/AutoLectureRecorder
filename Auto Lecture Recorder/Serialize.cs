using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;

using Auto_Lecture_Recorder.Lectures;

namespace Auto_Lecture_Recorder
{
    public static class Serializer
    {
        public static void SerializeWeekLectures(Dictionary<string, Day> week)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("lectures.alr", FileMode.OpenOrCreate, FileAccess.Write);

            formatter.Serialize(stream, week);

            stream.Close();
        }

        public static void SerializeYoutubePlaylists(Dictionary<string, Playlist> playlists)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("youtube_playlists.alr", FileMode.OpenOrCreate, FileAccess.Write);

            List<string> playlistsNames = new List<string>();

            foreach (KeyValuePair<string, Playlist> keyValue in playlists)
            {
                playlistsNames.Add(keyValue.Key);
            }

            formatter.Serialize(stream, playlistsNames);

            stream.Close();
        }

        public static void SerializeRegistrationInfo(List<string> registrationInfo)
        {
            if (registrationInfo.Count == 2)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("registration_info.alr", FileMode.OpenOrCreate, FileAccess.Write);

                formatter.Serialize(stream, registrationInfo);

                stream.Close();
            }
            else
            {
                throw new Exception("Wrong list length. Registration info list must contain " +
                                    "only registration number and password");
            }
        }

        public static Dictionary<string, Day> DeserializeWeekLectures()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("lectures.alr", FileMode.OpenOrCreate, FileAccess.Read);

            Dictionary<string, Day> week;
            if (stream.Length != 0)
            {
                week = (Dictionary<string, Day>)formatter.Deserialize(stream);
            }
            else
            {
                week = null;
            }

            stream.Close();
            return week;
                
        }

        public static void DeserializeYoutubePlaylists(Youtube.YoutubeUploader youtubeUploader)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("youtube_playlists.alr", FileMode.OpenOrCreate, FileAccess.Read);

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

        public static List<string> DeserializeRegistrationInfo()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("registration_info.alr", FileMode.OpenOrCreate, FileAccess.Read);

            List<string> registrationInfo;
            if (stream.Length != 0)
            {
                registrationInfo = (List<string>)formatter.Deserialize(stream);
            }
            else
            {
                registrationInfo = null;
            }

            stream.Close();
            return registrationInfo;
        }
    }
}
