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
            this.FilepathTextBox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.GoButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SettingGroupBox = new System.Windows.Forms.GroupBox();
            this.NewSubCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SettingGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilepathTextBox
            // 
            this.FilepathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilepathTextBox.Location = new System.Drawing.Point(12, 15);
            this.FilepathTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.FilepathTextBox.Name = "FilepathTextBox";
            this.FilepathTextBox.Size = new System.Drawing.Size(384, 27);
            this.FilepathTextBox.TabIndex = 0;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.Location = new System.Drawing.Point(402, 15);
            this.BrowseButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(70, 28);
            this.BrowseButton.TabIndex = 1;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GoButton.Location = new System.Drawing.Point(283, 236);
            this.GoButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(189, 100);
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
            this.pictureBox1.Location = new System.Drawing.Point(12, 49);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(460, 180);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // SettingGroupBox
            // 
            this.SettingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingGroupBox.Controls.Add(this.NewSubCheckBox);
            this.SettingGroupBox.Location = new System.Drawing.Point(12, 236);
            this.SettingGroupBox.Name = "SettingGroupBox";
            this.SettingGroupBox.Size = new System.Drawing.Size(252, 99);
            this.SettingGroupBox.TabIndex = 4;
            this.SettingGroupBox.TabStop = false;
            this.SettingGroupBox.Text = "Settings";
            // 
            // NewSubCheckBox
            // 
            this.NewSubCheckBox.Location = new System.Drawing.Point(6, 26);
            this.NewSubCheckBox.Name = "NewSubCheckBox";
            this.NewSubCheckBox.Size = new System.Drawing.Size(240, 24);
            this.NewSubCheckBox.TabIndex = 0;
            this.NewSubCheckBox.Text = "Create New Submarine File";
            this.NewSubCheckBox.UseVisualStyleBackColor = true;
            this.NewSubCheckBox.CheckedChanged += new System.EventHandler(this.NewSubCheckBox_CheckedChanged);
            // 
            // SubResolverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 345);
            this.Controls.Add(this.SettingGroupBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.FilepathTextBox);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(502, 392);
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
    }
}

