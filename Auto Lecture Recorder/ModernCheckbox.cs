using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Auto_Lecture_Recorder.Lectures;

namespace Auto_Lecture_Recorder
{
    public partial class ModernCheckbox : UserControl
    {
        public ModernCheckbox()
        {
            InitializeComponent();
        }

        public bool Checked { get; set; } = false;

        private void Component_MouseEnter(object sender, EventArgs e)
        {
            if (!Checked)
            {
                label.ForeColor = Color.FromArgb(250, 250, 250);
                checkbox.Image = Properties.Resources.checkbox_hover;
            }
                
        }

        private void Component_MouseLeave(object sender, EventArgs e)
        {
            if (!Checked)
            {
                label.ForeColor = Color.LightGray;
                checkbox.Image = Properties.Resources.checkbox;
            }
        }

        public void Component_Click(object sender, EventArgs e)
        {
            ToggleCheckbox();
        }
        
        public void Lecture_Enable_Click(object sender, EventArgs e, Lectures.Day day, Lecture currentLecture)
        {
            // Toggle checkbox
            ToggleCheckbox();
            currentLecture.Active = Checked;
            // Disable conflicting lectures
            if (Checked)
                day.DisableConflictingLectures(currentLecture);
            // Refresh the scheduled lectures
            var mainForm = (MainForm)Application.OpenForms[0];
            mainForm.RefreshScheduledRecordings(sender);
        } 

        public void ToggleCheckbox()
        {
            if (Checked)
            {
                Unckeck();
            }
            else
            {
                Check();
            }
        }

        public void Check()
        {
            Checked = true;
            label.ForeColor = Color.FromArgb(250, 250, 250);
            checkbox.Image = Properties.Resources.checkbox_clicked;
        }

        public void Unckeck()
        {
            Checked = false;
            label.ForeColor = Color.LightGray;
            checkbox.Image = Properties.Resources.checkbox;
        }

        public void AddClickEvents(EventHandler eventHandler)
        {
            label.Click += eventHandler;
            checkbox.Click += eventHandler;
            panelCheckmark.Click += eventHandler;
        }

        public void SetText(string text)
        {
            label.Text = text;
        }
    }
}
