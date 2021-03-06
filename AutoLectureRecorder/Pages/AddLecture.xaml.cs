using AutoLectureRecorder.Structure;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoLectureRecorder.Pages
{
    /// <summary>
    /// Interaction logic for AddLecture.xaml
    /// </summary>
    public partial class AddLecture : Page
    {
        public AddLecture()
        {
            InitializeComponent();
        }

        #region Add Lecture
        private void ButtonAddLecture_Click(object sender, RoutedEventArgs e)
        {
            if (IsUserInputNotEmpty())
            {
                TimeSpan startTime = TimePickerStartTime.SelectedTime.Value.TimeOfDay;
                TimeSpan endTime = TimePickerEndTime.SelectedTime.Value.TimeOfDay;

                if (endTime <= startTime)
                {
                    MessageBox.Show("The end time must be greater than the start time", "Wrong time input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string name = TextboxLectureName.Text;
                string meetingLink = TextboxLink.Text;
                string day = ComboboxDay.Text;
                bool isYoutubeActive = CheckboxYoutube.IsChecked.Value;

                Lecture lecture = new Lecture(name, startTime, endTime, meetingLink, isYoutubeActive, day);
                Schedule.AddLecture(lecture);
                ((MainWindow)Application.Current.MainWindow).AddNewLectureModels();

                MessageBox.Show("Lecture added successfully!");
            }
            else
                MessageBox.Show("Make sure that all fields are filled", "Empty fields detected", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool IsUserInputNotEmpty()
        {
            bool IsUnputCorrect = true;

            if (string.IsNullOrWhiteSpace(TextboxLectureName.Text))
            {
                TextboxLectureName.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AD2817"));
                IsUnputCorrect = false;
            }
                
            if (string.IsNullOrWhiteSpace(TextboxLink.Text))
            {
                TextboxLink.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AD2817"));
                IsUnputCorrect = false;
            }

            if (ComboboxDay.SelectedIndex == -1)
            {
                ComboboxDay.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AD2817"));
                IsUnputCorrect = false;
            }

            if (TimePickerStartTime.SelectedTime == null)
            {
                TimePickerStartTime.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AD2817"));
                IsUnputCorrect = false;
            }

            if (TimePickerEndTime.SelectedTime == null)
            {
                TimePickerEndTime.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AD2817"));
                IsUnputCorrect = false;
            }

            return IsUnputCorrect;
        }
        #endregion
    }
}
