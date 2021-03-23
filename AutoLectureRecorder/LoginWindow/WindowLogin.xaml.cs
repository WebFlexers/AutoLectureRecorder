using AutoLectureRecorder.Selenium;
using AutoLectureRecorder.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoLectureRecorder.LoginWindow
{
    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin : Window, IChrome
    {
        #region IChrome Implementation
        public ChromeBot ChromeBot { get; set; }
        public void LoadBot()
        {
            ChromeBot = new ChromeBot();
            ChromeBot.HideBrowser = true;
        }
        public void TerminateBot()
        {
            ChromeBot.TerminateDriver();
        }
        #endregion

        public WindowLogin()
        {
            InitializeComponent();
            
            User.LoadUser();

            if (User.IsLoggedIn())
            {
                ShowMainWindow();
            }
            else
            {
                LoadBot();
                new Thread(() =>
                {
                    ChromeBot.StartDriver();
                }).Start();
            }
        }

        private void ButtonAddLecture_Click(object sender, RoutedEventArgs e)
        {
            string registrationNum = TextBoxRN.Text;
            string password = TextBoxPassword.Password;

            ButtonAddLecture.IsEnabled = false;
            LoadingIndicator.Visibility = Visibility.Visible;

            if (User.Password != password || User.RegistrationNumber != registrationNum)
            {
                Thread thread = new Thread(() =>
                {
                    if (ChromeBot.AuthenticateUser(registrationNum, password))
                    {
                        ChromeBot.TerminateDriver();
                        User.UpdateUserData(registrationNum, password);
                        Serialize.SerializeUserData(registrationNum, password, null);

                        Dispatcher.Invoke(() => ShowMainWindow());
                    }
                    else
                    {
                        ChromeBot.StartDriver();
                        Dispatcher.Invoke(() => LoadingIndicator.Visibility = Visibility.Hidden);
                        Dispatcher.Invoke(() => ButtonAddLecture.IsEnabled = true);
                        MessageBox.Show("The given credentials are not valid", "Unable to authenticate user", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else
            {
                ShowMainWindow();
            }
        }   

        private void ShowMainWindow()
        {
            LoadingIndicator.Visibility = Visibility.Hidden;
            MainWindow main = new MainWindow();
            this.Hide();

            main.ShowDialog();
            try
            {
                LoadingIndicator.Visibility = Visibility.Hidden;
                ButtonAddLecture.IsEnabled = true;
                this.Show();

                if (ChromeBot == null)
                    LoadBot();

                ChromeBot.StartDriver();
            }
            catch { }  
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (ChromeBot != null)
                new Thread(() => ChromeBot.TerminateDriver()).Start();
        }
    }
}
