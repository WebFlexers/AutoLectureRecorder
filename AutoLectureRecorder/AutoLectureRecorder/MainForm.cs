using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoLectureRecorder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // Days list
        private List<Day> days = new List<Day>();

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadSchedule();
            displaySchedule(panelMonday);
        }

        private void loadSchedule() 
        {
            // Load Days and subjects
            // Monday
            List<Subject> subjectsMonday = new List<Subject>();
            subjectsMonday.Add(new Subject("Operating Systems", 14, 0, 16, 15, "MicrosoftTeams"));
            days.Add(new Day("Monday", subjectsMonday));

            // Tuesday
            List<Subject> subjectsTuesday = new List<Subject>();
            subjectsTuesday.Add(new Subject("Statistics", 10, 10, 12, 5, "MicrosoftTeams"));
            subjectsTuesday.Add(new Subject("Math Programming", 12, 10, 14, 5, "MicrosoftTeams"));
            subjectsTuesday.Add(new Subject("Object Oriented Programming", 14, 10, 16, 5, "MicrosoftTeams"));
            days.Add(new Day("Tuesday", subjectsTuesday));

            // Wednesday
            List<Subject> subjectsWednesday = new List<Subject>();
            subjectsWednesday.Add(new Subject("Compilers", 10, 10, 12, 5, "Webex"));
            subjectsWednesday.Add(new Subject("Operating Systems", 12, 10, 14, 5, "MicrosoftTeams"));
            days.Add(new Day("Wednesday", subjectsWednesday));

            // Thursday
            List<Subject> subjectsThrursday = new List<Subject>();
            subjectsThrursday.Add(new Subject("Object Oriented Programming", 8, 15, 10, 5, "MicrosoftTeams"));
            subjectsThrursday.Add(new Subject("Statistics", 10, 10, 12, 5, "MicrosoftTeams"));
            subjectsThrursday.Add(new Subject("Operating Systems", 14, 10, 16, 5, "MicrosoftTeams"));
            days.Add(new Day("Thursday", subjectsThrursday));

            // Friday
            List<Subject> subjectsFriday = new List<Subject>();
            subjectsFriday.Add(new Subject("Math Programming", 10, 10, 12, 5, "MicrosoftTeams"));
            subjectsFriday.Add(new Subject("Compilers", 14, 10, 16, 5, "Webex"));
            days.Add(new Day("Friday", subjectsFriday));

            // Saturday
            List<Subject> subjectsSaturday = new List<Subject>();
            subjectsSaturday.Add(new Subject("Analysis", 9, 55, 2, 5, "MicrosoftTeams"));
            days.Add(new Day("Saturday", subjectsSaturday));
        }

        private void hidePreviousSchedule()
        {
            foreach (Panel panel in panelDaySubjects.Controls.OfType<Panel>())
            {
                panel.Visible = false;        
            }
        }

        private void displaySchedule(Panel dayPanel)
        {
            int dayCounter = 5; // Starts with 5 because foreach starts counting from Saturday to Monday
            foreach (RadioButton radio in panelDays.Controls.OfType<RadioButton>())
            {
                if (radio.Checked)
                {
                    int top = 15;
                    foreach (Subject subject in days[dayCounter].Subjects)
                    {
                        // Display every subject of the selected day as checkbox
                        CheckBox checkbox = new CheckBox();

                        checkbox.Font = new Font("Century Gothic", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                        // Set the text
                        StringBuilder text = new StringBuilder();
                        text.Append(subject.Name);
                        text.Append(" (");
                        text.Append(subject.LectureStart.Hour.ToString());
                        text.Append(":");
                        text.Append(subject.LectureStart.Minute.ToString());
                        text.Append(" - ");
                        text.Append(subject.LectureFinish.Hour.ToString());
                        text.Append(":");
                        text.Append(subject.LectureFinish.Minute.ToString());
                        text.Append(")");
                        checkbox.Text = text.ToString();

                        // Autosize
                        checkbox.AutoSize = true;

                        // Location
                        checkbox.Left = 20;
                        checkbox.Top = top;
                        top += 30; // Increment top for the next checkbox

                        // Other properties
                        checkbox.Checked = true;
                        checkbox.TabIndex = 0;
                        checkbox.TextImageRelation = TextImageRelation.ImageBeforeText;
                        checkbox.UseVisualStyleBackColor = true;
                        checkbox.Visible = true;


                        // Only add a checkbox if all the textboxes haven't yet been made
                        if (dayPanel.Controls.Count != days[dayCounter].Subjects.Count)
                        {
                            dayPanel.Controls.Add(checkbox);
                        }
                        


                    }

                    // Show the panel
                    dayPanel.Visible = true;
                    break;
                }

                dayCounter--;
            }
            
        }




        // Left Side Menu functionality 
        private void menuRecord_Click(object sender, EventArgs e)
        {
            highlight.Top = menuSettings.Top;
        }

        private void menuAddSubjects_Click(object sender, EventArgs e)
        {
            highlight.Top = menuAddSubjects.Top;
        }

        private void menuRemoveSubjects_Click(object sender, EventArgs e)
        {
            highlight.Top = menuRemoveSubjects.Top;
        }

        private void menuEditSubjects_Click(object sender, EventArgs e)
        {
            highlight.Top = menuEditSubjects.Top;
        }


        // Responsiveness
        private void panelRecord_Resize(object sender, EventArgs e)
        {
            int size = panelSettings.Width / 6;
            foreach (Control radioButton in panelDays.Controls.OfType<RadioButton>())
            {
                radioButton.Width = size;
            }
        }

        // Day buttons functionality
        private void radioButtonMonday_Click(object sender, EventArgs e)
        {
            hidePreviousSchedule();
            displaySchedule(panelMonday);
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            hidePreviousSchedule();
            displaySchedule(panelTuesday);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            hidePreviousSchedule();
            displaySchedule(panelWednesday);
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            hidePreviousSchedule();
            displaySchedule(panelThursday);
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            hidePreviousSchedule();
            displaySchedule(panelFriday);
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            hidePreviousSchedule();
            displaySchedule(panelSaturday);
        }
    }
}
