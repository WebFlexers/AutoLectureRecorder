using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Auto_Lecture_Recorder.Lectures;
using Auto_Lecture_Recorder.BotController;
using System.Threading;

namespace Auto_Lecture_Recorder
{
    public partial class MainForm : Form
    {
        // Responsive object
        Responsive responsive;
        public MainForm()
        {
            InitializeComponent();
            // Instanciate responsiveness
            responsive = new Responsive(panelMainWindows.Controls, 853, 696);
            // To eliminate flickering
            foreach (Control control in Controls)
            {
                makeControlNonFlickering(control);
            }
        }

        // Make all control and its insides non flickering
        private void makeControlNonFlickering(Control control)
        {        
            SetDoubleBuffered(control);
            foreach (Panel innerPanel in control.Controls.OfType<Panel>())
            {
                makeControlNonFlickering(innerPanel);
            }     
        }
        // List that contains all days of the week
        Dictionary<string, Lectures.Day> week;

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Instanciate the week
            week = new Dictionary<string, Lectures.Day>();
            week.Add("Monday", new Lectures.Day("Monday"));
            week.Add("Tuesday", new Lectures.Day("Tuesday"));
            week.Add("Wednesday", new Lectures.Day("Wednesday"));
            week.Add("Thursday", new Lectures.Day("Thursday"));
            week.Add("Friday", new Lectures.Day("Friday"));
            week.Add("Saturday", new Lectures.Day("Saturday"));
            week.Add("Sunday", new Lectures.Day("Sunday"));
        }

