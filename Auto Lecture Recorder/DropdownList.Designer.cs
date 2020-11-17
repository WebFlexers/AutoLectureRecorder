namespace Auto_Lecture_Recorder
{
    partial class DropdownList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelDropdown = new System.Windows.Forms.Label();
            this.buttonDropdown = new System.Windows.Forms.Label();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // labelDropdown
            // 
            this.labelDropdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(86)))));
            this.labelDropdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDropdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelDropdown.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDropdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(220)))), ((int)(((byte)(236)))));
            this.labelDropdown.Location = new System.Drawing.Point(0, 0);
            this.labelDropdown.Margin = new System.Windows.Forms.Padding(0);
            this.labelDropdown.Name = "labelDropdown";
            this.labelDropdown.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.labelDropdown.Size = new System.Drawing.Size(180, 31);
            this.labelDropdown.TabIndex = 9;
            this.labelDropdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelDropdown.Click += new System.EventHandler(this.dropdown_Click);
            this.labelDropdown.MouseEnter += new System.EventHandler(this.dropdown_MouseEnter);
            this.labelDropdown.MouseLeave += new System.EventHandler(this.dropdown_MouseLeave);
            // 
            // buttonDropdown
            // 
            this.buttonDropdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(86)))));
            this.buttonDropdown.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonDropdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDropdown.Font = new System.Drawing.Font("Marlett", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonDropdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(220)))), ((int)(((byte)(236)))));
            this.buttonDropdown.Location = new System.Drawing.Point(180, 0);
            this.buttonDropdown.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDropdown.Name = "buttonDropdown";
            this.buttonDropdown.Size = new System.Drawing.Size(20, 31);
            this.buttonDropdown.TabIndex = 10;
            this.buttonDropdown.Text = "u";
            this.buttonDropdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonDropdown.Click += new System.EventHandler(this.dropdown_Click);
            this.buttonDropdown.MouseEnter += new System.EventHandler(this.dropdown_MouseEnter);
            this.buttonDropdown.MouseLeave += new System.EventHandler(this.dropdown_MouseLeave);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.AutoSize = false;
            this.contextMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(86)))));
            this.contextMenuStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.contextMenuStrip.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(220)))), ((int)(((byte)(236)))));
            this.contextMenuStrip.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.contextMenuStrip.MaximumSize = new System.Drawing.Size(0, 186);
            this.contextMenuStrip.Name = "menuStripDays";
            this.contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip.ShowImageMargin = false;
            this.contextMenuStrip.ShowItemToolTips = false;
            this.contextMenuStrip.Size = new System.Drawing.Size(0, 186);
            this.contextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_ItemClicked);
            // 
            // DropdownList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelDropdown);
            this.Controls.Add(this.buttonDropdown);
            this.Name = "DropdownList";
            this.Size = new System.Drawing.Size(200, 31);
            this.Resize += new System.EventHandler(this.DropdownList_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelDropdown;
        private System.Windows.Forms.Label buttonDropdown;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    }
}
