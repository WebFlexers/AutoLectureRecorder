using AutoLectureRecorder.Selenium;
using AutoLectureRecorder.Structure;
using System;
using System.Collections.Generic;
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
        ChromeBot chromeBot;

        public WindowLogin()
        {
            InitializeComponent();
            new Thread(() =>
            {
                chromeBot = new ChromeBot();
                chromeBot.HideBrowser = true;
                chromeBot.StartDriver();
            }).Start();   
        }

        private void ButtonAddLecture_Click(object sender, RoutedEventArgs e)
        {
            string registrationNum = TextBoxRN.Text;
            string password = TextBoxPassword.Password;

            ButtonAddLecture.IsEnabled = false;
            LoadingIndicator.Visibility = Visibility.Visible;

            Thread thread = new Thread(() =>
            {
                if (chromeBot.AuthenticateUser(registrationNum, password))
                {
                    User.AddUser(registrationNum, password);

                    Dispatcher.Invoke(() =>
                    {
                        LoadingIndicator.Visibility = Visibility.Hidden;
                        MainWindow main = new MainWindow();
                        main.Owner = this;
                        this.Hide();
                        main.ShowDialog();
                    });
                    
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

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
