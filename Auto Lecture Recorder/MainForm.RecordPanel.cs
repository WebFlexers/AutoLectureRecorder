using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using Auto_Lecture_Recorder.Lectures;
using Auto_Lecture_Recorder.BotController.Unipi;
using Auto_Lecture_Recorder.ScreenRecorder;
using Auto_Lecture_Recorder.Youtube;
using System.IO;
using System.Security.AccessControl;

namespace Auto_Lecture_Recorder
{
    partial class MainForm
    {
        
        // RECORD PANEL

        // Record button Styling
        private void checkBoxRecordButton_MouseEnter(object sender, EventArgs e)
        {
            checkBoxRecordButton.BackgroundImage = Properties.Resources.record_button_on;
        }

        private void checkBoxRecordButton_MouseLeave(object sender, EventArgs e)
        {
            if (!checkBoxRecordButton.Checked)
                checkBoxRecordButton.BackgroundImage = Properties.Resources.record_button_off;
        }


        // Record button functionality
        // Variable that contains the number of days that the next scheduled recording is away
        int numOfDaysFromToday;
        Lecture nextScheduledLecture;
        private Lecture FindNextLectureToBeRecorded()
        {
            // A for loop that scans the week days until it finds the correct lecture (It starts from today)
            for (int dayNum = 0; dayNum <= 7; dayNum++)
            {
                // Get the day that the lecture will take place starting from today, since dayNum starts with 0
                Lectures.Day lectureDay = week[DateTime.Now.AddDays(dayNum).ToString("dddd")];
                // Get a list of lectures of the day defined by dayNum
                List<Lecture> dayLectures = lectureDay.Lectures;
                // Sort the lectures list by start time
                dayLectures = dayLectures.OrderBy(lecture => lecture.StartTime).ToList();

                // Find the first scheduled lecture in the selected day that has a larger start time than the current time 
                // or if the scheduled lecture is for a later day return the first lecture of the closest day
                foreach (Lecture lecture in dayLectures)
                {
                    if (lecture.Active && (lecture.StartTime > DateTime.Now.TimeOfDay || dayNum != 0))
                    {
                        // Save the current day
                        numOfDaysFromToday = dayNum;
                        // Return the correct lecture
                        return lecture;
                    }
                }
            }

            // If no lectures are scheduled return null
            return null;
        }

        private void ScheduleLecture()
        {
            // Start the timer                   
            timerCountdown.Start();
            // Execute the timer tick to eliminate delay
            timerCountdown_Tick(timerCountdown, EventArgs.Empty);
            // Change recording button color to bright red
            checkBoxRecordButton.BackgroundImage = Properties.Resources.record_button_on;
            // Show the recording armed text
            labelCountdown.Visible = true;
        }

        private void CancelScheduledLecture()
        {
            // Change recording button color to dark red
            checkBoxRecordButton.BackgroundImage = Properties.Resources.record_button_off;
            // Hide the recording armed text
            labelCountdown.Visible = false;
            // Change recording info text
            labelRecordingInfo.Text = "No recording is scheduled";
            // Revert checkbox to unckecked
            checkBoxRecordButton.Checked = false;
            // Stop the timer
            timerCountdown.Stop();
        }

