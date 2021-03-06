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

namespace AutoLectureRecorder.Structure
{
    /// <summary>
    /// Interaction logic for LectureModel.xaml
    /// </summary>
    public partial class LectureModel : UserControl
    {
        public Lecture Lecture { get; set; }
        public LectureModel()
        {
            InitializeComponent();
        }

        public void LoadLecture(Lecture lecture)
        {
            Lecture = lecture;

            TextBlockLink.Text = lecture.MeetingLink;
            TextBlockName.Text = lecture.Name;
            TextBlockStartTime.Text = lecture.StartTime.ToString("hh\\:mm\\:ss");
            TextBlockEndTime.Text = lecture.EndTime.ToString("hh\\:mm\\:ss");
            CheckboxYoutube.IsChecked = lecture.IsAutoUploadActive;
            CheckboxEnabled.IsChecked = lecture.IsLectureActive;
        }

        #region Checkboxes functionality
        private void CheckboxEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (Lecture != null)
                Lecture.IsLectureActive = true;
        }

        private void CheckboxEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Lecture != null)
                Lecture.IsLectureActive = false;
        }

        private void CheckboxYoutube_Checked(object sender, RoutedEventArgs e)
        {
            if (Lecture != null)
                Lecture.IsAutoUploadActive = true;
        }

        private void CheckboxYoutube_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Lecture != null)
                Lecture.IsAutoUploadActive = false;
        }
        #endregion

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Lecture != null)
            {
                MessageBoxResult userResponse = MessageBox.Show("Are you sure you want to delete the lecture?", "Confirmation",
                                                MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (userResponse == MessageBoxResult.Yes)
                {
                    Schedule.DeleteLecture(Lecture);
                    MainWindow window = (MainWindow)Application.Current.MainWindow;
                    window.RemoveLecture(Lecture);
                }
            }      
        }

    }
}
