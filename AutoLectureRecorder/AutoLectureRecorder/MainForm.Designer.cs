namespace AutoLectureRecorder
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.menuEditSubjects = new System.Windows.Forms.Button();
            this.menuRemoveSubjects = new System.Windows.Forms.Button();
            this.menuAddSubjects = new System.Windows.Forms.Button();
            this.menuSettings = new System.Windows.Forms.Button();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.panelDaySubjects = new System.Windows.Forms.Panel();
            this.panelDays = new System.Windows.Forms.Panel();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButtonMonday = new System.Windows.Forms.RadioButton();
            this.panelHighlights = new System.Windows.Forms.Panel();
            this.highlight = new System.Windows.Forms.Panel();
            this.panelMonday = new System.Windows.Forms.Panel();
            this.panelTuesday = new System.Windows.Forms.Panel();
            this.panelWednesday = new System.Windows.Forms.Panel();
            this.panelThursday = new System.Windows.Forms.Panel();
            this.panelFriday = new System.Windows.Forms.Panel();
            this.panelSaturday = new System.Windows.Forms.Panel();
            this.panelMenu.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.panelDaySubjects.SuspendLayout();
            this.panelDays.SuspendLayout();
            this.panelHighlights.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(120)))), ((int)(((byte)(187)))));
            this.panelMenu.Controls.Add(this.menuEditSubjects);
            this.panelMenu.Controls.Add(this.menuRemoveSubjects);
            this.panelMenu.Controls.Add(this.menuAddSubjects);
            this.panelMenu.Controls.Add(this.menuSettings);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Margin = new System.Windows.Forms.Padding(0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(170, 450);
            this.panelMenu.TabIndex = 0;
            // 
            // menuEditSubjects
            // 
            this.menuEditSubjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuEditSubjects.FlatAppearance.BorderSize = 0;
            this.menuEditSubjects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menuEditSubjects.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuEditSubjects.ForeColor = System.Drawing.Color.White;
            this.menuEditSubjects.Image = global::AutoLectureRecorder.Properties.Resources.remove_book_30px;
            this.menuEditSubjects.Location = new System.Drawing.Point(0, 336);
            this.menuEditSubjects.Name = "menuEditSubjects";
            this.menuEditSubjects.Size = new System.Drawing.Size(170, 114);
            this.menuEditSubjects.TabIndex = 11;
            this.menuEditSubjects.Text = "Edit Subjects";
            this.menuEditSubjects.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.menuEditSubjects.UseVisualStyleBackColor = true;
            this.menuEditSubjects.Click += new System.EventHandler(this.menuEditSubjects_Click);
            // 
            // menuRemoveSubjects
            // 
            this.menuRemoveSubjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuRemoveSubjects.FlatAppearance.BorderSize = 0;
            this.menuRemoveSubjects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menuRemoveSubjects.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuRemoveSubjects.ForeColor = System.Drawing.Color.White;
            this.menuRemoveSubjects.Image = global::AutoLectureRecorder.Properties.Resources.remove_book_30px;
            this.menuRemoveSubjects.Location = new System.Drawing.Point(0, 224);
            this.menuRemoveSubjects.Name = "menuRemoveSubjects";
            this.menuRemoveSubjects.Size = new System.Drawing.Size(170, 112);
            this.menuRemoveSubjects.TabIndex = 10;
            this.menuRemoveSubjects.Text = "Remove Subjects";
            this.menuRemoveSubjects.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.menuRemoveSubjects.UseVisualStyleBackColor = true;
            this.menuRemoveSubjects.Click += new System.EventHandler(this.menuRemoveSubjects_Click);
            // 
            // menuAddSubjects
            // 
            this.menuAddSubjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuAddSubjects.FlatAppearance.BorderSize = 0;
            this.menuAddSubjects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menuAddSubjects.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuAddSubjects.ForeColor = System.Drawing.Color.White;
            this.menuAddSubjects.Image = global::AutoLectureRecorder.Properties.Resources.book_30px;
            this.menuAddSubjects.Location = new System.Drawing.Point(0, 112);
            this.menuAddSubjects.Name = "menuAddSubjects";
            this.menuAddSubjects.Size = new System.Drawing.Size(170, 112);
            this.menuAddSubjects.TabIndex = 9;
            this.menuAddSubjects.Text = "Add Subjects";
            this.menuAddSubjects.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.menuAddSubjects.UseVisualStyleBackColor = true;
            this.menuAddSubjects.Click += new System.EventHandler(this.menuAddSubjects_Click);
            // 
            // menuSettings
            // 
            this.menuSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuSettings.FlatAppearance.BorderSize = 0;
            this.menuSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menuSettings.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuSettings.ForeColor = System.Drawing.Color.White;
            this.menuSettings.Image = global::AutoLectureRecorder.Properties.Resources.settings_30px;
            this.menuSettings.Location = new System.Drawing.Point(0, 0);
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.Size = new System.Drawing.Size(170, 112);
            this.menuSettings.TabIndex = 8;
            this.menuSettings.Text = "Record Settings";
            this.menuSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.menuSettings.UseVisualStyleBackColor = true;
            this.menuSettings.Click += new System.EventHandler(this.menuRecord_Click);
            // 
            // panelSettings
            // 
            this.panelSettings.AutoScroll = true;
            this.panelSettings.BackColor = System.Drawing.Color.White;
            this.panelSettings.Controls.Add(this.panelDaySubjects);
            this.panelSettings.Controls.Add(this.panelDays);
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSettings.Location = new System.Drawing.Point(174, 0);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(626, 450);
            this.panelSettings.TabIndex = 1;
            this.panelSettings.Resize += new System.EventHandler(this.panelRecord_Resize);
            // 
            // panelDaySubjects
            // 
            this.panelDaySubjects.AutoScroll = true;
            this.panelDaySubjects.Controls.Add(this.panelSaturday);
            this.panelDaySubjects.Controls.Add(this.panelFriday);
            this.panelDaySubjects.Controls.Add(this.panelThursday);
            this.panelDaySubjects.Controls.Add(this.panelWednesday);
            this.panelDaySubjects.Controls.Add(this.panelTuesday);
            this.panelDaySubjects.Controls.Add(this.panelMonday);
            this.panelDaySubjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDaySubjects.Location = new System.Drawing.Point(0, 56);
            this.panelDaySubjects.Name = "panelDaySubjects";
            this.panelDaySubjects.Size = new System.Drawing.Size(626, 394);
            this.panelDaySubjects.TabIndex = 2;
            // 
            // panelDays
            // 
            this.panelDays.Controls.Add(this.radioButton5);
            this.panelDays.Controls.Add(this.radioButton4);
            this.panelDays.Controls.Add(this.radioButton3);
            this.panelDays.Controls.Add(this.radioButton2);
            this.panelDays.Controls.Add(this.radioButton1);
            this.panelDays.Controls.Add(this.radioButtonMonday);
            this.panelDays.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDays.Location = new System.Drawing.Point(0, 0);
            this.panelDays.Name = "panelDays";
            this.panelDays.Size = new System.Drawing.Size(626, 56);
            this.panelDays.TabIndex = 0;
            // 
            // radioButton5
            // 
            this.radioButton5.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(49)))), ((int)(((byte)(75)))));
            this.radioButton5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton5.FlatAppearance.BorderSize = 0;
            this.radioButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton5.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton5.Location = new System.Drawing.Point(520, 0);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(106, 56);
            this.radioButton5.TabIndex = 7;
            this.radioButton5.Text = "Saturday";
            this.radioButton5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton5.UseVisualStyleBackColor = false;
            this.radioButton5.Click += new System.EventHandler(this.radioButton5_Click);
            // 
            // radioButton4
            // 
            this.radioButton4.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(49)))), ((int)(((byte)(75)))));
            this.radioButton4.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButton4.FlatAppearance.BorderSize = 0;
            this.radioButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton4.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton4.Location = new System.Drawing.Point(416, 0);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(104, 56);
            this.radioButton4.TabIndex = 6;
            this.radioButton4.Text = "Friday";
            this.radioButton4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton4.UseVisualStyleBackColor = false;
            this.radioButton4.Click += new System.EventHandler(this.radioButton4_Click);
            // 
            // radioButton3
            // 
            this.radioButton3.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(49)))), ((int)(((byte)(75)))));
            this.radioButton3.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButton3.FlatAppearance.BorderSize = 0;
            this.radioButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton3.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton3.Location = new System.Drawing.Point(312, 0);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(104, 56);
            this.radioButton3.TabIndex = 5;
            this.radioButton3.Text = "Thursday";
            this.radioButton3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton3.UseVisualStyleBackColor = false;
            this.radioButton3.Click += new System.EventHandler(this.radioButton3_Click);
            // 
            // radioButton2
            // 
            this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(49)))), ((int)(((byte)(75)))));
            this.radioButton2.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButton2.FlatAppearance.BorderSize = 0;
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton2.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.Location = new System.Drawing.Point(208, 0);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(104, 56);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "Wednesday";
            this.radioButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton2.UseVisualStyleBackColor = false;
            this.radioButton2.Click += new System.EventHandler(this.radioButton2_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(49)))), ((int)(((byte)(75)))));
            this.radioButton1.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButton1.FlatAppearance.BorderSize = 0;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton1.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(104, 0);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(104, 56);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.Text = "Tuesday";
            this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.UseVisualStyleBackColor = false;
            this.radioButton1.Click += new System.EventHandler(this.radioButton1_Click);
            // 
            // radioButtonMonday
            // 
            this.radioButtonMonday.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonMonday.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(49)))), ((int)(((byte)(75)))));
            this.radioButtonMonday.Checked = true;
            this.radioButtonMonday.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButtonMonday.FlatAppearance.BorderSize = 0;
            this.radioButtonMonday.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonMonday.Font = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonMonday.Location = new System.Drawing.Point(0, 0);
            this.radioButtonMonday.Name = "radioButtonMonday";
            this.radioButtonMonday.Size = new System.Drawing.Size(104, 56);
            this.radioButtonMonday.TabIndex = 2;
            this.radioButtonMonday.TabStop = true;
            this.radioButtonMonday.Text = "Monday";
            this.radioButtonMonday.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonMonday.UseVisualStyleBackColor = false;
            this.radioButtonMonday.Click += new System.EventHandler(this.radioButtonMonday_Click);
            // 
            // panelHighlights
            // 
            this.panelHighlights.AutoSize = true;
            this.panelHighlights.BackColor = System.Drawing.Color.White;
            this.panelHighlights.Controls.Add(this.highlight);
            this.panelHighlights.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelHighlights.Location = new System.Drawing.Point(170, 0);
            this.panelHighlights.Name = "panelHighlights";
            this.panelHighlights.Size = new System.Drawing.Size(4, 450);
            this.panelHighlights.TabIndex = 2;
            // 
            // highlight
            // 
            this.highlight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(164)))), ((int)(((byte)(58)))));
            this.highlight.Location = new System.Drawing.Point(0, 0);
            this.highlight.Margin = new System.Windows.Forms.Padding(0);
            this.highlight.Name = "highlight";
            this.highlight.Size = new System.Drawing.Size(4, 112);
            this.highlight.TabIndex = 1;
            // 
            // panelMonday
            // 
            this.panelMonday.AutoSize = true;
            this.panelMonday.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMonday.Location = new System.Drawing.Point(0, 0);
            this.panelMonday.Name = "panelMonday";
            this.panelMonday.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panelMonday.Size = new System.Drawing.Size(626, 40);
            this.panelMonday.TabIndex = 0;
            // 
            // panelTuesday
            // 
            this.panelTuesday.AutoSize = true;
            this.panelTuesday.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTuesday.Location = new System.Drawing.Point(0, 40);
            this.panelTuesday.Name = "panelTuesday";
            this.panelTuesday.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panelTuesday.Size = new System.Drawing.Size(626, 40);
            this.panelTuesday.TabIndex = 1;
            // 
            // panelWednesday
            // 
            this.panelWednesday.AutoSize = true;
            this.panelWednesday.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelWednesday.Location = new System.Drawing.Point(0, 80);
            this.panelWednesday.Name = "panelWednesday";
            this.panelWednesday.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panelWednesday.Size = new System.Drawing.Size(626, 40);
            this.panelWednesday.TabIndex = 2;
            // 
            // panelThursday
            // 
            this.panelThursday.AutoSize = true;
            this.panelThursday.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelThursday.Location = new System.Drawing.Point(0, 120);
            this.panelThursday.Name = "panelThursday";
            this.panelThursday.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panelThursday.Size = new System.Drawing.Size(626, 40);
            this.panelThursday.TabIndex = 3;
            // 
            // panelFriday
            // 
            this.panelFriday.AutoSize = true;
            this.panelFriday.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFriday.Location = new System.Drawing.Point(0, 160);
            this.panelFriday.Name = "panelFriday";
            this.panelFriday.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panelFriday.Size = new System.Drawing.Size(626, 40);
            this.panelFriday.TabIndex = 4;
            // 
            // panelSaturday
            // 
            this.panelSaturday.AutoSize = true;
            this.panelSaturday.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSaturday.Location = new System.Drawing.Point(0, 200);
            this.panelSaturday.Name = "panelSaturday";
            this.panelSaturday.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panelSaturday.Size = new System.Drawing.Size(626, 40);
            this.panelSaturday.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelSettings);
            this.Controls.Add(this.panelHighlights);
            this.Controls.Add(this.panelMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "AutoLectureRecorder";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelMenu.ResumeLayout(false);
            this.panelSettings.ResumeLayout(false);
            this.panelDaySubjects.ResumeLayout(false);
            this.panelDaySubjects.PerformLayout();
            this.panelDays.ResumeLayout(false);
            this.panelHighlights.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button menuEditSubjects;
        private System.Windows.Forms.Button menuRemoveSubjects;
        private System.Windows.Forms.Button menuAddSubjects;
        private System.Windows.Forms.Button menuSettings;
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.Panel panelHighlights;
        private System.Windows.Forms.Panel highlight;
        private System.Windows.Forms.Panel panelDays;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButtonMonday;
        private System.Windows.Forms.Panel panelDaySubjects;
        private System.Windows.Forms.Panel panelSaturday;
        private System.Windows.Forms.Panel panelFriday;
        private System.Windows.Forms.Panel panelThursday;
        private System.Windows.Forms.Panel panelWednesday;
        private System.Windows.Forms.Panel panelTuesday;
        private System.Windows.Forms.Panel panelMonday;
    }
}

