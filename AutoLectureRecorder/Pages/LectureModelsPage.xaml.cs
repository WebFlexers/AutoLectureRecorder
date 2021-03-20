using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AutoLectureRecorder.Structure;

namespace AutoLectureRecorder.Pages
{
    /// <summary>
    /// Interaction logic for LectureModels.xaml
    /// </summary>
    public partial class LectureModelsPage : Page
    {
        string Day { get; set; }
        List<Lecture> Lectures { get; set; } = new List<Lecture>();
        //List<LectureModel> Models { get; set; } = new List<LectureModel>();
        public LectureModelsPage(string day)
        {
            Day = day;
            InitializeComponent();

            LoadLectures();
            Schedule.DisableConflictingLectures(day);
            SetNoLecturesWarningVisibility();
        }

        /* Initializes the list of lectures and creates the models */
        private void LoadLectures()
        {
            var lectures = Schedule.GetLecturesByDay(Day);
            if (lectures != null)
                foreach (Lecture lecture in lectures)
                {
                    Lectures.Add(lecture);
                    ConfigureLectureModel(lecture);
                    WrapPanelLectures.Children.Add(lecture.Model);
                }
        }

        /* Sets the no lectures warning textblock to visible or invisible accordingly */
        private void SetNoLecturesWarningVisibility()
        {
            var lectures = Schedule.GetLecturesByDay(Day);
            if (lectures != null)
            {
                if (lectures.Count > 0)
                {
                    TextBlockNoLectures.Visibility = Visibility.Hidden;
                }
                else
                {
                    TextBlockNoLectures.Visibility = Visibility.Visible;
                }
            }
            else
            {
                TextBlockNoLectures.Visibility = Visibility.Visible;
            }
        }

        /* Returns a new lecture model */
        private void ConfigureLectureModel(Lecture lecture)
        {
            lecture.InitializeModel();
            lecture.Model.Margin = new Thickness(25, 0, 25, 25);
            lecture.Model.Width = 210;
        }

        /* Finds the lectures that exist in Schedule 
         * but not in this page and adds them */
        public void AddNewLectureModels()
        {
            Application.Current.Dispatcher.BeginInvoke(
            new Action(() =>
            {
                var allLectures = Schedule.GetLecturesByDay(Day);
                if (allLectures != null)
                {
                    List<Lecture> newLectures = allLectures.Except(Lectures).ToList();

                    if (newLectures.Count > 0)
                    {
                        foreach (Lecture lecture in newLectures)
                        {
                            Lectures.Add(lecture);
                            ConfigureLectureModel(lecture);
                            WrapPanelLectures.Children.Add(lecture.Model);
                            Schedule.DisableConflictingLectures(lecture);
                        }
                        SetNoLecturesWarningVisibility();
                        InvalidateVisual();
                    }
                        
                }
            }), DispatcherPriority.Background);
        }

        /* Removes the given lecture and lecture model from the lists and the wrap panel */
        public bool RemoveLecture(Lecture lecture)
        {
            if (Lectures.Contains(lecture))
            {
                Lectures.Remove(lecture);

                WrapPanelLectures.Children.Remove(lecture.Model);
                SetNoLecturesWarningVisibility();
                InvalidateVisual();
                return true;
            }

            return false;
        }
    }
}
