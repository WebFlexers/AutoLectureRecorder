using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Lecture_Recorder.Lectures
{
    [Serializable]
    public class Lecture
    {
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Platform { get; set; }
        public bool Active { get; set; }

        [field: NonSerialized]
        public ModernCheckbox CheckBox { get; set; }

        public Lecture(string name, TimeSpan startTime, TimeSpan endTime, string platform)
        {
            // Set name
            Name = name;

            // Sets the lecture start time
            StartTime = startTime;
            // Sets the licture finish time
            EndTime = endTime;

            // Set platform
            Platform = platform;

            // Set active upon creation
            Active = true;
        }

        public ModernCheckbox CreateCheckbox(Responsive responsive, ref int controlsCount)
        {
            ModernCheckbox modernCheckbox = new ModernCheckbox();
            modernCheckbox.BackColor = Color.FromArgb(35, 39, 66);
            modernCheckbox.Dock = DockStyle.Top;
            modernCheckbox.Location = new Point(0, 0);
            modernCheckbox.Name = "modernCheckbox" + controlsCount;
            modernCheckbox.Size = new Size(220, 40);
            modernCheckbox.TabIndex = 15;

            // Initiate state
            modernCheckbox.SetText("Enabled");
            if (Active)
            {
                modernCheckbox.ToggleCheckbox();
            }

            // Store the control in responsive object
            if (!responsive.InitialControls.ContainsKey(modernCheckbox.Name))
                responsive.StoreControl(modernCheckbox);

            // Increment the control count
            controlsCount++;

            // Save the checkbox in the lecture object
            CheckBox = modernCheckbox;

            return modernCheckbox;
        }

        // Checks if the end time is bigger than the start time
        public static bool timeIsValid(TimeSpan startTime, TimeSpan endTime)
        {
            return endTime > startTime;
        }

        
    }
}
