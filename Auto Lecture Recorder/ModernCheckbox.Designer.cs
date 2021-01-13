namespace Auto_Lecture_Recorder
{
    partial class ModernCheckbox
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
            this.label = new System.Windows.Forms.Label();
            this.panelCheckmark = new System.Windows.Forms.Panel();
            this.checkbox = new System.Windows.Forms.PictureBox();
            this.panelCheckmark.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkbox)).BeginInit();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.LightGray;
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Margin = new System.Windows.Forms.Padding(0);
            this.label.Name = "label";
            this.label.Padding = new System.Windows.Forms.Padding(55, 0, 0, 0);
            this.label.Size = new System.Drawing.Size(347, 34);
            this.label.TabIndex = 4;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label.MouseEnter += new System.EventHandler(this.Component_MouseEnter);
            this.label.MouseLeave += new System.EventHandler(this.Component_MouseLeave);
            // 
            // panelCheckmark
            // 
            this.panelCheckmark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelCheckmark.Controls.Add(this.checkbox);
            this.panelCheckmark.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelCheckmark.Location = new System.Drawing.Point(0, 0);
            this.panelCheckmark.Margin = new System.Windows.Forms.Padding(0);
            this.panelCheckmark.Name = "panelCheckmark";
            this.panelCheckmark.Padding = new System.Windows.Forms.Padding(15, 1, 1, 1);
            this.panelCheckmark.Size = new System.Drawing.Size(53, 34);
            this.panelCheckmark.TabIndex = 5;
            this.panelCheckmark.MouseEnter += new System.EventHandler(this.Component_MouseEnter);
            this.panelCheckmark.MouseLeave += new System.EventHandler(this.Component_MouseLeave);
            // 
            // checkbox
            // 
            this.checkbox.BackgroundImage = global::Auto_Lecture_Recorder.Properties.Resources.checkbox;
            this.checkbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.checkbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkbox.Image = global::Auto_Lecture_Recorder.Properties.Resources.checkbox;
            this.checkbox.Location = new System.Drawing.Point(15, 1);
            this.checkbox.Margin = new System.Windows.Forms.Padding(0);
            this.checkbox.Name = "checkbox";
            this.checkbox.Size = new System.Drawing.Size(37, 32);
            this.checkbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.checkbox.TabIndex = 0;
            this.checkbox.TabStop = false;
            this.checkbox.MouseEnter += new System.EventHandler(this.Component_MouseEnter);
            this.checkbox.MouseLeave += new System.EventHandler(this.Component_MouseLeave);
            // 
            // ModernCheckbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(66)))));
            this.Controls.Add(this.panelCheckmark);
            this.Controls.Add(this.label);
            this.Name = "ModernCheckbox";
            this.Size = new System.Drawing.Size(347, 34);
            this.panelCheckmark.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkbox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Panel panelCheckmark;
        private System.Windows.Forms.PictureBox checkbox;
    }
}
