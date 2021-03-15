using AutoLectureRecorder.Structure;
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

namespace AutoLectureRecorder.Pages
{
    /// <summary>
    /// Interaction logic for Youtube.xaml
    /// </summary>
    public partial class Youtube : Page
    {
        public Youtube()
        {
            InitializeComponent();
        }

        List<VideoUploadStatus> _progressBarUserControls = new List<VideoUploadStatus>();
        public ProgressBar CreateProgressBar(Lecture lecture)
       {
            VideoUploadStatus videoUploadStatus = new VideoUploadStatus();
            videoUploadStatus.Margin = new Thickness(0, 10, 0, 0);
            videoUploadStatus.VerticalAlignment = VerticalAlignment.Top;
            videoUploadStatus.HorizontalAlignment = HorizontalAlignment.Center;
            videoUploadStatus.Init(lecture);

            _progressBarUserControls.Add(videoUploadStatus);
            StackPanelProgressbars.Children.Add(videoUploadStatus);
            return videoUploadStatus.Progressbar;
        }

        private void ButtonAddLecture_Click(object sender, RoutedEventArgs e)
        {
            for (int ix = StackPanelProgressbars.Children.Count - 1; ix >= 0; ix--)
            {
                if (StackPanelProgressbars.Children[ix] is VideoUploadStatus)
                {
                    var videoUploadStatus = (VideoUploadStatus)StackPanelProgressbars.Children[ix];
                    if (videoUploadStatus.Progressbar.Value != videoUploadStatus.Progressbar.Maximum)  
                        StackPanelProgressbars.Children.Remove(videoUploadStatus);
                }    
            }
        }
    }
}
