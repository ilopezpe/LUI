﻿namespace LUI.tabs
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
            this.PolarizerBetaLabel = new System.Windows.Forms.Label();
            this.DdgConfigBox = new LUI.controls.DdgCommandPanel();
            this.PolarizerBox = new LUI.controls.ObjectCommandPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Beta = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CountsLabel = new System.Windows.Forms.Label();
            this.CountsDisplay = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.SaveData = new System.Windows.Forms.Button();
            this.ParentPanel.SuspendLayout();
            this.StatusBox.SuspendLayout();
            this.CommandsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NScan)).BeginInit();
            this.CommonObjectPanel.SuspendLayout();
            this.LeftChildArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraGain)).BeginInit();
            this.RightChildArea.SuspendLayout();
            this.LeftPanel.SuspendLayout();
            this.PolarizerBox.Flow.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).BeginInit();
            this.panel3.SuspendLayout();
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
            this.Collect.Text = "Collect TRLD";
            // 
            // Graph
            // 
            this.Graph.InitialYMax = 1F;
            this.Graph.InitialYMin = -1F;
            this.Graph.XLeft = 1F;
            this.Graph.XRight = 1024F;
            this.Graph.YMax = 1F;
            this.Graph.YMin = -1F;
            // 
            // LeftChildArea
            // 
            this.LeftChildArea.Controls.Add(this.panel3);
            this.LeftChildArea.Controls.Add(this.CountsLabel);
            this.LeftChildArea.Controls.Add(this.CountsDisplay);
            this.LeftChildArea.Controls.Add(this.panel2);
            this.LeftChildArea.Controls.Add(this.DdgConfigBox);
            // 
            // RightChildArea
            // 
            this.RightChildArea.Controls.Add(this.PolarizerBox);
            // 
            // PolarizerBetaLabel
            // 
            this.PolarizerBetaLabel.AutoSize = true;
            this.PolarizerBetaLabel.Location = new System.Drawing.Point(43, 12);
            this.PolarizerBetaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PolarizerBetaLabel.Name = "PolarizerBetaLabel";
            this.PolarizerBetaLabel.Size = new System.Drawing.Size(29, 13);
            this.PolarizerBetaLabel.TabIndex = 10;
            this.PolarizerBetaLabel.Text = "Beta";
            // 
            // DdgConfigBox
            // 
            this.DdgConfigBox.AutoSize = true;
            this.DdgConfigBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DdgConfigBox.Location = new System.Drawing.Point(2, 2);
            this.DdgConfigBox.Margin = new System.Windows.Forms.Padding(2);
            this.DdgConfigBox.Name = "DdgConfigBox";
            this.DdgConfigBox.Size = new System.Drawing.Size(362, 59);
            this.DdgConfigBox.TabIndex = 15;
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
            this.PolarizerBox.TabIndex = 0;
            this.PolarizerBox.Text = "Polarizer";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.Beta);
            this.panel1.Controls.Add(this.PolarizerBetaLabel);
            this.panel1.Location = new System.Drawing.Point(3, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(151, 34);
            this.panel1.TabIndex = 1;
            // 
            // Beta
            // 
            this.Beta.DecimalPlaces = 2;
            this.Beta.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Location = new System.Drawing.Point(1170, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(0, 0);
            this.panel2.TabIndex = 16;
            // 
            // CountsLabel
            // 
            this.CountsLabel.AutoSize = true;
            this.CountsLabel.Location = new System.Drawing.Point(683, 9);
            this.CountsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CountsLabel.Name = "CountsLabel";
            this.CountsLabel.Size = new System.Drawing.Size(21, 13);
            this.CountsLabel.TabIndex = 20;
            this.CountsLabel.Text = "LD";
            // 
            // CountsDisplay
            // 
            this.CountsDisplay.Location = new System.Drawing.Point(543, 6);
            this.CountsDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.CountsDisplay.Name = "CountsDisplay";
            this.CountsDisplay.ReadOnly = true;
            this.CountsDisplay.Size = new System.Drawing.Size(132, 20);
            this.CountsDisplay.TabIndex = 19;
            this.CountsDisplay.Text = "0";
            this.CountsDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.SaveData);
            this.panel3.Location = new System.Drawing.Point(1028, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(142, 38);
            this.panel3.TabIndex = 21;
            // 
            // SaveData
            // 
            this.SaveData.Location = new System.Drawing.Point(3, 2);
            this.SaveData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SaveData.Name = "SaveData";
            this.SaveData.Size = new System.Drawing.Size(136, 34);
            this.SaveData.TabIndex = 12;
            this.SaveData.Text = "Save Data";
            this.SaveData.UseVisualStyleBackColor = true;
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
            this.RightChildArea.PerformLayout();
            this.LeftPanel.ResumeLayout(false);
            this.LeftPanel.PerformLayout();
            this.PolarizerBox.Flow.ResumeLayout(false);
            this.PolarizerBox.Flow.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private controls.DdgCommandPanel DdgConfigBox;
        private controls.ObjectCommandPanel PolarizerBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label PolarizerBetaLabel;
        private System.Windows.Forms.NumericUpDown Beta;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label CountsLabel;
        private System.Windows.Forms.TextBox CountsDisplay;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button SaveData;
    }
}