        // Eliminate flickering
        #region .. Double Buffered function ..
        public static void SetDoubleBuffered(Control c)
        {
            if (SystemInformation.TerminalServerSession)
                return;
            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

        #endregion








        // Title bar functionality
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonMaximize_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                buttonMaximize.BackgroundImage = Properties.Resources.restore_down_20px;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                buttonMaximize.BackgroundImage = Properties.Resources.maximize_button_16px;
            }
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // For moving form from title bar
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panelTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }









        // Responsiveness
        private void panelMenu_Resize(object sender, EventArgs e)
        {
            // Change the size of the radio buttons in panel menu when form is resized
            int sizeY = panelMenu.Height / 4;
            foreach (RadioButton radio in panelMenu.Controls.OfType<RadioButton>())
            {
                radio.Size = new Size(radio.Width, sizeY);
            }
        }

        private void panelMainWindows_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                responsive.UpdateFormSize(panelMainWindows.Width, panelMainWindows.Height);
                responsive.ScaleAllControls(panelMainWindows.Controls);
            }
                
        }











        // Menu functionality
        private void showPanel(Panel panelToShow)
        {
            foreach (Panel panel in panelMainWindows.Controls.OfType<Panel>())
            {
                panelToShow.Visible = true;
                if (!panel.Name.Equals(panelToShow.Name))
                {
                    panel.Visible = false;
                }
            }
        }
        private void radioMenuRecord_Click(object sender, EventArgs e)
        {
            showPanel(panelRecord);
        }

        private void radioMenuSubjects_Click(object sender, EventArgs e)
        {
            showPanel(panelLectures);
        }

        private void radioMenuAdd_Click(object sender, EventArgs e)
        {
            showPanel(panelAddLectures);
        }

        private void radioMenuSettings_Click(object sender, EventArgs e)
        {
            showPanel(panelSettings);
        }

        // Menu Styling
        // Reverts all menu images to the white version
        private void revertMenuImages()
        {
            foreach (RadioButton radio in panelMenu.Controls.OfType<RadioButton>())
            {
                if (!radio.Checked)
                {
                    switch (radio.Name)
                    {
                        case "radioMenuRecord":
                            radioMenuRecord.Image = Auto_Lecture_Recorder.Properties.Resources.video_call_white_35px;
                            break;
                        case "radioMenuSubjects":
                            radioMenuSubjects.Image = Auto_Lecture_Recorder.Properties.Resources.book_white_35px;
                            break;
                        case "radioMenuAdd":
                            radioMenuAdd.Image = Auto_Lecture_Recorder.Properties.Resources.add_book_white_35px;
                            break;
                        case "radioMenuSettings":
                            radioMenuSettings.Image = Auto_Lecture_Recorder.Properties.Resources.settings_white_35px;
                            break;
                    }
                }
            }
        }

        // Change to blue on mouse enter and back to white on mouse leave for all menu controls
        // Record
        private void radioMenuRecord_MouseEnter(object sender, EventArgs e)
        {
            radioMenuRecord.Image = Auto_Lecture_Recorder.Properties.Resources.video_call_blue_35px;
        }

        private void radioMenuRecord_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuRecord.Checked)
                radioMenuRecord.Image = Auto_Lecture_Recorder.Properties.Resources.video_call_white_35px;
        }
        // Lecture
        private void radioMenuSubjects_MouseEnter(object sender, EventArgs e)
        {
            radioMenuSubjects.Image = Auto_Lecture_Recorder.Properties.Resources.book_blue_35px;
        }

        private void radioMenuSubjects_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuSubjects.Checked)
                radioMenuSubjects.Image = Auto_Lecture_Recorder.Properties.Resources.book_white_35px;
        }
        // Add subject
        private void radioMenuAdd_MouseEnter(object sender, EventArgs e)
        {
            radioMenuAdd.Image = Auto_Lecture_Recorder.Properties.Resources.add_book_blue_35px;
        }

        private void radioMenuAdd_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuAdd.Checked)
                radioMenuAdd.Image = Auto_Lecture_Recorder.Properties.Resources.add_book_white_35px;
        }
        // Settings
        private void radioMenuSettings_MouseEnter(object sender, EventArgs e)
        {
            radioMenuSettings.Image = Auto_Lecture_Recorder.Properties.Resources.settings_blue_35px;
        }

        private void radioMenuSettings_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuSettings.Checked)
                radioMenuSettings.Image = Auto_Lecture_Recorder.Properties.Resources.settings_white_35px;
        }

        // Revert the rest button images when one is selected
        private void radioMenuRecord_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }

        private void radioMenuSubjects_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }

        private void radioMenuAdd_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }

        private void radioMenuSettings_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }









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
            // A for loop that scans the week days until it finds the correct lecture
            for (int dayNum = 0; dayNum < 7; dayNum++)
            {
                // Get the day that the lecture will take place
                Lectures.Day lectureDay = week[DateTime.Now.AddDays(dayNum).ToString("dddd")];
                // Get a list of lectures of the day defined by dayNum, but starting from today
                List<Lecture> dayLectures = lectureDay.Lectures;
                // Sort the lectures list by start time
                dayLectures = dayLectures.OrderBy(lecture => lecture.StartTime).ToList();

                // Find the first scheduled lecture in the selected day that has a larger start time than the current time 
                // or if the scheduled lecture is for a later day return the first lecture of the next day
                foreach (Lecture lecture in dayLectures)
                {
                    if (lecture.StartTime > DateTime.Now.TimeOfDay || !lectureDay.Name.Equals(DateTime.Now.ToString("dddd")))
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

        private void checkBoxRecordButton_Click(object sender, EventArgs e)
        {
            if (checkBoxRecordButton.Checked)
            {
                nextScheduledLecture = FindNextLectureToBeRecorded();
                // If at least one lecture exists move forward, otherwise show error
                if (nextScheduledLecture != null)
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
                else
                {
                    // Change recording button color to dark red
                    checkBoxRecordButton.BackgroundImage = Properties.Resources.record_button_off;
                    // Hide the recording armed text
                    labelCountdown.Visible = false;
                    // Revert checkbox to unckecked
                    checkBoxRecordButton.Checked = false;
                    // Show error message
                    // if statement to make sure that the error message isnt displayed when the checkBoxRecordButton_Click method
                    // is called programmatically and not through click
                    if (((Control)sender).Name.Equals(checkBoxRecordButton.Name))
                        MessageBox.Show("There are no scheduled recordings." + Environment.NewLine +
                                    "You can add lectures in the Add Lectures section or enable existing lectures in the settings section",
                                    "No scheduled recordings were found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            else
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
        }

        // Timer that updates the remaining time before next lecture label and starts the recording process
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
                // Disable timer
                timerCountdown.Stop();
                // Disable the schedule recording checkbox
                checkBoxRecordButton.Enabled = false;

                // Join meating and start recording
            }

            labelRecordingInfo.Text = "The next lecture to be recorded is:" + Environment.NewLine +
                                      nextScheduledLecture.Name + Environment.NewLine + "The recording will start in:";

            string displayFormat = "dd\\:hh\\:mm\\:ss";
            if (numOfDaysFromToday == 0)
            {
                displayFormat = "hh\\:mm\\:ss";
            }

            labelCountdown.Text = remainingTime.ToString(displayFormat);
        }



















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
            panel.Size = new Size(217, 230);
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
            // Delete x button
            Button xButton = CreateLectureXButton();
            xButton.Click += (sender, EventArgs) => { buttonDeleteLecture_Click(sender, EventArgs, lecture); };

            // Add the controls to the container panel
            lectureWindow.Controls.Add(xButton);
            lectureWindow.Controls.Add(platform);
            lectureWindow.Controls.Add(endTime);
            lectureWindow.Controls.Add(startTime);
            lectureWindow.Controls.Add(separatingPanel);
            lectureWindow.Controls.Add(title);

            // Add the Lecture window to the container
            responsive.ScaleControl(lectureWindow);
            panelLectures.Controls.Add(lectureWindow);

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
                foreach (RadioButton radioButton in panelDaysMenu.Controls)
                {
                    if (radioButton.Checked)
                    {
                        GenerateLectures(radioButton.Text);
                        break;
                    }
                }
                // Refresh the countdown timer
                timerCountdown.Stop();
                checkBoxRecordButton_Click(sender, EventArgs.Empty);
                checkBoxRecordButton_Click(sender, EventArgs.Empty);
            }
            
        }

        // Clears the lecture panel from all displayed lectures
        private void ClearDisplayedLectures()
        {
            List<Panel> panelsToRemove = panelLectures.Controls.OfType<Panel>().ToList();

            foreach (Panel panel in panelsToRemove)
            {
                if (!panel.Name.Equals("panelDaysMenu"))
                {
                    panelLectures.Controls.Remove(panel);
                    panel.Dispose();
                }
            }

            // Reset the lectures displayed now counter and the general control counter
            lecturesDisplayedNow = 0;
            controlsCount = 0;
        }





        // Index that shows from which element the DisplayLectures() method needs to start displaying
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
            int y = 151;
            // counter for for loop
            int i;
            // Display all the lectures in the given day
            for (i = dayLecturesListIndex; i < dayLectures.Count; i++)
            {
                // When the first row is filled move to the next
                if (lecturesDisplayedNow == 3)
                {
                    // Reset x
                    x = 38;
                    // increment y
                    y += 255;
                }

                // Create a lecture
                CreateLectureWindow(x, y, dayLectures[i]);

                // Increment x
                x += 280;
                              
                // When the screen is filled move to the next page
                if (lecturesDisplayedNow == 6)
                {
                    i++;
                    break;
                }
            }

            // Save the dayLecturesListIndex
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

        // Every page contains 6 lectures
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
        }

        private void daysMenuTuesday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Tuesday");
        }

        private void daysMenuWednesday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Wednesday");
        }

        private void daysMenuThursday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Thursday");
        }

        private void daysMenuFriday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Friday");
        }

        private void daysMenuSaturday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Saturday");
        }

        private void daysMenuSunday_Click(object sender, EventArgs e)
        {
            GenerateLectures("Sunday");
        }















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
            for (int i = 1; i <= 24; i++)
            {
                list.AddOption(i.ToString());
            }
        }

        private void displayMinutes(DropdownList list)
        {
            for (int i = 1; i <= 60; i++)
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
            Lectures.Day storedDay= week[dropdownDay.GetText()];
            TimeSpan startTime;
            TimeSpan endTime;
            string platform;

            // Check if everything is filled
            if (String.IsNullOrEmpty(textboxLectureName.GetText()) || String.IsNullOrEmpty(dropdownDay.GetText()) ||
                String.IsNullOrEmpty(dropdownPlatform.GetText()) || String.IsNullOrEmpty(dropdownStartHour.GetText()) ||
                String.IsNullOrEmpty(dropdownStartMin.GetText()) || String.IsNullOrEmpty(dropdownEndHour.GetText()) ||
                String.IsNullOrEmpty(dropdownEndMin.GetText()))
            {
                MessageBox.Show("Please fill all the fields to continue", "Empty fields detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // Next make sure that the end time is bigger than the start time
            else
            {
                // Get the time info          
                startTime = new TimeSpan(Convert.ToInt32(dropdownStartHour.GetText()), Convert.ToInt32(dropdownStartMin.GetText()), 0);
                endTime = new TimeSpan(Convert.ToInt32(dropdownEndHour.GetText()), Convert.ToInt32(dropdownEndMin.GetText()), 0);

                // if the time isn't valid show error
                if (!Lectures.Lecture.timeIsValid(startTime, endTime))
                {
                    MessageBox.Show("The end time can't be smaller that the start time!", "Incorrect time", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Get the given name
                givenLectureName = textboxLectureName.GetText();
                // If the given lecture name already exists in the given day display error message                       
                if (storedDay.Lectures.Exists(storedLecture => storedLecture.Name.Equals(givenLectureName)))
                {
                    StringBuilder errorMessage = new StringBuilder();
                    errorMessage.Append(givenLectureName);
                    errorMessage.Append(" already exists in ");
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
            Lectures.Lecture lecture = new Lectures.Lecture(givenLectureName, startTime, endTime, platform);
            // Add the lecture to the correct day
            storedDay.Lectures.Add(lecture);
            // Success message
            MessageBox.Show("Successfully added lecture!", "Success");

            // Redraw the lectures in the lecture panel
            foreach (RadioButton radioButton in panelDaysMenu.Controls)
            {
                if (radioButton.Checked)
                {
                    GenerateLectures(radioButton.Text);
                }
            }
        }

        // temp
        private void label10_Click(object sender, EventArgs e)
        {
            ClearDisplayedLectures();
        }

        
    }
}
