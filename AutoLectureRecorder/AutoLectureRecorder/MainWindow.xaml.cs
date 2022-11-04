﻿using AutoLectureRecorder.Pages;
using AutoLectureRecorder.Selenium;
using AutoLectureRecorder.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        ChromeBot _chromeBot;
        public MainWindow(ChromeBot bot)
        {
            _chromeBot = bot;
            InitializeComponent();
            InstanciatePages();
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

            TerminateChromeDrivers();

            this.Close();
        }

        #endregion

        #region Menu
        List<Page> _allPages = new List<Page>();
        RecordPage _recordPage;
        AddLecture _addLecturePage;
        Lectures _lecturesPage;
        YoutubeAuthenticate _youtubeAuthenticate;
        Youtube _youtube;
        SettingsPage _settingsPage;

        private void InstanciatePages()
        {
            _allPages.Add(_recordPage);
            _allPages.Add(_addLecturePage);
            _allPages.Add(_lecturesPage);
            _allPages.Add(_youtubeAuthenticate);
            _allPages.Add(_youtube);
            _allPages.Add(_settingsPage);
            _addLecturePage = new AddLecture(_chromeBot);
            _lecturesPage = new Lectures();
            _youtube = new Youtube();
        }
        

        private void MenuRecord_Selected(object sender, RoutedEventArgs e)
        {
            if (_recordPage == null)
                _recordPage = new RecordPage();

            FrameMain.Content = _recordPage;
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
                if (_youtubeAuthenticate == null)
                    _youtubeAuthenticate = new YoutubeAuthenticate();

                FrameMain.Content = _youtubeAuthenticate;
            }
            
        }

        private void MenuSettings_Selected(object sender, RoutedEventArgs e)
        {
            if (_settingsPage == null)
                _settingsPage = new SettingsPage();

            FrameMain.Content = _settingsPage;
        }

        /* Used to change frame content from another page or window */
        public void ShowAddLecturesSection()
        {
            if (_addLecturePage == null)
                _addLecturePage = new AddLecture(_chromeBot);

            FrameMain.Content = _addLecturePage;
        }

        public void ShowLecturesSection()
        {
            if (_lecturesPage == null)
                _lecturesPage = new Lectures();

            FrameMain.Content = _lecturesPage;
        }

        public void ShowYoutubeSection()
        {
            if (_youtube == null)
                _youtube = new Youtube();

            FrameMain.Content = _youtube;
        }
        #endregion

        #region Update data
        public void AddNewLectureModels()
        {
            _lecturesPage.AddNewLectureModels();
            _recordPage.UpdateNextLecture();
            Serialize.SerializeWeekLectures(Schedule.GetSerializableData());
        }

        public void RemoveLecture(Lecture lecture)
        {
            _lecturesPage.RemoveLectureModel(lecture);
            _recordPage.UpdateNextLecture();
            Serialize.SerializeWeekLectures(Schedule.GetSerializableData());
        }

        public ProgressBar CreateYoutubeProgressBar(Lecture lecture)
        {
            return _youtube.CreateProgressBar(lecture);
        }
        #endregion



        /* Close all chrome processes */
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Process[] chromeDriverInstances = Process.GetProcessesByName("chromedriver");

            //foreach (Process p in chromeDriverInstances)
            //    p.Kill();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TerminateChromeDrivers();
        }

        private void TerminateChromeDrivers()
        {
            foreach (Page page in _allPages)
            {
                if (page is IChrome pageWithChrome)
                {
                    new Thread(() => pageWithChrome.TerminateBot()).Start();
                }
            }
        }
    }
}