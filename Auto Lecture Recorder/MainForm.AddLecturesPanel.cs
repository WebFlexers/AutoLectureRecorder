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
using Auto_Lecture_Recorder.BotController;
using Auto_Lecture_Recorder.ScreenRecorder;
using Auto_Lecture_Recorder.Youtube;
using System.IO;

namespace Auto_Lecture_Recorder
{
    partial class MainForm
    {
        // ADD LECTURES PANEL
        // Add the options to all dropdown menus
        private void dropdownDay_Load(object sender, EventArgs e)
        {
            dropdownDay.AddOption("Monday");
            dropdownDay.AddOption("Tuesday");
            dropdownDay.AddOption("Wednesday");
            dropdownDay.AddOption("Thursday");
            dropdownDay.AddOption("Friday");
            dropdownDay.AddOption("Saturday");
            dropdownDay.AddOption("Sunday");
        }

        private void displayHours(DropdownList list)
        {
            for (int i = 0; i <= 24; i++)
            {
                list.AddOption(i.ToString());
            }
        }

        private void displayMinutes(DropdownList list)
        {
            for (int i = 0; i <= 60; i++)
            {
                list.AddOption(i.ToString());
            }
        }

        private void dropdownPlatform_Load(object sender, EventArgs e)
        {
            dropdownPlatform.AddOption("Microsoft Teams");
            dropdownPlatform.AddOption("Webex");
        }

        // Load the options
        private void dropdownStartHour_Load(object sender, EventArgs e)
        {
            displayHours((DropdownList)sender);
        }

        private void dropdownStartMin_Load(object sender, EventArgs e)
        {
            displayMinutes((DropdownList)sender);
        }

        // Add the lecture to the list
        private void buttonAddSubject_Click(object sender, EventArgs e)
        {
            // All field variables
            string givenLectureName;
            Lectures.Day storedDay;
            TimeSpan startTime;
            TimeSpan endTime;
            string platform;

            // Check if everything is filled
            if (String.IsNullOrWhiteSpace(textboxLectureName.GetText()) || String.IsNullOrWhiteSpace(dropdownDay.GetText()) ||
                String.IsNullOrWhiteSpace(dropdownPlatform.GetText()) || String.IsNullOrWhiteSpace(dropdownStartHour.GetText()) ||
                String.IsNullOrWhiteSpace(dropdownStartMin.GetText()) || String.IsNullOrWhiteSpace(dropdownEndHour.GetText()) ||
                String.IsNullOrWhiteSpace(dropdownEndMin.GetText()))
            {
                MessageBox.Show("Please fill all the fields to continue", "Empty fields detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // Next make sure that the end time is bigger than the start time
            else
            {
                // Get day
                storedDay = week[dropdownDay.GetText()];
                // Get the time info
                startTime = new TimeSpan(Convert.ToInt32(dropdownStartHour.GetText()), Convert.ToInt32(dropdownStartMin.GetText()), 0);
                endTime = new TimeSpan(Convert.ToInt32(dropdownEndHour.GetText()), Convert.ToInt32(dropdownEndMin.GetText()), 0);

                // if the time isn't valid show error
                if (!Lecture.timeIsValid(startTime, endTime))
                {
                    MessageBox.Show("The end time has to be larger that the start time!", "Incorrect time input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Get the given name
                givenLectureName = textboxLectureName.GetText();
                // If the given lecture name already exists in the given day display error message                       
                if (storedDay.Lectures.Exists(storedLecture => storedLecture.Name.Equals(givenLectureName)))
                {
                    StringBuilder errorMessage = new StringBuilder();
                    errorMessage.Append(givenLectureName);
                    errorMessage.Append(" already exists on ");
                    errorMessage.Append(storedDay.Name);
                    errorMessage.Append("!");
                    errorMessage.Append(Environment.NewLine);
                    errorMessage.Append(Environment.NewLine);
                    errorMessage.Append("If you want to modify the lecture remove it");
                    errorMessage.Append(Environment.NewLine);
                    errorMessage.Append("from the lectures section and add it again");
                    MessageBox.Show(errorMessage.ToString(), "Lecture is already present", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }

            }

            platform = dropdownPlatform.GetText();

            // Create Lecture object
            Lecture lecture = new Lecture(givenLectureName, startTime, endTime, platform);
            // Add the lecture to the correct day
            storedDay.Lectures.Add(lecture);
            // Success message
            MessageBox.Show("Successfully added lecture!", "Success");

            // Redraw the lectures in the lecture panel depending on which day is selected on the daysMenu
            foreach (RadioButton radioButton in panelDaysMenu.Controls)
            {
                if (radioButton.Checked)
                {
                    GenerateLectures(radioButton.Text);
                    break;
                }
            }

            // Find and disable any conflicting lectures
            foreach (Lecture activeLecture in storedDay.Lectures)
            {
                if (activeLecture.Active && activeLecture != lecture)
                {
                    storedDay.DisableConflictingLectures(activeLecture);
                }
            }

            // Update the scheduled recordings
            RefreshScheduledRecordings(sender);
        }
    }
}