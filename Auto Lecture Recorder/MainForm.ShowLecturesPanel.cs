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
        // SHOW LECTURES PANEL
        // Used to give unique names to the controls
        int controlsCount = 1;

        // Create all the controls
        private Panel CreateLectureOuterPanel(int x, int y)
        {
            Panel panel = new Panel();
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Location = new Point(x, y);
            panel.Name = "panelLecture" + controlsCount;
            panel.Size = new Size(217, 270);
            panel.TabIndex = 0;

            // Store the control in responsive object
            if (!responsive.InitialControls.ContainsKey(panel.Name))
                responsive.StoreControl(panel);
            // Increment the control count
            controlsCount++;

            return panel;
        }

        private Label CreateLectureTitleLabel(string text)
        {
            Label label = new Label();

            label.BackColor = Color.FromArgb(51, 56, 86);
            label.Dock = DockStyle.Top;
            label.Font = new Font("Century Gothic", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label.ForeColor = Color.FromArgb(253, 147, 41);
            label.Location = new Point(0, 0);
            label.Name = "lectureTitleLabel" + controlsCount;
            label.Size = new Size(215, 89);
            label.TabIndex = 8;
            label.Text = text;
            label.TextAlign = ContentAlignment.MiddleCenter;

            // Store the control in responsive object
            if (!responsive.InitialControls.ContainsKey(label.Name))
                responsive.StoreControl(label);
            // Increment the control count
            controlsCount++;

            return label;
        }

        private Button CreateLectureXButton()
        {
            Button buttonX = new Button();
            buttonX.BackColor = Color.FromArgb(51, 56, 86);
            buttonX.BackgroundImage = Properties.Resources.x_24px;
            buttonX.BackgroundImageLayout = ImageLayout.Center;
            buttonX.FlatAppearance.BorderSize = 0;
            buttonX.FlatStyle = FlatStyle.Flat;
            buttonX.Cursor = Cursors.Hand;
            buttonX.Left = 189;
            buttonX.Top = 0;
            buttonX.Name = "xButton" + controlsCount;
            buttonX.Size = new Size(28, 28);
            buttonX.TabIndex = 13;
            buttonX.UseVisualStyleBackColor = false;
            // Store the control in responsive object
            if (!responsive.InitialControls.ContainsKey(buttonX.Name))
                responsive.StoreControl(buttonX);
            // Increment the control count
            controlsCount++;

            return buttonX;
        }

        private Panel CreateLectureSeparatingPanel()
        {
            Panel panel = new Panel();

            panel.BackColor = Color.FromArgb(100, 100, 100);
            panel.Dock = DockStyle.Top;
            panel.ForeColor = Color.FromArgb(100, 100, 100);
            panel.Location = new Point(0, 77);
            panel.Name = "separatingPanel" + controlsCount;
            panel.Size = new Size(215, 1);
            panel.TabIndex = 12;
            // Store the control in responsive object
            if (!responsive.InitialControls.ContainsKey(panel.Name))
                responsive.StoreControl(panel);
            // Increment the control count
            controlsCount++;

            return panel;
        }

        private Label CreateLectureInfoLabel(string text)
        {
            Label label = new Label();

            label.Dock = DockStyle.Top;
            label.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label.ForeColor = Color.White;
            label.Location = new Point(0, 0);
            label.Name = "infoLabel" + controlsCount;
            label.Size = new Size(215, 45);
            label.TabIndex = 9;
            label.Text = text;
            label.TextAlign = ContentAlignment.MiddleCenter;

            // Store the control in responsive object
            if (!responsive.InitialControls.ContainsKey(label.Name))
                responsive.StoreControl(label);

            // Increment the control count
            controlsCount++;

            return label;
        }


        // Number of lectures that are currently displayed on screen
        int lecturesDisplayedNow = 0;

        // Create a lecture display window in the given location
        private void CreateLectureWindow(int x, int y, Lecture lecture)
        {
            // General panel
            Panel lectureWindow = CreateLectureOuterPanel(x, y);
            // Title bar
            Label title = CreateLectureTitleLabel(lecture.Name);
            Panel separatingPanel = CreateLectureSeparatingPanel();
            // Info
            Label startTime = CreateLectureInfoLabel("Start time: " + lecture.StartTime);
            Label endTime = CreateLectureInfoLabel("End time: " + lecture.EndTime);
            Label platform = CreateLectureInfoLabel("Platform: " + lecture.Platform);
            // Enabled checkbox
            ModernCheckbox modernCheckbox = lecture.CreateCheckbox(responsive, ref controlsCount);
            modernCheckbox.AddClickEvents(new EventHandler((sender, EventArgs) =>
                           modernCheckbox.Lecture_Enable_Click(sender, EventArgs, week[FindSelectedDay()], lecture)));

            // Delete x button
            Button xButton = CreateLectureXButton();
            xButton.Click += (sender, EventArgs) => { buttonDeleteLecture_Click(sender, EventArgs, lecture); };

            // Add the controls to the container panel
            lectureWindow.Controls.Add(xButton);
            lectureWindow.Controls.Add(modernCheckbox);
            lectureWindow.Controls.Add(platform);
            lectureWindow.Controls.Add(endTime);
            lectureWindow.Controls.Add(startTime);
            lectureWindow.Controls.Add(separatingPanel);
            lectureWindow.Controls.Add(title);

            // Add the Lecture window to the container
            responsive.ScaleControl(lectureWindow);
            panelGeneratedLectures.Controls.Add(lectureWindow);

            // Increment the lecture counter
            lecturesDisplayedNow++;
        }

        private void buttonDeleteLecture_Click(object sender, EventArgs e, Lecture lecture)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this lecture?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Remove the lecture from the list
                week[lectureDay].Lectures.Remove(lecture);
                // Redraw the lectures in the lecture panel
                GenerateLectures(FindSelectedDay());

                // Refresh the scheduled recordings
                RefreshScheduledRecordings(sender);

            }

        }

        public String FindSelectedDay()
        {
            foreach (RadioButton day in panelDaysMenu.Controls)
            {
                if (day.Checked)
                {
                    return day.Text;
                }
            }

            return null;
        }

        // Clears the lecture panel from all displayed lectures
        private void ClearDisplayedLectures()
        {
            List<Panel> panelsToRemove = panelGeneratedLectures.Controls.OfType<Panel>().ToList();

            foreach (Panel panel in panelsToRemove)
            {
                // Nullify the checkbox so the lecture object doesnt get an exception
                var checkbox = panel.Controls.OfType<ModernCheckbox>().ToList();
                checkbox[0] = null;
                // Delete the displayed lecture
                panelGeneratedLectures.Controls.Remove(panel);
                panel.Dispose();
            }

            // Reset the lectures displayed now counter and the general control counter
            lecturesDisplayedNow = 0;
            controlsCount = 0;
        }





        // Index that shows from which lecture the DisplayLectures() method should start displaying
        int dayLecturesListIndex = 0;
        string lectureDay;

        // Generate 6 or less lectures and display them on the screen (6 lectures fill the screen)   
        private void DisplayLectures()
        {
            // List containing all the lectures of the given day
            List<Lecture> dayLectures = week[lectureDay].Lectures;

            // Hide the no lectures exist label (it will be shown at the end if no lectures are found for the given day)
            labelLectureExistance.Hide();

            // Clear any previous lectures
            ClearDisplayedLectures();

            // Starting coordinates
            int x = 38;
            int y = 10;
            // for loop counter
            int i;
            // Display 6 or less lectures in the given day depending on the dayLecturesListIndex 
            for (i = dayLecturesListIndex; i < dayLectures.Count; i++)
            {
                // When the first row is filled move to the next
                if (lecturesDisplayedNow == 3)
                {
                    // Reset x
                    x = 38;
                    // increment y
                    y += 285;
                }

                // Create a lecture
                CreateLectureWindow(x, y, dayLectures[i]);

                // Increment x
                x += 280;

                // When the screen is filled move to the next page
                if (lecturesDisplayedNow == 6)
                {
                    // i is incremented here because afterwards we escape from the for loop so i doesn't get incremented as desired
                    i++;
                    break;
                }
            }

            // Update the dayLecturesListIndex
            dayLecturesListIndex = i;

            // Check if next button should be visible
            if (dayLecturesListIndex < dayLectures.Count)
            {
                buttonLecturesNext.Show();
            }
            else
            {
                buttonLecturesNext.Hide();
            }

            // Check if no lectures are generated
            if (lecturesDisplayedNow == 0)
            {
                labelLectureExistance.Show();
            }

        }

        // Determine in which pages we are currently on. Every page contains 6 lectures        
        int pageNum = 0;
        // Show the next lectures
        private void buttonLecturesNext_Click(object sender, EventArgs e)
        {
            pageNum++;
            DisplayLectures();
            buttonLecturesPrevious.Show();
        }

        // Show the previous lectures
        private void buttonLecturesPrevious_Click(object sender, EventArgs e)
        {
            pageNum--;
            dayLecturesListIndex = pageNum * 6;
            if (dayLecturesListIndex < 6)
            {
                buttonLecturesPrevious.Hide();
            }

            DisplayLectures();
        }

        private void GenerateLectures(string day)
        {
            dayLecturesListIndex = 0;
            pageNum = 0;
            lectureDay = day;
            buttonLecturesPrevious.Hide();
            DisplayLectures();
        }




        // Days menu functionality
        private void daysMenuMonday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Monday");
            Lectures.Day.DisableConflictingLectures(week["Monday"]);
        }

        private void daysMenuTuesday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Tuesday");
            Lectures.Day.DisableConflictingLectures(week["Tuesday"]);
        }

        private void daysMenuWednesday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Wednesday");
            Lectures.Day.DisableConflictingLectures(week["Wednesday"]);
        }

        private void daysMenuThursday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Thursday");
            Lectures.Day.DisableConflictingLectures(week["Thursday"]);
        }

        private void daysMenuFriday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Friday");
            Lectures.Day.DisableConflictingLectures(week["Friday"]);
        }

        private void daysMenuSaturday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Saturday");
            Lectures.Day.DisableConflictingLectures(week["Saturday"]);
        }

        private void daysMenuSunday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Sunday");
            Lectures.Day.DisableConflictingLectures(week["Sunday"]);
        }

    }
}
