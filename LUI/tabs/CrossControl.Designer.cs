﻿namespace LUI.tabs
{
    partial class CrossControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.LoadAngles = new System.Windows.Forms.Button();
            this.AnglesView = new System.Windows.Forms.DataGridView();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveData = new System.Windows.Forms.Button();
            this.AngleProgress = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
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
            this.ParentPanel.SuspendLayout();
            this.StatusBox.SuspendLayout();
            this.CommandsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NScan)).BeginInit();
            this.CommonObjectPanel.SuspendLayout();
            this.LeftChildArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraGain)).BeginInit();
            this.RightChildArea.SuspendLayout();
            this.LeftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AnglesView)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraTemperature)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusBox
            // 
            this.StatusBox.Controls.Add(this.label2);
            this.StatusBox.Controls.Add(this.AngleProgress);
            this.StatusBox.Controls.SetChildIndex(this.ProgressLabel, 0);
            this.StatusBox.Controls.SetChildIndex(this.CameraStatus, 0);
            this.StatusBox.Controls.SetChildIndex(this.ScanProgress, 0);
            this.StatusBox.Controls.SetChildIndex(this.AngleProgress, 0);
            this.StatusBox.Controls.SetChildIndex(this.label2, 0);
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
            // Graph
            // 
            this.Graph.Size = new System.Drawing.Size(1176, 583);
            this.Graph.XLeft = 1F;
            this.Graph.XRight = 1024F;
            // 
            // LeftChildArea
            // 
            this.LeftChildArea.Controls.Add(this.panel2);
            this.LeftChildArea.Location = new System.Drawing.Point(0, 583);
            this.LeftChildArea.Size = new System.Drawing.Size(1176, 238);
            // 
            // RightChildArea
            // 
            this.RightChildArea.Controls.Add(this.splitContainer1);
            // 
            // LoadAngles
            // 
            this.LoadAngles.Location = new System.Drawing.Point(3, 2);
            this.LoadAngles.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LoadAngles.Name = "LoadAngles";
            this.LoadAngles.Size = new System.Drawing.Size(136, 34);
            this.LoadAngles.TabIndex = 10;
            this.LoadAngles.Text = "Load Angles";
            this.LoadAngles.UseVisualStyleBackColor = true;
            this.LoadAngles.Click += new System.EventHandler(this.LoadAngles_Click);
            // 
            // AnglesView
            // 
            this.AnglesView.AllowUserToAddRows = false;
            this.AnglesView.AllowUserToDeleteRows = false;
            this.AnglesView.AllowUserToOrderColumns = true;
            this.AnglesView.AllowUserToResizeColumns = false;
            this.AnglesView.AllowUserToResizeRows = false;
            this.AnglesView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.AnglesView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.AnglesView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AnglesView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Value});
            this.AnglesView.Location = new System.Drawing.Point(3, 41);
            this.AnglesView.MultiSelect = false;
            this.AnglesView.Name = "AnglesView";
            this.AnglesView.RowHeadersVisible = false;
            this.AnglesView.RowTemplate.Height = 24;
            this.AnglesView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.AnglesView.ShowEditingIcon = false;
            this.AnglesView.Size = new System.Drawing.Size(136, 150);
            this.AnglesView.TabIndex = 11;
            // 
            // Value
            // 
            this.Value.DataPropertyName = "Value";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Format = "N2";
            dataGridViewCellStyle1.NullValue = null;
            this.Value.DefaultCellStyle = dataGridViewCellStyle1;
            this.Value.HeaderText = "Degree (deg)";
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            this.Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SaveData
            // 
            this.SaveData.Location = new System.Drawing.Point(3, 196);
            this.SaveData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SaveData.Name = "SaveData";
            this.SaveData.Size = new System.Drawing.Size(136, 34);
            this.SaveData.TabIndex = 12;
            this.SaveData.Text = "Save Data";
            this.SaveData.UseVisualStyleBackColor = true;
            // 
            // AngleProgress
            // 
            this.AngleProgress.Location = new System.Drawing.Point(151, 37);
            this.AngleProgress.Name = "AngleProgress";
            this.AngleProgress.ReadOnly = true;
            this.AngleProgress.Size = new System.Drawing.Size(58, 20);
            this.AngleProgress.TabIndex = 14;
            this.AngleProgress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.LoadAngles);
            this.panel2.Controls.Add(this.AnglesView);
            this.panel2.Controls.Add(this.SaveData);
            this.panel2.Location = new System.Drawing.Point(1028, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(142, 232);
            this.panel2.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(110, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Angles";
            // 
            // Flow
            // 
            this.Flow.AutoSize = true;
            this.Flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.Flow.Location = new System.Drawing.Point(3, 16);
            this.Flow.Name = "Flow";
            this.Flow.Size = new System.Drawing.Size(294, 73);
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
            this.PolarizerBox.Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PolarizerBox.Flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.PolarizerBox.Flow.Location = new System.Drawing.Point(3, 16);
            this.PolarizerBox.Flow.Name = "Flow";
            this.PolarizerBox.Flow.Size = new System.Drawing.Size(294, 33);
            this.PolarizerBox.Flow.TabIndex = 0;
            this.PolarizerBox.Location = new System.Drawing.Point(0, 0);
            this.PolarizerBox.Name = "PolarizerBox";
            this.PolarizerBox.SelectedObject = null;
            this.PolarizerBox.Size = new System.Drawing.Size(300, 52);
            this.PolarizerBox.TabIndex = 18;
            this.PolarizerBox.Text = "Polarizer";
            // 
            // CrossControl
            // 
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "CrossControl";
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
            ((System.ComponentModel.ISupportInitialize)(this.AnglesView)).EndInit();
            this.panel2.ResumeLayout(false);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadAngles;
        private System.Windows.Forms.DataGridView AnglesView;
        private System.Windows.Forms.Button SaveData;
        protected System.Windows.Forms.TextBox AngleProgress;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Label PolarizerBetaLabel;
        private System.Windows.Forms.NumericUpDown Beta;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private controls.ObjectCommandPanel PolarizerBox;
        private System.Windows.Forms.FlowLayoutPanel Flow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown CameraTemperature;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton ImageMode;
        private System.Windows.Forms.RadioButton FvbMode;
    }
}
