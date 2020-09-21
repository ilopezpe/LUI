namespace LUI.tabs
{
    partial class LdextinctionControl
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
            this.SaveData = new System.Windows.Forms.Button();
            this.PolarizerBox = new LUI.controls.ObjectCommandPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Beta = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CountsLabel = new System.Windows.Forms.Label();
            this.CountsDisplay = new System.Windows.Forms.TextBox();
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
            this.panel2.SuspendLayout();
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
            this.Collect.Text = "Acquire";
            // 
            // Graph
            // 
            this.Graph.XLeft = 1F;
            this.Graph.XRight = 1024F;
            this.Graph.YLabelFormat = "G1";
            // 
            // LeftChildArea
            // 
            this.LeftChildArea.Controls.Add(this.CountsLabel);
            this.LeftChildArea.Controls.Add(this.CountsDisplay);
            this.LeftChildArea.Controls.Add(this.panel2);
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
            this.panel2.Controls.Add(this.SaveData);
            this.panel2.Location = new System.Drawing.Point(1028, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(142, 38);
            this.panel2.TabIndex = 16;
            // 
            // CountsLabel
            // 
            this.CountsLabel.AutoSize = true;
            this.CountsLabel.Location = new System.Drawing.Point(685, 5);
            this.CountsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CountsLabel.Name = "CountsLabel";
            this.CountsLabel.Size = new System.Drawing.Size(53, 13);
            this.CountsLabel.TabIndex = 18;
            this.CountsLabel.Text = "Extinction";
            // 
            // CountsDisplay
            // 
            this.CountsDisplay.Location = new System.Drawing.Point(545, 2);
            this.CountsDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.CountsDisplay.Name = "CountsDisplay";
            this.CountsDisplay.ReadOnly = true;
            this.CountsDisplay.Size = new System.Drawing.Size(132, 20);
            this.CountsDisplay.TabIndex = 17;
            this.CountsDisplay.TabStop = false;
            this.CountsDisplay.Text = "0";
            this.CountsDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LdextinctionControl
            // 
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "LdextinctionControl";
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
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button SaveData;
        private controls.ObjectCommandPanel PolarizerBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label PolarizerBetaLabel;
        private System.Windows.Forms.NumericUpDown Beta;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label CountsLabel;
        private System.Windows.Forms.TextBox CountsDisplay;
    }
}
