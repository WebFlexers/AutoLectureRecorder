namespace Auto_Lecture_Recorder
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
            this.panelBorderRight = new System.Windows.Forms.Panel();
            this.panelBorderLeft = new System.Windows.Forms.Panel();
            this.panelBorderTop = new System.Windows.Forms.Panel();
            this.panelBorderBottom = new System.Windows.Forms.Panel();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.radioMenuSettings = new System.Windows.Forms.RadioButton();
            this.radioMenuAdd = new System.Windows.Forms.RadioButton();
            this.radioMenuSubjects = new System.Windows.Forms.RadioButton();
            this.radioMenuRecord = new System.Windows.Forms.RadioButton();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelMenuBorder = new System.Windows.Forms.Panel();
            this.panelTitleBar = new System.Windows.Forms.Panel();
            this.buttonMinimize = new System.Windows.Forms.Button();
            this.buttonMaximize = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.panelRecord = new System.Windows.Forms.Panel();
            this.panelRecordArmed = new System.Windows.Forms.Panel();
            this.textBoxRecordInfo = new System.Windows.Forms.TextBox();
            this.labelRecordTimer = new System.Windows.Forms.Label();
            this.panelRecordContent = new System.Windows.Forms.Panel();
            this.checkBoxRecordButton = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelSubjects = new System.Windows.Forms.Panel();
            this.panelAddSubjects = new System.Windows.Forms.Panel();
            this.panelAddButton = new System.Windows.Forms.Panel();
            this.buttonAddSubject = new System.Windows.Forms.Button();
            this.panelEndTime = new System.Windows.Forms.Panel();
            this.dropdownEndMin = new Auto_Lecture_Recorder.DropdownList();
            this.dropdownEndHour = new Auto_Lecture_Recorder.DropdownList();
            this.label9 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.panelStartTime = new System.Windows.Forms.Panel();
            this.dropdownStartMin = new Auto_Lecture_Recorder.DropdownList();
            this.dropdownStartHour = new Auto_Lecture_Recorder.DropdownList();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelSubjectDay = new System.Windows.Forms.Panel();
            this.dropdownDay = new Auto_Lecture_Recorder.DropdownList();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSubjectName = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxSubjectName = new System.Windows.Forms.TextBox();
            this.labelSubjectName = new System.Windows.Forms.Label();
            this.panelAddSubjectLabel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.panelMainWindows = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panelMenu.SuspendLayout();
            this.panelTitleBar.SuspendLayout();
            this.panelRecord.SuspendLayout();
            this.panelRecordArmed.SuspendLayout();
            this.panelRecordContent.SuspendLayout();
            this.panelAddSubjects.SuspendLayout();
            this.panelAddButton.SuspendLayout();
            this.panelEndTime.SuspendLayout();
            this.panelStartTime.SuspendLayout();
            this.panelSubjectDay.SuspendLayout();
            this.panelSubjectName.SuspendLayout();
            this.panelAddSubjectLabel.SuspendLayout();
            this.panelMainWindows.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBorderRight
            // 
            this.panelBorderRight.BackColor = System.Drawing.Color.Black;
            this.panelBorderRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelBorderRight.Location = new System.Drawing.Point(949, 0);
            this.panelBorderRight.Name = "panelBorderRight";
            this.panelBorderRight.Size = new System.Drawing.Size(1, 542);
            this.panelBorderRight.TabIndex = 0;
            // 
            // panelBorderLeft
            // 
            this.panelBorderLeft.BackColor = System.Drawing.Color.Black;
            this.panelBorderLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelBorderLeft.Location = new System.Drawing.Point(0, 0);
            this.panelBorderLeft.Name = "panelBorderLeft";
            this.panelBorderLeft.Size = new System.Drawing.Size(1, 542);
            this.panelBorderLeft.TabIndex = 1;
            // 
            // panelBorderTop
            // 
            this.panelBorderTop.BackColor = System.Drawing.Color.Black;
            this.panelBorderTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBorderTop.Location = new System.Drawing.Point(1, 0);
            this.panelBorderTop.Name = "panelBorderTop";
            this.panelBorderTop.Size = new System.Drawing.Size(948, 1);
            this.panelBorderTop.TabIndex = 2;
            // 
            // panelBorderBottom
            // 
            this.panelBorderBottom.BackColor = System.Drawing.Color.Black;
            this.panelBorderBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBorderBottom.Location = new System.Drawing.Point(1, 541);
            this.panelBorderBottom.Name = "panelBorderBottom";
            this.panelBorderBottom.Size = new System.Drawing.Size(948, 1);
            this.panelBorderBottom.TabIndex = 3;
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(86)))));
            this.panelMenu.Controls.Add(this.radioMenuSettings);
            this.panelMenu.Controls.Add(this.radioMenuAdd);
            this.panelMenu.Controls.Add(this.radioMenuSubjects);
            this.panelMenu.Controls.Add(this.radioMenuRecord);
            this.panelMenu.Controls.Add(this.labelTitle);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(1, 1);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(161, 540);
            this.panelMenu.TabIndex = 4;
            this.panelMenu.Resize += new System.EventHandler(this.panelMenu_Resize);
            // 
            // radioMenuSettings
            // 
            this.radioMenuSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioMenuSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioMenuSettings.FlatAppearance.BorderSize = 0;
            this.radioMenuSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioMenuSettings.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioMenuSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(220)))), ((int)(((byte)(236)))));
            this.radioMenuSettings.Image = global::Auto_Lecture_Recorder.Properties.Resources.settings_white_35px;
            this.radioMenuSettings.Location = new System.Drawing.Point(0, 412);
            this.radioMenuSettings.Name = "radioMenuSettings";
            this.radioMenuSettings.Size = new System.Drawing.Size(161, 128);
            this.radioMenuSettings.TabIndex = 6;
            this.radioMenuSettings.Text = "Settings";
            this.radioMenuSettings.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.radioMenuSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.radioMenuSettings.UseVisualStyleBackColor = true;
            this.radioMenuSettings.CheckedChanged += new System.EventHandler(this.radioMenuSettings_CheckedChanged);
            this.radioMenuSettings.Click += new System.EventHandler(this.radioMenuSettings_Click);
            this.radioMenuSettings.MouseEnter += new System.EventHandler(this.radioMenuSettings_MouseEnter);
            this.radioMenuSettings.MouseLeave += new System.EventHandler(this.radioMenuSettings_MouseLeave);
            // 
            // radioMenuAdd
            // 
            this.radioMenuAdd.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioMenuAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioMenuAdd.FlatAppearance.BorderSize = 0;
            this.radioMenuAdd.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioMenuAdd.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioMenuAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(220)))), ((int)(((byte)(236)))));
            this.radioMenuAdd.Image = global::Auto_Lecture_Recorder.Properties.Resources.add_book_white_35px;
            this.radioMenuAdd.Location = new System.Drawing.Point(0, 286);
            this.radioMenuAdd.Name = "radioMenuAdd";
            this.radioMenuAdd.Size = new System.Drawing.Size(161, 126);
            this.radioMenuAdd.TabIndex = 5;
            this.radioMenuAdd.Text = "Add Lectures";
            this.radioMenuAdd.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.radioMenuAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.radioMenuAdd.UseVisualStyleBackColor = true;
            this.radioMenuAdd.CheckedChanged += new System.EventHandler(this.radioMenuAdd_CheckedChanged);
            this.radioMenuAdd.Click += new System.EventHandler(this.radioMenuAdd_Click);
            this.radioMenuAdd.MouseEnter += new System.EventHandler(this.radioMenuAdd_MouseEnter);
            this.radioMenuAdd.MouseLeave += new System.EventHandler(this.radioMenuAdd_MouseLeave);
            // 
            // radioMenuSubjects
            // 
            this.radioMenuSubjects.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioMenuSubjects.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioMenuSubjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioMenuSubjects.FlatAppearance.BorderSize = 0;
            this.radioMenuSubjects.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuSubjects.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuSubjects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioMenuSubjects.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioMenuSubjects.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(220)))), ((int)(((byte)(236)))));
            this.radioMenuSubjects.Image = global::Auto_Lecture_Recorder.Properties.Resources.book_white_35px;
            this.radioMenuSubjects.Location = new System.Drawing.Point(0, 160);
            this.radioMenuSubjects.Name = "radioMenuSubjects";
            this.radioMenuSubjects.Size = new System.Drawing.Size(161, 126);
            this.radioMenuSubjects.TabIndex = 4;
            this.radioMenuSubjects.Text = "Lectures";
            this.radioMenuSubjects.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.radioMenuSubjects.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.radioMenuSubjects.UseVisualStyleBackColor = true;
            this.radioMenuSubjects.CheckedChanged += new System.EventHandler(this.radioMenuSubjects_CheckedChanged);
            this.radioMenuSubjects.Click += new System.EventHandler(this.radioMenuSubjects_Click);
            this.radioMenuSubjects.MouseEnter += new System.EventHandler(this.radioMenuSubjects_MouseEnter);
            this.radioMenuSubjects.MouseLeave += new System.EventHandler(this.radioMenuSubjects_MouseLeave);
            // 
            // radioMenuRecord
            // 
            this.radioMenuRecord.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioMenuRecord.Checked = true;
            this.radioMenuRecord.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioMenuRecord.FlatAppearance.BorderSize = 0;
            this.radioMenuRecord.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuRecord.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(115)))));
            this.radioMenuRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioMenuRecord.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioMenuRecord.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(220)))), ((int)(((byte)(236)))));
            this.radioMenuRecord.Image = global::Auto_Lecture_Recorder.Properties.Resources.video_call_blue_35px;
            this.radioMenuRecord.Location = new System.Drawing.Point(0, 34);
            this.radioMenuRecord.Name = "radioMenuRecord";
            this.radioMenuRecord.Size = new System.Drawing.Size(161, 126);
            this.radioMenuRecord.TabIndex = 3;
            this.radioMenuRecord.TabStop = true;
            this.radioMenuRecord.Text = "Record";
            this.radioMenuRecord.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.radioMenuRecord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.radioMenuRecord.UseVisualStyleBackColor = true;
            this.radioMenuRecord.CheckedChanged += new System.EventHandler(this.radioMenuRecord_CheckedChanged);
            this.radioMenuRecord.Click += new System.EventHandler(this.radioMenuRecord_Click);
            this.radioMenuRecord.MouseEnter += new System.EventHandler(this.radioMenuRecord_MouseEnter);
            this.radioMenuRecord.MouseLeave += new System.EventHandler(this.radioMenuRecord_MouseLeave);
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelTitle.Font = new System.Drawing.Font("Century", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(203)))), ((int)(((byte)(227)))));
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(161, 34);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Auto Lecture Recorder";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelMenuBorder
            // 
            this.panelMenuBorder.BackColor = System.Drawing.Color.Black;
            this.panelMenuBorder.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuBorder.Location = new System.Drawing.Point(162, 1);
            this.panelMenuBorder.Name = "panelMenuBorder";
            this.panelMenuBorder.Size = new System.Drawing.Size(1, 540);
            this.panelMenuBorder.TabIndex = 5;
            // 
            // panelTitleBar
            // 
            this.panelTitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.panelTitleBar.Controls.Add(this.buttonMinimize);
            this.panelTitleBar.Controls.Add(this.buttonMaximize);
            this.panelTitleBar.Controls.Add(this.buttonExit);
            this.panelTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitleBar.Location = new System.Drawing.Point(163, 1);
            this.panelTitleBar.Name = "panelTitleBar";
            this.panelTitleBar.Size = new System.Drawing.Size(786, 34);
            this.panelTitleBar.TabIndex = 6;
            this.panelTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseMove);
            // 
            // buttonMinimize
            // 
            this.buttonMinimize.BackgroundImage = global::Auto_Lecture_Recorder.Properties.Resources.minus_20px;
            this.buttonMinimize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonMinimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonMinimize.FlatAppearance.BorderSize = 0;
            this.buttonMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMinimize.Location = new System.Drawing.Point(684, 0);
            this.buttonMinimize.Name = "buttonMinimize";
            this.buttonMinimize.Size = new System.Drawing.Size(34, 34);
            this.buttonMinimize.TabIndex = 2;
            this.buttonMinimize.UseVisualStyleBackColor = true;
            this.buttonMinimize.Click += new System.EventHandler(this.buttonMinimize_Click);
            // 
            // buttonMaximize
            // 
            this.buttonMaximize.BackgroundImage = global::Auto_Lecture_Recorder.Properties.Resources.maximize_button_16px;
            this.buttonMaximize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonMaximize.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonMaximize.FlatAppearance.BorderSize = 0;
            this.buttonMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMaximize.Location = new System.Drawing.Point(718, 0);
            this.buttonMaximize.Name = "buttonMaximize";
            this.buttonMaximize.Size = new System.Drawing.Size(34, 34);
            this.buttonMaximize.TabIndex = 1;
            this.buttonMaximize.UseVisualStyleBackColor = true;
            this.buttonMaximize.Click += new System.EventHandler(this.buttonMaximize_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.BackgroundImage = global::Auto_Lecture_Recorder.Properties.Resources.x_24px;
            this.buttonExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonExit.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Location = new System.Drawing.Point(752, 0);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(34, 34);
            this.buttonExit.TabIndex = 0;
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // panelRecord
            // 
            this.panelRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.panelRecord.Controls.Add(this.button1);
            this.panelRecord.Controls.Add(this.panelRecordArmed);
            this.panelRecord.Controls.Add(this.panelRecordContent);
            this.panelRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRecord.Location = new System.Drawing.Point(0, 0);
            this.panelRecord.Name = "panelRecord";
            this.panelRecord.Size = new System.Drawing.Size(786, 506);
            this.panelRecord.TabIndex = 7;
            // 
            // panelRecordArmed
            // 
            this.panelRecordArmed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelRecordArmed.Controls.Add(this.textBoxRecordInfo);
            this.panelRecordArmed.Controls.Add(this.labelRecordTimer);
            this.panelRecordArmed.Location = new System.Drawing.Point(202, 332);
            this.panelRecordArmed.Margin = new System.Windows.Forms.Padding(3, 3, 3, 50);
            this.panelRecordArmed.Name = "panelRecordArmed";
            this.panelRecordArmed.Size = new System.Drawing.Size(383, 150);
            this.panelRecordArmed.TabIndex = 4;
            this.panelRecordArmed.Visible = false;
            // 
            // textBoxRecordInfo
            // 
            this.textBoxRecordInfo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textBoxRecordInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.textBoxRecordInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxRecordInfo.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRecordInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(203)))), ((int)(((byte)(227)))));
            this.textBoxRecordInfo.Location = new System.Drawing.Point(10, 32);
            this.textBoxRecordInfo.Multiline = true;
            this.textBoxRecordInfo.Name = "textBoxRecordInfo";
            this.textBoxRecordInfo.ReadOnly = true;
            this.textBoxRecordInfo.Size = new System.Drawing.Size(362, 68);
            this.textBoxRecordInfo.TabIndex = 2;
            this.textBoxRecordInfo.Text = "The next lecture to be recorded is:\r\nObject Oriented Programming\r\nThe recording w" +
    "ill start in:";
            this.textBoxRecordInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelRecordTimer
            // 
            this.labelRecordTimer.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelRecordTimer.AutoSize = true;
            this.labelRecordTimer.Font = new System.Drawing.Font("Century", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRecordTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(203)))), ((int)(((byte)(227)))));
            this.labelRecordTimer.Location = new System.Drawing.Point(139, 103);
            this.labelRecordTimer.Name = "labelRecordTimer";
            this.labelRecordTimer.Size = new System.Drawing.Size(104, 25);
            this.labelRecordTimer.TabIndex = 3;
            this.labelRecordTimer.Text = "08:20:00";
            // 
            // panelRecordContent
            // 
            this.panelRecordContent.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelRecordContent.AutoSize = true;
            this.panelRecordContent.Controls.Add(this.checkBoxRecordButton);
            this.panelRecordContent.Controls.Add(this.label2);
            this.panelRecordContent.Location = new System.Drawing.Point(122, 15);
            this.panelRecordContent.Name = "panelRecordContent";
            this.panelRecordContent.Padding = new System.Windows.Forms.Padding(10);
            this.panelRecordContent.Size = new System.Drawing.Size(542, 311);
            this.panelRecordContent.TabIndex = 4;
            // 
            // checkBoxRecordButton
            // 
            this.checkBoxRecordButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkBoxRecordButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxRecordButton.BackgroundImage = global::Auto_Lecture_Recorder.Properties.Resources.record_button_off;
            this.checkBoxRecordButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.checkBoxRecordButton.FlatAppearance.BorderSize = 0;
            this.checkBoxRecordButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBoxRecordButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.checkBoxRecordButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.checkBoxRecordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxRecordButton.Location = new System.Drawing.Point(152, 63);
            this.checkBoxRecordButton.Name = "checkBoxRecordButton";
            this.checkBoxRecordButton.Size = new System.Drawing.Size(239, 235);
            this.checkBoxRecordButton.TabIndex = 0;
            this.checkBoxRecordButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxRecordButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.checkBoxRecordButton.UseVisualStyleBackColor = true;
            this.checkBoxRecordButton.CheckedChanged += new System.EventHandler(this.checkBoxRecordButton_CheckedChanged);
            this.checkBoxRecordButton.MouseEnter += new System.EventHandler(this.checkBoxRecordButton_MouseEnter);
            this.checkBoxRecordButton.MouseLeave += new System.EventHandler(this.checkBoxRecordButton_MouseLeave);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(203)))), ((int)(((byte)(227)))));
            this.label2.Location = new System.Drawing.Point(38, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(466, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Click the button to schedule the recording";
            // 
            // panelSubjects
            // 
            this.panelSubjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.panelSubjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSubjects.Location = new System.Drawing.Point(0, 0);
            this.panelSubjects.Name = "panelSubjects";
            this.panelSubjects.Size = new System.Drawing.Size(786, 506);
            this.panelSubjects.TabIndex = 5;
            // 
            // panelAddSubjects
            // 
            this.panelAddSubjects.AutoScroll = true;
            this.panelAddSubjects.AutoSize = true;
            this.panelAddSubjects.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelAddSubjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.panelAddSubjects.Controls.Add(this.panelAddButton);
            this.panelAddSubjects.Controls.Add(this.panelEndTime);
            this.panelAddSubjects.Controls.Add(this.panelStartTime);
            this.panelAddSubjects.Controls.Add(this.panelSubjectDay);
            this.panelAddSubjects.Controls.Add(this.panelSubjectName);
            this.panelAddSubjects.Controls.Add(this.panelAddSubjectLabel);
            this.panelAddSubjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAddSubjects.Location = new System.Drawing.Point(0, 0);
            this.panelAddSubjects.Name = "panelAddSubjects";
            this.panelAddSubjects.Size = new System.Drawing.Size(786, 506);
            this.panelAddSubjects.TabIndex = 6;
            // 
            // panelAddButton
            // 
            this.panelAddButton.AutoSize = true;
            this.panelAddButton.Controls.Add(this.buttonAddSubject);
            this.panelAddButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAddButton.Location = new System.Drawing.Point(0, 558);
            this.panelAddButton.Name = "panelAddButton";
            this.panelAddButton.Padding = new System.Windows.Forms.Padding(0, 5, 0, 30);
            this.panelAddButton.Size = new System.Drawing.Size(769, 99);
            this.panelAddButton.TabIndex = 7;
            // 
            // buttonAddSubject
            // 
            this.buttonAddSubject.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAddSubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(86)))));
            this.buttonAddSubject.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAddSubject.FlatAppearance.BorderSize = 0;
            this.buttonAddSubject.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(56)))), ((int)(((byte)(86)))));
            this.buttonAddSubject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddSubject.Font = new System.Drawing.Font("Century", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddSubject.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(147)))), ((int)(((byte)(41)))));
            this.buttonAddSubject.Location = new System.Drawing.Point(170, 17);
            this.buttonAddSubject.Name = "buttonAddSubject";
            this.buttonAddSubject.Size = new System.Drawing.Size(416, 49);
            this.buttonAddSubject.TabIndex = 0;
            this.buttonAddSubject.Text = "Add subject";
            this.buttonAddSubject.UseVisualStyleBackColor = false;
            this.buttonAddSubject.Click += new System.EventHandler(this.buttonAddSubject_Click);
            // 
            // panelEndTime
            // 
            this.panelEndTime.AutoSize = true;
            this.panelEndTime.Controls.Add(this.dropdownEndMin);
            this.panelEndTime.Controls.Add(this.dropdownEndHour);
            this.panelEndTime.Controls.Add(this.label9);
            this.panelEndTime.Controls.Add(this.label12);
            this.panelEndTime.Controls.Add(this.label24);
            this.panelEndTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEndTime.Location = new System.Drawing.Point(0, 418);
            this.panelEndTime.Name = "panelEndTime";
            this.panelEndTime.Padding = new System.Windows.Forms.Padding(0, 5, 0, 30);
            this.panelEndTime.Size = new System.Drawing.Size(769, 140);
            this.panelEndTime.TabIndex = 6;
            // 
            // dropdownEndMin
            // 
            this.dropdownEndMin.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dropdownEndMin.Location = new System.Drawing.Point(389, 76);
            this.dropdownEndMin.Name = "dropdownEndMin";
            this.dropdownEndMin.Size = new System.Drawing.Size(60, 31);
            this.dropdownEndMin.TabIndex = 24;
            this.dropdownEndMin.Load += new System.EventHandler(this.dropdownStartMin_Load);
            // 
            // dropdownEndHour
            // 
            this.dropdownEndHour.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dropdownEndHour.Location = new System.Drawing.Point(293, 76);
            this.dropdownEndHour.Name = "dropdownEndHour";
            this.dropdownEndHour.Size = new System.Drawing.Size(60, 31);
            this.dropdownEndHour.TabIndex = 23;
            this.dropdownEndHour.Load += new System.EventHandler(this.dropdownStartHour_Load);
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(123)))), ((int)(((byte)(245)))));
            this.label9.Location = new System.Drawing.Point(378, 49);
            this.label9.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 24);
            this.label9.TabIndex = 22;
            this.label9.Text = "Minutes";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(123)))), ((int)(((byte)(245)))));
            this.label12.Location = new System.Drawing.Point(289, 49);
            this.label12.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 24);
            this.label12.TabIndex = 20;
            this.label12.Text = "Hours";
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(147)))), ((int)(((byte)(41)))));
            this.label24.Location = new System.Drawing.Point(284, 5);
            this.label24.Margin = new System.Windows.Forms.Padding(0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(184, 24);
            this.label24.TabIndex = 2;
            this.label24.Text = "Lecture End Time";
            // 
            // panelStartTime
            // 
            this.panelStartTime.AutoSize = true;
            this.panelStartTime.Controls.Add(this.dropdownStartMin);
            this.panelStartTime.Controls.Add(this.dropdownStartHour);
            this.panelStartTime.Controls.Add(this.label16);
            this.panelStartTime.Controls.Add(this.label19);
            this.panelStartTime.Controls.Add(this.label5);
            this.panelStartTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStartTime.Location = new System.Drawing.Point(0, 273);
            this.panelStartTime.Name = "panelStartTime";
            this.panelStartTime.Padding = new System.Windows.Forms.Padding(0, 5, 0, 30);
            this.panelStartTime.Size = new System.Drawing.Size(769, 145);
            this.panelStartTime.TabIndex = 5;
            // 
            // dropdownStartMin
            // 
            this.dropdownStartMin.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dropdownStartMin.Location = new System.Drawing.Point(389, 81);
            this.dropdownStartMin.Name = "dropdownStartMin";
            this.dropdownStartMin.Size = new System.Drawing.Size(60, 31);
            this.dropdownStartMin.TabIndex = 18;
            this.dropdownStartMin.Load += new System.EventHandler(this.dropdownStartMin_Load);
            // 
            // dropdownStartHour
            // 
            this.dropdownStartHour.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dropdownStartHour.Location = new System.Drawing.Point(293, 81);
            this.dropdownStartHour.Name = "dropdownStartHour";
            this.dropdownStartHour.Size = new System.Drawing.Size(60, 31);
            this.dropdownStartHour.TabIndex = 17;
            this.dropdownStartHour.Load += new System.EventHandler(this.dropdownStartHour_Load);
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(123)))), ((int)(((byte)(245)))));
            this.label16.Location = new System.Drawing.Point(376, 49);
            this.label16.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(87, 24);
            this.label16.TabIndex = 16;
            this.label16.Text = "Minutes";
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(123)))), ((int)(((byte)(245)))));
            this.label19.Location = new System.Drawing.Point(289, 49);
            this.label19.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(65, 24);
            this.label19.TabIndex = 14;
            this.label19.Text = "Hours";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(147)))), ((int)(((byte)(41)))));
            this.label5.Location = new System.Drawing.Point(281, 5);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(190, 24);
            this.label5.TabIndex = 2;
            this.label5.Text = "Lecture Start Time";
            // 
            // panelSubjectDay
            // 
            this.panelSubjectDay.AutoSize = true;
            this.panelSubjectDay.Controls.Add(this.dropdownDay);
            this.panelSubjectDay.Controls.Add(this.label4);
            this.panelSubjectDay.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSubjectDay.Location = new System.Drawing.Point(0, 164);
            this.panelSubjectDay.Name = "panelSubjectDay";
            this.panelSubjectDay.Padding = new System.Windows.Forms.Padding(0, 5, 0, 30);
            this.panelSubjectDay.Size = new System.Drawing.Size(769, 109);
            this.panelSubjectDay.TabIndex = 4;
            // 
            // dropdownDay
            // 
            this.dropdownDay.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dropdownDay.Location = new System.Drawing.Point(170, 45);
            this.dropdownDay.Name = "dropdownDay";
            this.dropdownDay.Size = new System.Drawing.Size(416, 31);
            this.dropdownDay.TabIndex = 7;
            this.dropdownDay.Load += new System.EventHandler(this.dropdownDay_Load);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(147)))), ((int)(((byte)(41)))));
            this.label4.Location = new System.Drawing.Point(310, 5);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 24);
            this.label4.TabIndex = 1;
            this.label4.Text = "Lecture day";
            // 
            // panelSubjectName
            // 
            this.panelSubjectName.AutoSize = true;
            this.panelSubjectName.Controls.Add(this.panel2);
            this.panelSubjectName.Controls.Add(this.textBoxSubjectName);
            this.panelSubjectName.Controls.Add(this.labelSubjectName);
            this.panelSubjectName.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSubjectName.Location = new System.Drawing.Point(0, 71);
            this.panelSubjectName.Name = "panelSubjectName";
            this.panelSubjectName.Padding = new System.Windows.Forms.Padding(0, 5, 0, 30);
            this.panelSubjectName.Size = new System.Drawing.Size(769, 93);
            this.panelSubjectName.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(123)))), ((int)(((byte)(245)))));
            this.panel2.Location = new System.Drawing.Point(170, 60);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(412, 3);
            this.panel2.TabIndex = 4;
            // 
            // textBoxSubjectName
            // 
            this.textBoxSubjectName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textBoxSubjectName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.textBoxSubjectName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSubjectName.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSubjectName.ForeColor = System.Drawing.Color.White;
            this.textBoxSubjectName.Location = new System.Drawing.Point(170, 38);
            this.textBoxSubjectName.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.textBoxSubjectName.Name = "textBoxSubjectName";
            this.textBoxSubjectName.Size = new System.Drawing.Size(412, 24);
            this.textBoxSubjectName.TabIndex = 2;
            this.textBoxSubjectName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelSubjectName
            // 
            this.labelSubjectName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelSubjectName.AutoSize = true;
            this.labelSubjectName.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubjectName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(147)))), ((int)(((byte)(41)))));
            this.labelSubjectName.Location = new System.Drawing.Point(298, 10);
            this.labelSubjectName.Margin = new System.Windows.Forms.Padding(0);
            this.labelSubjectName.Name = "labelSubjectName";
            this.labelSubjectName.Size = new System.Drawing.Size(156, 24);
            this.labelSubjectName.TabIndex = 1;
            this.labelSubjectName.Text = "Lecture name";
            // 
            // panelAddSubjectLabel
            // 
            this.panelAddSubjectLabel.AutoSize = true;
            this.panelAddSubjectLabel.Controls.Add(this.label3);
            this.panelAddSubjectLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAddSubjectLabel.Location = new System.Drawing.Point(0, 0);
            this.panelAddSubjectLabel.Name = "panelAddSubjectLabel";
            this.panelAddSubjectLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 30);
            this.panelAddSubjectLabel.Size = new System.Drawing.Size(769, 71);
            this.panelAddSubjectLabel.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(123)))), ((int)(((byte)(245)))));
            this.label3.Location = new System.Drawing.Point(258, 8);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(237, 33);
            this.label3.TabIndex = 1;
            this.label3.Text = "Add new lecture";
            // 
            // panelSettings
            // 
            this.panelSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSettings.Location = new System.Drawing.Point(0, 0);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(786, 506);
            this.panelSettings.TabIndex = 7;
            // 
            // panelMainWindows
            // 
            this.panelMainWindows.Controls.Add(this.panelRecord);
            this.panelMainWindows.Controls.Add(this.panelAddSubjects);
            this.panelMainWindows.Controls.Add(this.panelSubjects);
            this.panelMainWindows.Controls.Add(this.panelSettings);
            this.panelMainWindows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainWindows.Location = new System.Drawing.Point(163, 35);
            this.panelMainWindows.Name = "panelMainWindows";
            this.panelMainWindows.Size = new System.Drawing.Size(786, 506);
            this.panelMainWindows.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(652, 366);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 542);
            this.Controls.Add(this.panelMainWindows);
            this.Controls.Add(this.panelTitleBar);
            this.Controls.Add(this.panelMenuBorder);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelBorderBottom);
            this.Controls.Add(this.panelBorderTop);
            this.Controls.Add(this.panelBorderLeft);
            this.Controls.Add(this.panelBorderRight);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Lecture Recorder";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelMenu.ResumeLayout(false);
            this.panelTitleBar.ResumeLayout(false);
            this.panelRecord.ResumeLayout(false);
            this.panelRecord.PerformLayout();
            this.panelRecordArmed.ResumeLayout(false);
            this.panelRecordArmed.PerformLayout();
            this.panelRecordContent.ResumeLayout(false);
            this.panelRecordContent.PerformLayout();
            this.panelAddSubjects.ResumeLayout(false);
            this.panelAddSubjects.PerformLayout();
            this.panelAddButton.ResumeLayout(false);
            this.panelEndTime.ResumeLayout(false);
            this.panelEndTime.PerformLayout();
            this.panelStartTime.ResumeLayout(false);
            this.panelStartTime.PerformLayout();
            this.panelSubjectDay.ResumeLayout(false);
            this.panelSubjectDay.PerformLayout();
            this.panelSubjectName.ResumeLayout(false);
            this.panelSubjectName.PerformLayout();
            this.panelAddSubjectLabel.ResumeLayout(false);
            this.panelAddSubjectLabel.PerformLayout();
            this.panelMainWindows.ResumeLayout(false);
            this.panelMainWindows.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBorderRight;
        private System.Windows.Forms.Panel panelBorderLeft;
        private System.Windows.Forms.Panel panelBorderTop;
        private System.Windows.Forms.Panel panelBorderBottom;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelMenuBorder;
        private System.Windows.Forms.Panel panelTitleBar;
        private System.Windows.Forms.Button buttonMinimize;
        private System.Windows.Forms.Button buttonMaximize;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Panel panelRecord;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.RadioButton radioMenuRecord;
        private System.Windows.Forms.RadioButton radioMenuSettings;
        private System.Windows.Forms.RadioButton radioMenuAdd;
        private System.Windows.Forms.RadioButton radioMenuSubjects;
        private System.Windows.Forms.CheckBox checkBoxRecordButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelRecordTimer;
        private System.Windows.Forms.TextBox textBoxRecordInfo;
        private System.Windows.Forms.Panel panelRecordContent;
        private System.Windows.Forms.Panel panelRecordArmed;
        private System.Windows.Forms.Panel panelSubjects;
        private System.Windows.Forms.Panel panelAddSubjects;
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.Panel panelAddSubjectLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelSubjectName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxSubjectName;
        private System.Windows.Forms.Label labelSubjectName;
        private System.Windows.Forms.Panel panelSubjectDay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelStartTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Panel panelEndTime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Panel panelAddButton;
        private System.Windows.Forms.Button buttonAddSubject;
        private System.Windows.Forms.Panel panelMainWindows;
        private DropdownList dropdownDay;
        private DropdownList dropdownStartMin;
        private DropdownList dropdownStartHour;
        private DropdownList dropdownEndMin;
        private DropdownList dropdownEndHour;
        private System.Windows.Forms.Button button1;
    }
}

