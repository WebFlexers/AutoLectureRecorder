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
using System.Diagnostics;
using System.Windows.Threading;
using System.Globalization;
using AutoLectureRecorder.Structure;
using System.Linq;
using MoreLinq;
using System.Threading.Tasks;
using System.Threading;
using AutoLectureRecorder.Selenium;

namespace AutoLectureRecorder.Pages
{
    /// <summary>
    /// Interaction logic for RecordPage.xaml
    /// </summary>
    public partial class RecordPage : Page, IChrome
    {
        public ChromeBot ChromeBot { get; set; }

        public void LoadBot(ChromeBot bot)
        {
            ChromeBot = bot;
            ChromeBot.IsBrowserHidden = false;
        }

        public void TerminateBot()
        {
            new Thread(() => ChromeBot.TerminateDriver()).Start();
        }

        public RecordPage()
        {
            LoadBot(new ChromeBot());
            InitializeComponent();
            Load();
        }

        DispatcherTimer timerStartLecture;
        private void Load()
        {
            // Hide next lecture and time
            HideUI();
            // Initiate the timer
            timerStartLecture = new DispatcherTimer();
            timerStartLecture.IsEnabled = false;
            timerStartLecture.Interval = TimeSpan.FromSeconds(1);
            timerStartLecture.Tick += TimerStartLecture_Tick;
        }

