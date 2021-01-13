namespace Auto_Lecture_Recorder
{
    partial class ModernTextBox
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
            this.panelUnderline = new System.Windows.Forms.Panel();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // panelUnderline
            // 
            this.panelUnderline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(123)))), ((int)(((byte)(245)))));
            this.panelUnderline.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelUnderline.Location = new System.Drawing.Point(0, 36);
            this.panelUnderline.Margin = new System.Windows.Forms.Padding(0);
            this.panelUnderline.Name = "panelUnderline";
            this.panelUnderline.Size = new System.Drawing.Size(269, 3);
            this.panelUnderline.TabIndex = 6;
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.ForeColor = System.Drawing.Color.White;
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(269, 39);
            this.textBox.TabIndex = 7;
            this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ModernTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.panelUnderline);
            this.Controls.Add(this.textBox);
            this.Name = "ModernTextBox";
            this.Size = new System.Drawing.Size(269, 39);
            this.Load += new System.EventHandler(this.ModernTextBox_Load);
            this.Resize += new System.EventHandler(this.ModernTextBox_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelUnderline;
        public System.Windows.Forms.TextBox textBox;
    }
}
