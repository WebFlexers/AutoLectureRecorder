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
    public partial class DropdownList : UserControl
    {
        // Responsive object
        Responsive responsive;
        public DropdownList()
        {
            InitializeComponent();
            responsive = new Responsive(Controls, 199, 31);

        }

        private void dropdown_MouseEnter(object sender, EventArgs e)
        {
            labelDropdown.BackColor = Color.FromArgb(61, 67, 103);
            buttonDropdown.BackColor = Color.FromArgb(61, 67, 103);
        }

        private void dropdown_MouseLeave(object sender, EventArgs e)
        {
            labelDropdown.BackColor = Color.FromArgb(51, 56, 86);
            buttonDropdown.BackColor = Color.FromArgb(51, 56, 86);
        }

        private void DropdownList_Resize(object sender, EventArgs e)
        {
            // Resize the font dynamically
            float newSize = responsive.InitialControls["buttonDropdown"].Font.Size * this.Height / responsive.InitialFormHeight;
            buttonDropdown.Font = new Font("Marlett", newSize, FontStyle.Bold, GraphicsUnit.Point, 2);

            // Resize the menu items
            resizeMenu();
        }

        private void resizeMenu()
        {
            contextMenuStrip.Width = this.Width;
            contextMenuStrip.Height = contextMenuStrip.Items.Count * 26 + 4;

            foreach (ToolStripMenuItem item in contextMenuStrip.Items)
            {
                item.Width = this.Width;
            }
        }







        private void dropdown_Click(object sender, EventArgs e)
        {
            contextMenuStrip.Show(this, 0, this.Height);
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            labelDropdown.Text = e.ClickedItem.Text;
        }

        public string GetText()
        {
            return labelDropdown.Text;
        }

        public void AddDefaultOption(string option)
        {
            foreach (ToolStripMenuItem item in contextMenuStrip.Items)
            {
                if (item.Text.Equals(option))
                {
                    labelDropdown.Text = option;
                    return;
                }
            }

            AddOption(option);
            labelDropdown.Text = option;
        }

        public bool ContainsOption(string option)
        {
            foreach (ToolStripMenuItem item in contextMenuStrip.Items)
            {
                if (item.Text.Equals(option))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddOption(string option)
        {
            // Create menu option
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.AutoSize = false;
            item.Size = new Size(this.Width, 26);
            item.Text = option;

            // Add the option to the menu
            contextMenuStrip.Items.Add(item);

            // Resize the menu
            resizeMenu();
        }

        public void RemoveOption(string option)
        {
            ToolStripMenuItem toolStripMenuItem = null;
            foreach (ToolStripMenuItem item in contextMenuStrip.Items)
            {
                if (item.Name.Equals(option))
                {
                    toolStripMenuItem = item;
                }
            }

            contextMenuStrip.Items.Remove(toolStripMenuItem);

            // Resize the menu
            resizeMenu();
        }

        public void ClearOptions()
        {
            // Clear
            contextMenuStrip.Items.Clear();
            // Resize
            resizeMenu();
        }

        public void AddSelectionClickEvent(ToolStripItemClickedEventHandler clickEvent)
        {
            contextMenuStrip.ItemClicked += clickEvent;
        }
    }
}
