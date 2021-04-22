namespace Barotrauma_Circuit_Resolver
{
    partial class SubResolverForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubResolverForm));
            this.FilepathTextBox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.GoButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SettingGroupBox = new System.Windows.Forms.GroupBox();
            this.SaveGraphCheckBox = new System.Windows.Forms.CheckBox();
            this.NewSubCheckBox = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SettingGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilepathTextBox
            // 
            this.FilepathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilepathTextBox.Location = new System.Drawing.Point(10, 11);
            this.FilepathTextBox.Name = "FilepathTextBox";
            this.FilepathTextBox.Size = new System.Drawing.Size(387, 23);
            this.FilepathTextBox.TabIndex = 0;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.Location = new System.Drawing.Point(403, 11);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(61, 21);
            this.BrowseButton.TabIndex = 1;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GoButton.Location = new System.Drawing.Point(299, 335);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(165, 94);
            this.GoButton.TabIndex = 2;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox1.Location = new System.Drawing.Point(10, 39);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(453, 260);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // SettingGroupBox
            // 
            this.SettingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingGroupBox.Controls.Add(this.SaveGraphCheckBox);
            this.SettingGroupBox.Controls.Add(this.NewSubCheckBox);
            this.SettingGroupBox.Location = new System.Drawing.Point(10, 335);
            this.SettingGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SettingGroupBox.Name = "SettingGroupBox";
            this.SettingGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SettingGroupBox.Size = new System.Drawing.Size(271, 94);
            this.SettingGroupBox.TabIndex = 4;
            this.SettingGroupBox.TabStop = false;
            this.SettingGroupBox.Text = "Settings";
            // 
            // SaveGraphCheckBox
            // 
            this.SaveGraphCheckBox.Location = new System.Drawing.Point(5, 42);
            this.SaveGraphCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SaveGraphCheckBox.Name = "SaveGraphCheckBox";
            this.SaveGraphCheckBox.Size = new System.Drawing.Size(210, 18);
            this.SaveGraphCheckBox.TabIndex = 0;
            this.SaveGraphCheckBox.Text = "Save Graph";
            this.SaveGraphCheckBox.UseVisualStyleBackColor = true;
            this.SaveGraphCheckBox.CheckedChanged += new System.EventHandler(this.NewSubCheckBox_CheckedChanged);
            // 
            // NewSubCheckBox
            // 
            this.NewSubCheckBox.Location = new System.Drawing.Point(5, 20);
            this.NewSubCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.NewSubCheckBox.Name = "NewSubCheckBox";
            this.NewSubCheckBox.Size = new System.Drawing.Size(210, 18);
            this.NewSubCheckBox.TabIndex = 0;
            this.NewSubCheckBox.Text = "Create New Submarine File";
            this.NewSubCheckBox.UseVisualStyleBackColor = true;
            this.NewSubCheckBox.CheckedChanged += new System.EventHandler(this.NewSubCheckBox_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(10, 304);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(454, 23);
            this.progressBar1.TabIndex = 5;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.label1.Location = new System.Drawing.Point(13, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 15);
            this.label1.TabIndex = 6;
            // 
            // SubResolverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 436);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.SettingGroupBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.FilepathTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.MinimumSize = new System.Drawing.Size(450, 215);
            this.Name = "SubResolverForm";
            this.Text = "Barotrauma Circuit Resolver";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.SettingGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.TextBox FilepathTextBox;

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox SettingGroupBox;
        private System.Windows.Forms.CheckBox NewSubCheckBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox SaveGraphCheckBox;
    }
}

