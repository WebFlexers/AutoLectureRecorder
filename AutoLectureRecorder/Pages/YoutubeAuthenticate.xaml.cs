using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeAPI;

namespace AutoLectureRecorder.Pages
{
    /// <summary>
    /// Interaction logic for Upload.xaml
    /// </summary>
    public partial class YoutubeAuthenticate : Page
    {
        public YoutubeAuthenticate()
        {
            InitializeComponent();
        }

        private async void ButtonAddLecture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                YoutubeUploader youtube = new YoutubeUploader();
                if (await youtube.Authenticate())
                {

                }
                else
                    MessageBox.Show("Unable to connect to your youtube account", "Failed to authenticate", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch
            {
                MessageBox.Show("A vital error has occured", "Authentication error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
                
        }
    }
}
