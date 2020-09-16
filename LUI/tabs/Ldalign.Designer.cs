namespace LUI.tabs
{
    partial class LdalignControl
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
            System.Windows.Forms.Label NAverageLabel;
            this.Flow = new System.Windows.Forms.FlowLayoutPanel();
            this.PolarizerBetaLabel = new System.Windows.Forms.Label();
            this.Beta = new System.Windows.Forms.NumericUpDown();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.CameraTemperature = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ImageMode = new System.Windows.Forms.RadioButton();
            this.FvbMode = new System.Windows.Forms.RadioButton();
            this.PolarizerBox = new LUI.controls.ObjectCommandPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.LoadProfile = new System.Windows.Forms.Button();
            this.SaveData = new System.Windows.Forms.Button();
            this.SaveProfile = new System.Windows.Forms.Button();
            this.OptionsBox = new System.Windows.Forms.GroupBox();
            this.ShowDifference = new System.Windows.Forms.CheckBox();
            this.ShowLast = new System.Windows.Forms.CheckBox();
            this.PersistentGraphing = new System.Windows.Forms.CheckBox();
            this.CollectLaser = new System.Windows.Forms.CheckBox();
            this.NAverage = new System.Windows.Forms.NumericUpDown();
            this.DdgConfigBox = new LUI.controls.DdgCommandPanel();
            NAverageLabel = new System.Windows.Forms.Label();
            this.ParentPanel.SuspendLayout();
            this.StatusBox.SuspendLayout();
            this.CommandsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NScan)).BeginInit();
            this.CommonObjectPanel.SuspendLayout();
            this.LeftChildArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraGain)).BeginInit();
            this.RightChildArea.SuspendLayout();
            this.LeftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraTemperature)).BeginInit();
            this.panel3.SuspendLayout();
            this.PolarizerBox.Flow.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).BeginInit();
            this.panel2.SuspendLayout();
            this.OptionsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NAverage)).BeginInit();
            this.SuspendLayout();
            this.StatusBox.Controls.SetChildIndex(this.ProgressLabel, 0);
            this.StatusBox.Controls.SetChildIndex(this.CameraStatus, 0);
            this.StatusBox.Controls.SetChildIndex(this.ScanProgress, 0);
            // 
            // NScan
            // 
            this.NScan.Margin = new System.Windows.Forms.Padding(9, 7, 9, 7);
            this.NScan.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // Collect
            // 
            this.Collect.Text = "Collect LD";
            // 
            // Graph
            // 
            this.Graph.Size = new System.Drawing.Size(1176, 696);
            this.Graph.XLeft = 1F;
            this.Graph.XRight = 1024F;
            // 
            // LeftChildArea
            // 
            this.LeftChildArea.Controls.Add(this.DdgConfigBox);
            this.LeftChildArea.Controls.Add(this.OptionsBox);
            this.LeftChildArea.Controls.Add(this.panel2);
            this.LeftChildArea.Location = new System.Drawing.Point(0, 696);
            this.LeftChildArea.Size = new System.Drawing.Size(1176, 125);
            // 
            // RightChildArea
            // 
            this.RightChildArea.Controls.Add(this.splitContainer1);
            // 
            // NAverageLabel
            // 
            NAverageLabel.AutoSize = true;
            NAverageLabel.Location = new System.Drawing.Point(63, 81);
            NAverageLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            NAverageLabel.Name = "NAverageLabel";
            NAverageLabel.Size = new System.Drawing.Size(99, 13);
            NAverageLabel.TabIndex = 13;
            NAverageLabel.Text = "Local Average Size";
            // 
            // Flow
            // 
            this.Flow.AutoSize = true;
            this.Flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.Flow.Location = new System.Drawing.Point(3, 16);
            this.Flow.Name = "Flow";
            this.Flow.Size = new System.Drawing.Size(294, 33);
            this.Flow.TabIndex = 0;
            // 
            // PolarizerBetaLabel
            // 
            this.PolarizerBetaLabel.AutoSize = true;
            this.PolarizerBetaLabel.Location = new System.Drawing.Point(43, 12);
            this.PolarizerBetaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PolarizerBetaLabel.Name = "PolarizerBetaLabel";
            this.PolarizerBetaLabel.Size = new System.Drawing.Size(29, 13);
            this.PolarizerBetaLabel.TabIndex = 10;
            // 
            // Beta
            // 
            this.Beta.Location = new System.Drawing.Point(80, 10);
            this.Beta.Margin = new System.Windows.Forms.Padding(4);
            this.Beta.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.Beta.Name = "Beta";
            this.Beta.Size = new System.Drawing.Size(67, 20);
            this.Beta.TabIndex = 10;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.PolarizerBox);
            this.splitContainer1.Size = new System.Drawing.Size(300, 286);
            this.splitContainer1.SplitterDistance = 110;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.CameraTemperature);
            this.groupBox2.Controls.Add(this.panel3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 101);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Additional Camera Options";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(129, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Camera Temperature (°C):";
            // 
            // CameraTemperature
            // 
            this.CameraTemperature.Location = new System.Drawing.Point(141, 62);
            this.CameraTemperature.Name = "CameraTemperature";
            this.CameraTemperature.Size = new System.Drawing.Size(48, 20);
            this.CameraTemperature.TabIndex = 24;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.ImageMode);
            this.panel3.Controls.Add(this.FvbMode);
            this.panel3.Location = new System.Drawing.Point(6, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(183, 23);
            this.panel3.TabIndex = 23;
            // 
            // ImageMode
            // 
            this.ImageMode.AutoSize = true;
            this.ImageMode.Location = new System.Drawing.Point(126, 3);
            this.ImageMode.Name = "ImageMode";
            this.ImageMode.Size = new System.Drawing.Size(54, 17);
            this.ImageMode.TabIndex = 1;
            this.ImageMode.Text = "Image";
            this.ImageMode.UseVisualStyleBackColor = true;
            // 
            // FvbMode
            // 
            this.FvbMode.AutoSize = true;
            this.FvbMode.Checked = true;
            this.FvbMode.Location = new System.Drawing.Point(3, 3);
            this.FvbMode.Name = "FvbMode";
            this.FvbMode.Size = new System.Drawing.Size(117, 17);
            this.FvbMode.TabIndex = 0;
            this.FvbMode.TabStop = true;
            this.FvbMode.Text = "Full Vertical Binning";
            this.FvbMode.UseVisualStyleBackColor = true;
            // 
            // PolarizerBox
            // 
            this.PolarizerBox.AutoSize = true;
            this.PolarizerBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PolarizerBox.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // PolarizerBox.Flow
            // 
            this.PolarizerBox.Flow.AutoSize = true;
            this.PolarizerBox.Flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PolarizerBox.Flow.Controls.Add(this.panel1);
            this.PolarizerBox.Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PolarizerBox.Flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.PolarizerBox.Flow.Location = new System.Drawing.Point(3, 16);
            this.PolarizerBox.Flow.Name = "Flow";
            this.PolarizerBox.Flow.Size = new System.Drawing.Size(294, 73);
            this.PolarizerBox.Flow.TabIndex = 0;
            this.PolarizerBox.Location = new System.Drawing.Point(0, 0);
            this.PolarizerBox.Name = "PolarizerBox";
            this.PolarizerBox.SelectedObject = null;
            this.PolarizerBox.Size = new System.Drawing.Size(300, 92);
            this.PolarizerBox.TabIndex = 18;
            this.PolarizerBox.Text = "Polarizer";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.Beta);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(3, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(151, 34);
            this.panel1.TabIndex = 2;
            // 
            // Beta2
            // 
            this.Beta.DecimalPlaces = 2;
            this.Beta.Increment = new decimal(new int[] {
            2,
            0,
            0,
            131072});
            this.Beta.Location = new System.Drawing.Point(80, 10);
            this.Beta.Margin = new System.Windows.Forms.Padding(4);
            this.Beta.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.Beta.Name = "Beta";
            this.Beta.Size = new System.Drawing.Size(67, 20);
            this.Beta.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Beta";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.LoadProfile);
            this.panel2.Controls.Add(this.SaveData);
            this.panel2.Controls.Add(this.SaveProfile);
            this.panel2.Location = new System.Drawing.Point(1031, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(142, 114);
            this.panel2.TabIndex = 25;
            // 
            // LoadProfile
            // 
            this.LoadProfile.Location = new System.Drawing.Point(3, 2);
            this.LoadProfile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LoadProfile.Name = "LoadProfile";
            this.LoadProfile.Size = new System.Drawing.Size(136, 34);
            this.LoadProfile.TabIndex = 2;
            this.LoadProfile.Text = "Load Profile";
            this.LoadProfile.UseVisualStyleBackColor = true;
            // 
            // SaveData
            // 
            this.SaveData.Location = new System.Drawing.Point(3, 78);
            this.SaveData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SaveData.Name = "SaveData";
            this.SaveData.Size = new System.Drawing.Size(136, 34);
            this.SaveData.TabIndex = 23;
            this.SaveData.Text = "Save Data";
            this.SaveData.UseVisualStyleBackColor = true;
            // 
            // SaveProfile
            // 
            this.SaveProfile.Location = new System.Drawing.Point(3, 40);
            this.SaveProfile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SaveProfile.Name = "SaveProfile";
            this.SaveProfile.Size = new System.Drawing.Size(136, 34);
            this.SaveProfile.TabIndex = 16;
            this.SaveProfile.Text = "Save Profile";
            this.SaveProfile.UseVisualStyleBackColor = true;
            // 
            // OptionsBox
            // 
            this.OptionsBox.AutoSize = true;
            this.OptionsBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.OptionsBox.Controls.Add(this.ShowDifference);
            this.OptionsBox.Controls.Add(this.ShowLast);
            this.OptionsBox.Controls.Add(this.PersistentGraphing);
            this.OptionsBox.Controls.Add(this.CollectLaser);
            this.OptionsBox.Controls.Add(this.NAverage);
            this.OptionsBox.Controls.Add(NAverageLabel);
            this.OptionsBox.Location = new System.Drawing.Point(436, 3);
            this.OptionsBox.Name = "OptionsBox";
            this.OptionsBox.Size = new System.Drawing.Size(304, 119);
            this.OptionsBox.TabIndex = 26;
            this.OptionsBox.TabStop = false;
            this.OptionsBox.Text = "Options";
            // 
            // ShowDifference
            // 
            this.ShowDifference.AutoSize = true;
            this.ShowDifference.Checked = true;
            this.ShowDifference.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowDifference.Location = new System.Drawing.Point(7, 22);
            this.ShowDifference.Margin = new System.Windows.Forms.Padding(4);
            this.ShowDifference.Name = "ShowDifference";
            this.ShowDifference.Size = new System.Drawing.Size(105, 17);
            this.ShowDifference.TabIndex = 4;
            this.ShowDifference.Text = "Show Difference";
            this.ShowDifference.UseVisualStyleBackColor = true;
            // 
            // ShowLast
            // 
            this.ShowLast.AutoSize = true;
            this.ShowLast.Checked = true;
            this.ShowLast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowLast.Location = new System.Drawing.Point(7, 50);
            this.ShowLast.Margin = new System.Windows.Forms.Padding(4);
            this.ShowLast.Name = "ShowLast";
            this.ShowLast.Size = new System.Drawing.Size(124, 17);
            this.ShowLast.TabIndex = 3;
            this.ShowLast.Text = "Show Loaded Profile";
            this.ShowLast.UseVisualStyleBackColor = true;
            // 
            // PersistentGraphing
            // 
            this.PersistentGraphing.AutoSize = true;
            this.PersistentGraphing.Checked = true;
            this.PersistentGraphing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PersistentGraphing.Location = new System.Drawing.Point(174, 22);
            this.PersistentGraphing.Margin = new System.Windows.Forms.Padding(4);
            this.PersistentGraphing.Name = "PersistentGraphing";
            this.PersistentGraphing.Size = new System.Drawing.Size(118, 17);
            this.PersistentGraphing.TabIndex = 17;
            this.PersistentGraphing.Text = "Persistent Graphing";
            this.PersistentGraphing.UseVisualStyleBackColor = true;
            // 
            // CollectLaser
            // 
            this.CollectLaser.AutoSize = true;
            this.CollectLaser.Location = new System.Drawing.Point(174, 50);
            this.CollectLaser.Name = "CollectLaser";
            this.CollectLaser.Size = new System.Drawing.Size(124, 17);
            this.CollectLaser.TabIndex = 19;
            this.CollectLaser.Text = "Collect Laser Scatter";
            this.CollectLaser.UseVisualStyleBackColor = true;
            // 
            // NAverage
            // 
            this.NAverage.Location = new System.Drawing.Point(7, 79);
            this.NAverage.Margin = new System.Windows.Forms.Padding(4);
            this.NAverage.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.NAverage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NAverage.Name = "NAverage";
            this.NAverage.Size = new System.Drawing.Size(48, 20);
            this.NAverage.TabIndex = 12;
            this.NAverage.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // DdgConfigBox
            // 
            this.DdgConfigBox.AutoSize = true;
            this.DdgConfigBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DdgConfigBox.Location = new System.Drawing.Point(2, 5);
            this.DdgConfigBox.Margin = new System.Windows.Forms.Padding(2);
            this.DdgConfigBox.Name = "DdgConfigBox";
            this.DdgConfigBox.Size = new System.Drawing.Size(362, 59);
            this.DdgConfigBox.TabIndex = 27;
            // 
            // LdalignControl
            // 
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "LdalignControl";
            this.ParentPanel.ResumeLayout(false);
            this.ParentPanel.PerformLayout();
            this.StatusBox.ResumeLayout(false);
            this.StatusBox.PerformLayout();
            this.CommandsBox.ResumeLayout(false);
            this.CommandsBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NScan)).EndInit();
            this.CommonObjectPanel.ResumeLayout(false);
            this.CommonObjectPanel.PerformLayout();
            this.LeftChildArea.ResumeLayout(false);
            this.LeftChildArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraGain)).EndInit();
            this.RightChildArea.ResumeLayout(false);
            this.LeftPanel.ResumeLayout(false);
            this.LeftPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraTemperature)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.PolarizerBox.Flow.ResumeLayout(false);
            this.PolarizerBox.Flow.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).EndInit();
            this.panel2.ResumeLayout(false);
            this.OptionsBox.ResumeLayout(false);
            this.OptionsBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NAverage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label PolarizerBetaLabel;
        private System.Windows.Forms.NumericUpDown Beta;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown CameraTemperature;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton ImageMode;
        private System.Windows.Forms.RadioButton FvbMode;
        private controls.ObjectCommandPanel PolarizerBox;
        private System.Windows.Forms.FlowLayoutPanel Flow;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button LoadProfile;
        private System.Windows.Forms.Button SaveData;
        private System.Windows.Forms.Button SaveProfile;
        private System.Windows.Forms.GroupBox OptionsBox;
        private System.Windows.Forms.CheckBox ShowDifference;
        private System.Windows.Forms.CheckBox ShowLast;
        private System.Windows.Forms.CheckBox PersistentGraphing;
        private System.Windows.Forms.CheckBox CollectLaser;
        private System.Windows.Forms.NumericUpDown NAverage;
        private controls.DdgCommandPanel DdgConfigBox;
    }
}
