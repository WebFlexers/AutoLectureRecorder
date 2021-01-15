using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auto_Lecture_Recorder
{
    partial class MainForm
    {
        bool loggingIn = false;
        int dotsNum = 0;

        private void textboxPassword_Load(object sender, EventArgs e)
        {
            textboxPassword.MakePasswordField();
        }

        private void timerLoginProgress_Tick(object sender, EventArgs e)
        {
            // ... wait effect
            if (loggingIn)
            {
                dotsNum++;
                StringBuilder dots = new StringBuilder();
                dots.Clear();
                for (int i = 0; i < dotsNum; i++)
                {
                    dots.Append(".");
                }
                
                labelLoginStatus.Text = "Logging in" + Environment.NewLine + "Please wait" + Environment.NewLine + dots.ToString();

                if (dotsNum == 3)
                {
                    dotsNum = 0;
                }
            }
            else
            {
                timerLoginProgress.Stop();
            }
            
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Disable button until authentication is done
            buttonLogin.Enabled = false;
            // Reset color
            labelLoginStatus.ForeColor = Color.FromArgb(42, 123, 245);
            // Enable label
            labelLoginStatus.Visible = true;
            // Progress report
            loggingIn = true;
            timerLoginProgress_Tick(sender, e);
            timerLoginProgress.Start();

            // Login
            RN = textboxRN.GetText();
            password = textboxPassword.GetText();
            // Execute login on different thread
            Thread loginThread = new Thread(() => AuthenticateUser(RN, password));
            loginThread.Start();
        }

        delegate void EnableButton();
        delegate void LoginResult();
        private void AuthenticateUser(string RN, string password)
        {
            
            teamsBot.HideBrowser = true;
            teamsBot.StartDriver();

            if (teamsBot.AuthenticateUser(RN, password))
            {
                LoginResult result = OnLoginSuccess;
                panelAuthentication.Invoke(result);
            }
            else
            {
                LoginResult result = OnLoginFailure;
                panelAuthentication.Invoke(result);
            }

            teamsBot.HideBrowser = false;
            teamsBot.TerminateDriver();

            Invoke((Action)(() => buttonLogin.Enabled = true));
        }

        private void OnLoginSuccess()
        {
            loggingIn = false;
            teamsAuthenticated = true;
            labelLoginStatus.ForeColor = Color.FromArgb(151, 253, 30);
            labelLoginStatus.Text = "Logged in successfully!" + Environment.NewLine + "You can schedule lectures now";
            labelLoginStatus.Visible = true;
            // Serialize the result
            List<string> registrationInfo = new List<string>();
            registrationInfo.Add(RN);
            registrationInfo.Add(password);

            Serializer.SerializeRegistrationInfo(registrationInfo);
        }

        private void OnLoginFailure()
        {
            loggingIn = false;
            teamsAuthenticated = false;
            labelLoginStatus.ForeColor = Color.FromArgb(225, 7, 6);
            labelLoginStatus.Text = "Login failed. Please make sure you entered" + Environment.NewLine +
                                    "the correct credentials and try again";
        }
    }
}
