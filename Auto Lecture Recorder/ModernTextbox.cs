using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Lecture_Recorder
{
    public partial class ModernTextBox : UserControl
    {
        Responsive responsive;
        public ModernTextBox()
        {
            InitializeComponent();
            responsive = new Responsive(Controls, 269, 30);
        }

        private void ModernTextBox_Load(object sender, EventArgs e)
        {
            richTextBox.SelectionAlignment = HorizontalAlignment.Center;
        }

        public string GetText()
        {
            return richTextBox.Text;
        }

        public void SetText(string text)
        {
            richTextBox.Text = text;
        }

        private void ModernTextBox_Resize(object sender, EventArgs e)
        {
            // Update the font size
            float newSize = responsive.InitialControls["richTextBox"].Font.Size * this.Height / responsive.InitialFormHeight;
            richTextBox.Font = new Font(richTextBox.Font.FontFamily, newSize, FontStyle.Regular, GraphicsUnit.Point, 2);
        }
    }
}
