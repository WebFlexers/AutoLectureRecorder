using AutoLectureRecorder.Pages;
using AutoLectureRecorder.Selenium;
using AutoLectureRecorder.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // Deserialize
            Schedule.LoadSchedule(Serialize.DeserializeWeekLectures());

            InitializeComponent();

            // Instantiate lectures to avoid null exception
            lecturesPage = new Lectures();
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
        AddLecture addLecturePage;
        Lectures lecturesPage;
        Upload uploadPage;

        private void MenuRecord_Selected(object sender, RoutedEventArgs e)
        {
            if (recordPage == null)
                recordPage = new RecordPage();

            FrameMain.Content = recordPage;
        }

        private void MenuAddLectures_Selected(object sender, RoutedEventArgs e)
        {
            ShowAddLecturesSection();
        }

        private void MenuLectures_Selected(object sender, RoutedEventArgs e)
        {
            ShowLecturesSection();
        }

        private void MenuYoutube_Selected(object sender, RoutedEventArgs e)
        {
            if (uploadPage == null)
                uploadPage = new Upload();

            FrameMain.Content = uploadPage;
        }

        private void MenuSettings_Selected(object sender, RoutedEventArgs e)
        {

        }

        /* Used to change frame content from another page or window */
        public void ShowAddLecturesSection()
        {
            if (addLecturePage == null)
                addLecturePage = new AddLecture();

            FrameMain.Content = addLecturePage;
        }

        public void ShowLecturesSection()
        {
            if (lecturesPage == null)
                lecturesPage = new Lectures();

            FrameMain.Content = lecturesPage;
        }
        #endregion

        #region Update data
        public void AddNewLectureModels()
        {
            lecturesPage.AddNewLectureModels();
            recordPage.UpdateNextLecture();
            Serialize.SerializeWeekLectures(Schedule.GetSerializableData());
        }

        public void RemoveLecture(Lecture lecture)
        {
            lecturesPage.RemoveLectureModel(lecture);
            recordPage.UpdateNextLecture();
            Serialize.SerializeWeekLectures(Schedule.GetSerializableData());
        }

        #endregion

        /* Close all chrome processes */
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process[] chromeInstances = Process.GetProcessesByName("chrome");

            foreach (Process p in chromeInstances)
                p.Kill();

            Process[] chromeDriverInstances = Process.GetProcessesByName("chromedriver");

            foreach (Process p in chromeDriverInstances)
                p.Kill();

            Process[] chromeDriverConhost = Process.GetProcessesByName("conhost");

            foreach (Process p in chromeDriverConhost)
                p.Kill();
        }

        
    }
}
