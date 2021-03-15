using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLectureRecorder.Structure
{
    public static class User
    {
        public static string RegistrationNumber { get; set; }
        public static string Password { get; set; }

        static User()
        {
            string[] userData = Serialize.DeserializeUserData();
            if (userData != null)
            {
                RegistrationNumber = userData[0];
                Password = userData[1];
            }
            
        }

        public static bool IsLoggedIn()
        {
            if (RegistrationNumber != null && Password != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void UpdateUserData(string registrationNumber, string password)
        {
            RegistrationNumber = registrationNumber;
            Password = password;
        }
    }
}
