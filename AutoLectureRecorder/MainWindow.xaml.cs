using AutoLectureRecorder.Pages;
using AutoLectureRecorder.Selenium;
using AutoLectureRecorder.Structure;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YoutubeAPI;

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
           
            MenuItemRN.Header = User.RegistrationNumber;
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string userFile = Path.Combine(Serialize.DataDirectory, "user.alr");
            if (File.Exists(userFile))
            {
                File.Delete(userFile);
                User.RegistrationNumber = null;
                User.Password = null;
                User.MicrosoftTeams = null;
            }

            Chrome.Bot.TerminateDriver();

            this.Close();
        }

        #endregion

        #region Menu
        RecordPage recordPage;
        AddLecture addLecturePage = new AddLecture();
        Lectures lecturesPage = new Lectures();
        YoutubeAuthenticate youtubeAuthenticate;
        Youtube youtube = new Youtube();

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
            if (YoutubeUploader.IsAuthenticated)
            {
                ShowYoutubeSection();
            }
            else
            {
                if (youtubeAuthenticate == null)
                    youtubeAuthenticate = new YoutubeAuthenticate();

                FrameMain.Content = youtubeAuthenticate;
            }
            
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

        public void ShowYoutubeSection()
        {
            if (youtube == null)
                youtube = new Youtube();

            FrameMain.Content = youtube;
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

        public ProgressBar CreateYoutubeProgressBar(Lecture lecture)
        {
            return youtube.CreateProgressBar(lecture);
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

        private void Window_Closed(object sender, EventArgs e)
        {
            Chrome.Bot.TerminateDriver();
        }
    }
}
