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
using AutoLectureRecorder.Structure;

namespace AutoLectureRecorder.Pages
{
    /// <summary>
    /// Interaction logic for Lectures.xaml
    /// </summary>
    public partial class Lectures : Page
    {
        public Lectures()
        {
            InitializeComponent();
            LoadDaysLectures();
        }

        /* One page for each model */
        LectureModelsPage[] lectureModelPages = new LectureModelsPage[7];

        /* Create the pages for each day and select Monday */
        public void LoadDaysLectures()
        {
            int index = 0;
            foreach (string day in Schedule.AllDays)
            {
                lectureModelPages[index] = new LectureModelsPage(day);
                index++;
            }
            ListViewItemMonday.IsSelected = true;
        }

        /* Add to each page every lecture that doesn't already exist in them */
        public void AddNewLectureModels()
        {
            foreach (LectureModelsPage page in lectureModelPages)
                page.AddNewLectureModels();
        }

        /* Remove the corresponding lecture model from it's page */
        public void RemoveLectureModel(Lecture lecture)
        {
            foreach (LectureModelsPage page in lectureModelPages)
                if (page.RemoveLecture(lecture))
                    break;
        }

        #region Listview Menu
        private void ListViewItemMonday_Selected(object sender, RoutedEventArgs e)
        {
            FrameDayLectures.Content = lectureModelPages[0];
        }

        private void ListViewItemTuesday_Selected(object sender, RoutedEventArgs e)
        {
            FrameDayLectures.Content = lectureModelPages[1];
        }

        private void ListViewItemWednesday_Selected(object sender, RoutedEventArgs e)
        {
            FrameDayLectures.Content = lectureModelPages[2];
        }

        private void ListViewItemThursday_Selected(object sender, RoutedEventArgs e)
        {
            FrameDayLectures.Content = lectureModelPages[3];
        }
        private void ListViewItemFriday_Selected(object sender, RoutedEventArgs e)
        {
            FrameDayLectures.Content = lectureModelPages[4];
        }
        private void ListViewItemSaturday_Selected(object sender, RoutedEventArgs e)
        {
            FrameDayLectures.Content = lectureModelPages[5];
        }
        private void ListViewItemSunday_Selected(object sender, RoutedEventArgs e)
        {
            FrameDayLectures.Content = lectureModelPages[6];
        }
        #endregion
    }
}
