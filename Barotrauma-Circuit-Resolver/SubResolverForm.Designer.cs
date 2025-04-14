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
            FilepathTextBox = new System.Windows.Forms.TextBox();
            BrowseButton = new System.Windows.Forms.Button();
            GoButton = new System.Windows.Forms.Button();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            SettingGroupBox = new System.Windows.Forms.GroupBox();
            InvertMemoryCheckBox = new System.Windows.Forms.CheckBox();
            RetainParallelCheckBox = new System.Windows.Forms.CheckBox();
            SaveGraphCheckBox = new System.Windows.Forms.CheckBox();
            NewSubCheckBox = new System.Windows.Forms.CheckBox();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            label1 = new System.Windows.Forms.Label();
            PickingTimeSortBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SettingGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // FilepathTextBox
            // 
            FilepathTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FilepathTextBox.Location = new System.Drawing.Point(10, 11);
            FilepathTextBox.Name = "FilepathTextBox";
            FilepathTextBox.Size = new System.Drawing.Size(387, 23);
            FilepathTextBox.TabIndex = 0;
            // 
            // BrowseButton
            // 
            BrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BrowseButton.Location = new System.Drawing.Point(403, 11);
            BrowseButton.Name = "BrowseButton";
            BrowseButton.Size = new System.Drawing.Size(61, 21);
            BrowseButton.TabIndex = 1;
            BrowseButton.Text = "Browse";
            BrowseButton.UseVisualStyleBackColor = true;
            BrowseButton.Click += BrowseButton_Click;
            // 
            // GoButton
            // 
            GoButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            GoButton.Location = new System.Drawing.Point(287, 366);
            GoButton.Name = "GoButton";
            GoButton.Size = new System.Drawing.Size(177, 138);
            GoButton.TabIndex = 2;
            GoButton.Text = "Go";
            GoButton.UseVisualStyleBackColor = true;
            GoButton.Click += GoButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            pictureBox1.Location = new System.Drawing.Point(11, 39);
            pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(453, 293);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // SettingGroupBox
            // 
            SettingGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            SettingGroupBox.Controls.Add(PickingTimeSortBox);
            SettingGroupBox.Controls.Add(InvertMemoryCheckBox);
            SettingGroupBox.Controls.Add(RetainParallelCheckBox);
            SettingGroupBox.Controls.Add(SaveGraphCheckBox);
            SettingGroupBox.Controls.Add(NewSubCheckBox);
            SettingGroupBox.Location = new System.Drawing.Point(10, 366);
            SettingGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            SettingGroupBox.Name = "SettingGroupBox";
            SettingGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            SettingGroupBox.Size = new System.Drawing.Size(271, 138);
            SettingGroupBox.TabIndex = 4;
            SettingGroupBox.TabStop = false;
            SettingGroupBox.Text = "Settings";
            SettingGroupBox.Enter += SettingGroupBox_Enter;
            // 
            // InvertMemoryCheckBox
            // 
            InvertMemoryCheckBox.Location = new System.Drawing.Point(6, 64);
            InvertMemoryCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            InvertMemoryCheckBox.Name = "InvertMemoryCheckBox";
            InvertMemoryCheckBox.Size = new System.Drawing.Size(233, 18);
            InvertMemoryCheckBox.TabIndex = 0;
            InvertMemoryCheckBox.Text = "Invert order of storage components";
            InvertMemoryCheckBox.UseVisualStyleBackColor = true;
            InvertMemoryCheckBox.CheckedChanged += NewSubCheckBox_CheckedChanged;
            // 
            // RetainParallelCheckBox
            // 
            RetainParallelCheckBox.Location = new System.Drawing.Point(6, 86);
            RetainParallelCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            RetainParallelCheckBox.Name = "RetainParallelCheckBox";
            RetainParallelCheckBox.Size = new System.Drawing.Size(233, 18);
            RetainParallelCheckBox.TabIndex = 0;
            RetainParallelCheckBox.Text = "Retain parallel component order";
            RetainParallelCheckBox.UseVisualStyleBackColor = true;
            RetainParallelCheckBox.CheckedChanged += NewSubCheckBox_CheckedChanged;
            // 
            // SaveGraphCheckBox
            // 
            SaveGraphCheckBox.Location = new System.Drawing.Point(5, 42);
            SaveGraphCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            SaveGraphCheckBox.Name = "SaveGraphCheckBox";
            SaveGraphCheckBox.Size = new System.Drawing.Size(210, 18);
            SaveGraphCheckBox.TabIndex = 0;
            SaveGraphCheckBox.Text = "Save Graph";
            SaveGraphCheckBox.UseVisualStyleBackColor = true;
            SaveGraphCheckBox.CheckedChanged += NewSubCheckBox_CheckedChanged;
            // 
            // NewSubCheckBox
            // 
            NewSubCheckBox.Location = new System.Drawing.Point(5, 20);
            NewSubCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            NewSubCheckBox.Name = "NewSubCheckBox";
            NewSubCheckBox.Size = new System.Drawing.Size(210, 18);
            NewSubCheckBox.TabIndex = 0;
            NewSubCheckBox.Text = "Create New Submarine File";
            NewSubCheckBox.UseVisualStyleBackColor = true;
            NewSubCheckBox.CheckedChanged += NewSubCheckBox_CheckedChanged;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBar1.Location = new System.Drawing.Point(11, 337);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(454, 23);
            progressBar1.TabIndex = 5;
            progressBar1.Click += progressBar1_Click;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            label1.Location = new System.Drawing.Point(14, 359);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(0, 15);
            label1.TabIndex = 6;
            // 
            // PickingTimeSortBox
            // 
            PickingTimeSortBox.Location = new System.Drawing.Point(5, 108);
            PickingTimeSortBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            PickingTimeSortBox.Name = "PickingTimeSortBox";
            PickingTimeSortBox.Size = new System.Drawing.Size(233, 18);
            PickingTimeSortBox.TabIndex = 1;
            PickingTimeSortBox.Text = "Sort by PickingTime";
            PickingTimeSortBox.UseVisualStyleBackColor = true;
            PickingTimeSortBox.CheckedChanged += PickingTimeSort_CheckedChanged;
            // 
            // SubResolverForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(475, 512);
            Controls.Add(label1);
            Controls.Add(progressBar1);
            Controls.Add(SettingGroupBox);
            Controls.Add(pictureBox1);
            Controls.Add(GoButton);
            Controls.Add(BrowseButton);
            Controls.Add(FilepathTextBox);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximumSize = new System.Drawing.Size(1920, 1080);
            MinimumSize = new System.Drawing.Size(450, 240);
            Name = "SubResolverForm";
            Text = "Barotrauma Circuit Resolver";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            SettingGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.CheckBox RetainParallelCheckBox;
        private System.Windows.Forms.CheckBox InvertMemoryCheckBox;
        private System.Windows.Forms.CheckBox PickingTimeSortBox;
    }
}

