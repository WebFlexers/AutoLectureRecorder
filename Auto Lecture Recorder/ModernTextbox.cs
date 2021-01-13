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
            
        }

        public string GetText()
        {
            return textBox.Text;
        }

        public void SetText(string text)
        {
            textBox.Text = text;
        }

        public void MakePasswordField()
        {
            textBox.PasswordChar = '*';
        }

        public void MakeNormalField()
        {
            textBox.PasswordChar = '\0';
        }

        public void AllowOnlyNumbers()
        {
            textBox.KeyPress += textBox_KeyPress;
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void ModernTextBox_Resize(object sender, EventArgs e)
        {
            // Update the font size
            responsive.ScaleControl(textBox);
        }

    }
}
