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
    /// Interaction logic for VideoUploadStatus.xaml
    /// </summary>
    public partial class VideoUploadStatus : UserControl
    {
        public Lecture Lecture { get; set; }

        public VideoUploadStatus()
        {
            InitializeComponent();
        }

        public ProgressBar Progressbar { get => ProgressBarLecture; private set { } }

        public void Init(Lecture lecture)
        {
            Lecture = lecture;
            TextBlockLectureName.Text = Lecture.Name;
        }
    }
}
