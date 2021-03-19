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
    public partial class WindowLogin : Window
    {
        public WindowLogin()
        {
            InitializeComponent();

            if (User.IsLoggedIn())
            {
                ShowMainWindow();
            }
            else
            {
                new Thread(() =>
                {
                    Chrome.Bot.HideBrowser = true;
                    Chrome.Bot.StartDriver();
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
                    if (Chrome.Bot.AuthenticateUser(registrationNum, password))
                    {
                        User.UpdateUserData(registrationNum, password);
                        Serialize.SerializeUserData(registrationNum, password);

                        Dispatcher.Invoke(() => ShowMainWindow());
                    }
                    else
                    {
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
            }
            catch { }
            
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