        bool _isRecordButtonClicked = false;
        /* Starts or stops the timer to the next lecture and updates the UI accordingly */
        private void ButtonRecord_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecordButtonClicked)
                DeactivateRecordButton();
            else if (CanStartLecture())
                ActivateRecordButton();
            else
                MessageBox.Show("No active lectures were found. Create or activate lectures to continue", "Unable to schedule lectures",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void DeactivateRecordButton()
        {
            RecordEllipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#AD2817");
            _isRecordButtonClicked = false;
            timerStartLecture.Stop();
            HideUI();
        }

        private void ActivateRecordButton()
        {
            RecordEllipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#DD331D");
            _isRecordButtonClicked = true;
            TextBlockNextLecture.Text = nextLecture.Name;
            TimerStartLecture_Tick(ButtonRecord, EventArgs.Empty);
            timerStartLecture.Start();
            ShowUI();
        }

        bool IsUserLoggedIn = true;
        Lecture nextLecture;
        private bool CanStartLecture()
        {
            nextLecture = FindNextLecture();
            if (nextLecture != null && IsUserLoggedIn)
                return true;
            else
                return false;
        }

        /* Finds the closest lecture that can be recorded */
        private Lecture FindNextLecture()
        {
            int indexOfToday = Schedule.AllDaysIndexes[Schedule.Today];
            for (int i = 0; i < 7; i++)
            {
                // Make sure to check all days
                int dayIndex = indexOfToday + i;
                if (dayIndex >= 7)
                    dayIndex -= 7;

                string day = Schedule.AllDaysIndexes.ElementAt(dayIndex).Key;

                List<Lecture> dayLectures = Schedule.GetLecturesByDay(day);
                if (dayLectures != null)
                {
                    List<Lecture> validLectures;

                    // If the day is today then get all active lectures that begin after DateTime.Now
                    if (day.Equals(Schedule.Today))
                        validLectures = Schedule.TodaysLectures.Where(l => (l.IsLectureActive)
                                                && (l.StartTime > DateTime.Now.TimeOfDay)).ToList();
                    else
                        validLectures = Schedule.GetLecturesByDay(day).Where(l => (l.IsLectureActive)).ToList();

                    if (validLectures.Count > 0)
                        return validLectures.MinBy(l => l.StartTime).FirstOrDefault();
                }
            }
            return null;
        }

        TimeSpan remainingTime;
        private void TimerStartLecture_Tick(object sender, EventArgs e)
        {
            // Compute how many days far is the next lecture from today
            int todayIndex = Schedule.AllDaysIndexes[Schedule.Today];
            int lectureIndex = Schedule.AllDaysIndexes[nextLecture.Day];
            int daysDistance;

            if (todayIndex > lectureIndex)
                daysDistance = lectureIndex - todayIndex + 7;
            else
                daysDistance = lectureIndex - todayIndex;

            // Compute the remaining time for the lecture
            remainingTime = nextLecture.StartTime - DateTime.Now.TimeOfDay;
            var remainingTimeWithDays = remainingTime.Add(TimeSpan.FromDays(daysDistance));

            if (remainingTimeWithDays.Days > 0)
                TextBlockTime.Text = remainingTimeWithDays.ToString("d\\:hh\\:mm\\:ss");
            else
                TextBlockTime.Text = remainingTimeWithDays.ToString("hh\\:mm\\:ss");

            if (remainingTimeWithDays <= TimeSpan.Zero)
            {
                timerStartLecture.Stop();
                new Thread(StartLecture).Start();
            }
        }

        public void UpdateNextLecture()
        {
            nextLecture = FindNextLecture();

            if (nextLecture == null)
                DeactivateRecordButton();
            else
                TextBlockNextLecture.Text = nextLecture.Name;
        }

        ScreenRecorder recorder = new ScreenRecorder();
        bool _isLectureActive = false;
        private void StartLecture()
        {
            ChromeBot.StartDriver();

            if (ChromeBot.ConnectToMeetingByName(nextLecture.MeetingTeam, User.RegistrationNumber, User.Password))
            {
                _isLectureActive = true;
                // Schedule stop lecture
                try
                {
                    Task.Delay(nextLecture.EndTime - DateTime.Now.TimeOfDay).ContinueWith(_ => { StopLecture(); });
                    Task.Delay(TimeSpan.FromMinutes(30)).ContinueWith(_ => { CheckParticipants(); });
                }
                catch (Exception e)
                {
                    Trace.WriteLine("End time scheduling failed with error message: " + e.Message);
                    ChromeBot.TerminateDriver();
                    return;
                }
                // Start recording
                recorder.CreateRecording(nextLecture.Name);
                _isRecordButtonClicked = true;
            }
            else
            {
                Trace.WriteLine("Failed to connect to teams meeting " + nextLecture.MeetingTeam);
                // Schedule next lecture
                Dispatcher.Invoke(() =>
                {
                    DeactivateRecordButton();
                    if (CanStartLecture())
                    {
                        ActivateRecordButton();
                    }
                });
                ChromeBot.TerminateDriver();
            }
        }

        private void StopLecture()
        {
            if (_isLectureActive)
            {
                Trace.WriteLine("Stop Lecture Started");
                ChromeBot.TerminateDriver();

                Trace.WriteLine("Exited chrome driva");
                ProgressBar progressBar = null;
                Dispatcher.Invoke(() => 
                {
                    if (nextLecture.IsAutoUploadActive)
                    {
                        progressBar = ((MainWindow)Application.Current.Windows[1]).CreateYoutubeProgressBar(nextLecture);
                    }
                });
                
                Trace.WriteLine("Made it to end recording");
                recorder.EndRecording(nextLecture.IsAutoUploadActive, progressBar);

                Trace.WriteLine("_isRecordButtonClicked is " + _isRecordButtonClicked);
                if (_isRecordButtonClicked)
                {
                    Trace.WriteLine("Update next lecture started");
                    Dispatcher.Invoke(() =>
                    {
                        DeactivateRecordButton();
                        if (CanStartLecture())
                        {
                            ActivateRecordButton();
                        }
                    });
                }

                _isLectureActive = false;
            }
        }

        private void CheckParticipants()
        {
            //try
            //{
            //    while (_isLectureActive)
            //    {
            //        if (ChromeBot.GetParticipantsNumber() <= 20)
            //        {
            //            StopLecture();
            //        }
            //        Thread.Sleep(TimeSpan.FromMinutes(5));
            //    }
            //}
            //catch (Exception e)
            //{
            //    Trace.WriteLine(e.Message);
            //}
        }

        private void ShowUI()
        {
            StackPanelNextLecture.Visibility = Visibility.Visible;
            StackPanelStartTime.Visibility = Visibility.Visible;
        }

        private void HideUI()
        {
            StackPanelNextLecture.Visibility = Visibility.Hidden;
            StackPanelStartTime.Visibility = Visibility.Hidden;
        }
    }
}