        private void checkBoxRecordButton_Click(object sender, EventArgs e)
        {
            if (teamsAuthenticated)
            {
                if (checkBoxRecordButton.Checked)
                {
                    nextScheduledLecture = FindNextLectureToBeRecorded();
                    // If at least one lecture exists move forward, otherwise show error
                    if (nextScheduledLecture != null)
                    {
                        ScheduleLecture();
                    }
                    else
                    {
                        CancelScheduledLecture();
                        // Show error message
                        // if statement to make sure that the error message isnt displayed when the checkBoxRecordButton_Click method
                        // is called programmatically and not through click
                        if (((Control)sender).Name.Equals(checkBoxRecordButton.Name))
                            MessageBox.Show("No enabled lectures exist." + Environment.NewLine +
                                        "You can add lectures in the Add Lectures section or enable existing lectures in the settings section",
                                        "No active lectures were found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                else
                {
                    CancelScheduledLecture();
                }
            }
            else if (((Control)sender).Name.Equals(checkBoxRecordButton.Name))
            {
                CancelScheduledLecture();
                MessageBox.Show("In order to schedule a recording you must be logged in to your institution's account. You can login" +
                                " at the authentication section", "Authentication required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                CancelScheduledLecture();
            }
        }

        // Timer that updates the remaining time before next lecture label and starts the recording process when it reaches 0
        private void timerCountdown_Tick(object sender, EventArgs e)
        {

            int hours = DateTime.Now.Hour;
            int minutes = DateTime.Now.Minute;
            int seconds = DateTime.Now.Second;
            TimeSpan timeNow = new TimeSpan(hours, minutes, seconds);
            TimeSpan timeNextLecture = new TimeSpan(numOfDaysFromToday, nextScheduledLecture.StartTime.Hours,
                                                    nextScheduledLecture.StartTime.Minutes, 0);

            TimeSpan remainingTime = timeNextLecture - timeNow;

            if (remainingTime.TotalSeconds <= 0)
            {
                // Make the countdown 0
                remainingTime = new TimeSpan(0);
                labelCountdown.Text = remainingTime.ToString("hh\\:mm\\:ss");

                // Disable timer
                timerCountdown.Stop();

                // Update the scheduled lectures
                // RefreshScheduledRecordings(sender);

                // Update the number of participants that is needed to keep recording
                minimumParticipantsLeft = Convert.ToInt32(textboxAutoQuitNum.GetText());
                // Join meating and start recording
                Thread joinAndRecord = new Thread(JoinMeatingAndRecord);
                joinAndRecord.IsBackground = true;
                joinAndRecord.Start();

            }
            else
            {
                labelRecordingInfo.Text = "The next lecture to be recorded is:" + Environment.NewLine +
                                      nextScheduledLecture.Name + Environment.NewLine + "The recording will start in:";

                string displayFormat = "dd\\:hh\\:mm\\:ss";
                if (numOfDaysFromToday == 0)
                {
                    displayFormat = "hh\\:mm\\:ss";
                }

                labelCountdown.Text = remainingTime.ToString(displayFormat);
            }

        }



        private void timerCheckParticipants_Tick(object sender, EventArgs e)
        {
            if (timerEndtime.Enabled)
            {
                int participantsNumber = 0;
                try
                {
                    participantsNumber = teamsBot.GetParticipantsNumber();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Lefteris inted. The GetParticipants method threw an exception");
                    Console.WriteLine(exception.Message);    
                }

                if (participantsNumber < minimumParticipantsLeft)
                {
                    timerCheckParticipants.Stop();
                    ExitLectureAndSave();
                }
            }
            else
            {
                timerCheckParticipants.Stop();
            }
        }



        delegate void ScheduleNextLecture(object sender);
        TimeSpan timerIntervalSeconds;
        TimeSpan timeElapsed = new TimeSpan(0);
        TimeSpan checkParticipantsTime = new TimeSpan(0, 45, 0);
        private void timerEndtime_Tick(object sender, EventArgs e)
        {
            /* Find the remaining time of the lecture according to the set lecture end time */
            timerIntervalSeconds = new TimeSpan(0, 0, timerEndtime.Interval * 1000);
            timeElapsed.Add(timerIntervalSeconds);
            LabelActiveTimeElapsed.Text = timeElapsed.ToString();
            labelActiveRemainingTime.Text = nextScheduledLecture.EndTime.Subtract(timeElapsed).ToString();

            TimeSpan timeNow = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            TimeSpan timeEnd = nextScheduledLecture.EndTime;

            TimeSpan remainingTime = timeEnd - timeNow;

            if (remainingTime.TotalSeconds <= 0)
            {
                ExitLectureAndSave();
            }

            /* Start to check for participants number after 45 minutes */
            if (checkParticipantsTime >= timeElapsed && !timerCheckParticipants.Enabled)
            {
                timerCheckParticipants.Start();
            }

        }
        
        private void ExitLectureAndSave()
        {
            timerEndtime.Stop();
            recorder.StopRecording();
            teamsBot.TerminateDriver();

            // Schedule the next lecture
            ScheduleNextLecture scheduleNextLecture = RefreshScheduledRecordings;
            panelRecord.Invoke(scheduleNextLecture, panelRecord);

            // Rename the file
            string videoName = recorder.VideoName + " " + DateTime.Now.ToString("dd-MM-yyyy");
            string newVideoPath = Path.Combine(recorder.VideoFolderPath, videoName + ".mp4");

            // Move to the recording folder
            MoveRecordingToRecFolder(recorder.RecordingPath, newVideoPath);

            // Upload to youtube
            if (checkboxSettingsYoutube.Checked)
            {
                UploadRecording(newVideoPath, videoName, recorder.VideoName);
                panelYoutubeUpload.Show();
                labelYoutubeUploadStatus.ForeColor = Color.FromArgb(42, 123, 245);
                labelYoutubeUploadStatus.Text = "Uploading...";
                progressBarYoutube.Value = 0;
                timerUpdateYoutubeProgressbar.Start();

                // Serialize youtube playlists
                Serializer.SerializeYoutubePlaylists(youtubeUploader.Playlists);
            }

            // Delete the temp recording
            DeleteTempRecording();
        }

        private void UploadRecording(string videoPath, string youtubeVideoName, string playlistName)
        {
            // Upload to youtube
            try
            {
                Thread youtubeThread = new Thread(() =>
                {
                    youtubeUploader.Run(videoPath, youtubeVideoName, playlistName,
                                        "Your daily lecture delivery. Powered by Auto Lecture Recorder").Wait();
                });
                youtubeThread.IsBackground = true;
                youtubeThread.Start();
                Console.WriteLine("Video has begun uploading");

            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void MoveRecordingToRecFolder(string currentVideoName, string newVideoName)
        {
            FileInfo recordingFile = new FileInfo(currentVideoName);
            if (recordingFile.Exists)
            {
                Console.WriteLine(newVideoName);
                recordingFile.MoveTo(newVideoName);   
            }
            else
            {
                Console.WriteLine("Wrong path");
            }
        }

        private void DeleteTempRecording()
        {
            // Delete temp file
            string fileToDeletePath = Path.Combine(Path.GetTempPath(), "temp_recording.mp4");
            if (File.Exists(fileToDeletePath))
            {
                File.Delete(fileToDeletePath);
                Console.WriteLine("Successfully deleted temp file");
            }
        }

        private void timerUpdateYoutubeProgressbar_Tick(object sender, EventArgs e)
        {
            labelYoutubeUploadStatus.ForeColor = Color.FromArgb(42, 123, 245);

            // Updates the progressbar and sets it to the maximum if the uploading is done 
            if (youtubeUploader.UpdateProgressBar(progressBarYoutube))
            {
                progressBarYoutube.Maximum = 100;
                progressBarYoutube.Value = 100;
                labelYoutubeUploadStatus.ForeColor = Color.FromArgb(151, 253, 30);
                labelYoutubeUploadStatus.Text = "Successfully Uploaded to youtube!";
                timerUpdateYoutubeProgressbar.Stop();
            }
        }

        delegate void TimerMethods();    
        private void JoinMeatingAndRecord()
        {
            
            // Join meating
            teamsBot.StartDriver();
            if (teamsBot.ConnectToTeams(RN, password))
            {
                if (teamsBot.ConnectToTeamsMeeting(nextScheduledLecture.Name))
                {
                    // Record
                    recorder.StartRecording(nextScheduledLecture.Name);

                    // Show Active recording panel
                    ShowPanel(panelActiveRecording);

                    // Update the information
                    labelActiveStartTime.Text = nextScheduledLecture.StartTime.ToString();
                    labelActiveEndTime.Text = nextScheduledLecture.EndTime.ToString();

                    // End time timer
                    TimerMethods initiateTimers = new TimerMethods(timerEndtime.Start);
                    panelRecord.Invoke(initiateTimers);

                }
                else
                {
                    teamsBot.TerminateDriver();
                    Console.WriteLine("Unable to connect to teams");
                }
            }
            else
            {
                Console.WriteLine("Wrong Credentials");
            }

            
        }


        public void RefreshScheduledRecordings(object sender)
        {
            // Stop the timer to avoid exceptions
            timerCountdown.Stop();
            // Click the check box record button twice. The goal here is to find the next lecture to be recorded in case the previous
            // one was just deleted
            checkBoxRecordButton_Click(sender, EventArgs.Empty);
            checkBoxRecordButton_Click(sender, EventArgs.Empty);
        }
    }
}
