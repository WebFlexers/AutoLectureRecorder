using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLectureRecorder.Structure
{
    public static class User
    {
        public static string RegistrationNumber { get; set; }
        public static string Password { get; set; }

        public static void AddUser(string registrationNumber, string password)
        {
            RegistrationNumber = registrationNumber;
            Password = password;
        }
    }
}
