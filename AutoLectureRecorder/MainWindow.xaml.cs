using AutoLectureRecorder.Pages;
using AutoLectureRecorder.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace AutoLectureRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Instantiate lectures to avoid null exception
            lectures = new Lectures();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MenuRecord.IsSelected = true;
        }

        #region Titlebar
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonResize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                ResizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;             
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                ResizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
            }
        }

        private void ButtonWindowState_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                ResizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
            else if (this.WindowState == WindowState.Maximized)
                ResizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
        }
        #endregion

        #region Menu
        RecordPage recordPage;
        AddLecture addLecture;
        Lectures lectures;

        private void MenuRecord_Selected(object sender, RoutedEventArgs e)
        {
            if (recordPage == null)
                recordPage = new RecordPage();

            FrameMain.Content = recordPage;
        }

        private void MenuLectures_Selected(object sender, RoutedEventArgs e)
        {
            ShowLecturesSection();
        }

        private void MenuYoutube_Selected(object sender, RoutedEventArgs e)
        {
            ShowAddLecturesSection();
        }

        private void MenuSettings_Selected(object sender, RoutedEventArgs e)
        {

        }

        /* Used to change frame content from another page or window */
        public void ShowAddLecturesSection()
        {
            if (addLecture == null)
                addLecture = new AddLecture();

            FrameMain.Content = addLecture;
        }

        public void ShowLecturesSection()
        {
            if (lectures == null)
                lectures = new Lectures();

            FrameMain.Content = lectures;
        }
        #endregion

        #region Update data
        public void AddNewLectureModels()
        {
            lectures.AddNewLectureModels();
        }

        public void RemoveLecture(Lecture lecture)
        {
            lectures.RemoveLectureModel(lecture);
        }
        #endregion
    }
}
