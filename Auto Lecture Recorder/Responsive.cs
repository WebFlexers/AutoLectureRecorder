using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Lecture_Recorder
{
    public class Responsive
    {
        public Dictionary<string, Control> InitialControls { get; set; }
        public int InitialFormWidth { get; set; }
        public int InitialFormHeight { get; set; }

        // Width and height of the form
        int formWidth;
        int formHeight;

        

        public Responsive(Control.ControlCollection controls, int initialWidth, int initialHeight)
        {
            InitialControls = new Dictionary<string, Control>();

            foreach (Control control in controls)
                StoreControl(control);

            formWidth = initialWidth;
            InitialFormWidth = initialWidth;
            formHeight = initialHeight;
            InitialFormHeight = initialHeight;
            
        }

        public void StoreControl(Control control)
        {
            // Save all the control's position, size and name properties in a dictionary
            InitialControls.Add(control.Name, new Control(control.Name, control.Left, control.Top,
                                control.Width, control.Height));

            // If the control has a different font size than the default also store the size
            if (control.Font.Size != 8.25 && !(control is UserControl))
            {
                var fontFamily = control.Font.FontFamily;
                var fontStyle = control.Font.Style;
                var fontSize = control.Font.Size;
                InitialControls[control.Name].Font = new Font(fontFamily, fontSize, fontStyle);
            }

            // If the control is a panel get the inside controls as well
            if (control is Panel)
            {
                foreach (Control insideControl in control.Controls)
                {
                    StoreControl(insideControl);
                }
            }

        }

        public void UpdateFormSize(int width, int heigth)
        {
            formWidth = width;
            formHeight = heigth;
        }

        public void ScaleControl(Control control)
        {
            /* if the control is a panel and is docked there is no need to resize
            if (control is Panel && control.Dock != DockStyle.None)
            {
                // Scale the inner controls instead
                foreach (Control insideControl in control.Controls)
                {
                    ScaleControl(insideControl);
                }
            }
            */
            // Size
            // Get the size from the dictionary and convert it to percent size
            control.Width = InitialControls[control.Name].Width * formWidth / InitialFormWidth;
            control.Height = InitialControls[control.Name].Height * formHeight / InitialFormHeight;

            // Change the font accordingly
            if (control.Font != null && !(control is UserControl))
            {
                var fontFamily = InitialControls[control.Name].Font.FontFamily;
                var fontStyle = InitialControls[control.Name].Font.Style;
                control.Font = new Font(fontFamily, InitialControls[control.Name].Font.Size * formHeight / InitialFormHeight,
                                        fontStyle);
            }

            // if the control is a panel but not docked then it gets scaled too but also scales the inside controls
            if (control is Panel)
            {
                foreach (Control insideControl in control.Controls)
                {
                    ScaleControl(insideControl);
                }
            } 

            // Location
            control.Top = InitialControls[control.Name].Location.Y * formHeight / InitialFormHeight;
            control.Left = InitialControls[control.Name].Location.X * formWidth / InitialFormWidth;

        }

        public void ScaleAllControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ScaleControl(control);
            }
        }
    }
}
