using AutoLectureRecorder.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLectureRecorder.Structure
{
    [Serializable]
    public static class User
    {
        public static string RegistrationNumber { get; set; }
        public static string Password { get; set; }
        public static List<string> MicrosoftTeams { get; set; }

        public static void LoadUser()
        {
            object[] userData = Serialize.DeserializeUserData();
            if (userData != null)
            {
                RegistrationNumber = (string)userData[0];
                Password = (string)userData[1];
                MicrosoftTeams = (List<string>)userData[2];
            }
        }

        public static bool IsLoggedIn()
        {
            if ((RegistrationNumber == null && Password == null) || Chrome.Bot.IsCookieExpired("TSPREAUTHCOOKIE"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void UpdateUserData(string registrationNumber, string password)
        {
            RegistrationNumber = registrationNumber;
            Password = password;
        }

        public static void UpdateUserData(string registrationNumber, string password, List<string> microsoftTeams)
        {
            RegistrationNumber = registrationNumber;
            Password = password;
            MicrosoftTeams = microsoftTeams;
        }
    }
}
