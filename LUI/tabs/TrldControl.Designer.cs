namespace LUI.tabs
{
    partial class TrldControl
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
            this.PolarizerBetaLabel = new System.Windows.Forms.Label();
            this.LoadTimes = new System.Windows.Forms.Button();
            this.TimesView = new System.Windows.Forms.DataGridView();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveData = new System.Windows.Forms.Button();
            this.DdgConfigBox = new LUI.controls.DdgCommandPanel();
            this.PolarizerBox = new LUI.controls.ObjectCommandPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Beta = new System.Windows.Forms.NumericUpDown();
            this.PolarizerConfigBox = new System.Windows.Forms.GroupBox();
            this.buttonConfigTrld = new System.Windows.Forms.Button();
            this.buttonConfigORD = new System.Windows.Forms.Button();
            this.TimeProgress = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ParentPanel.SuspendLayout();
            this.StatusBox.SuspendLayout();
            this.CommandsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NScan)).BeginInit();
            this.CommonObjectPanel.SuspendLayout();
            this.LeftChildArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraGain)).BeginInit();
            this.RightChildArea.SuspendLayout();
            this.LeftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimesView)).BeginInit();
            this.PolarizerBox.Flow.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).BeginInit();
            this.PolarizerConfigBox.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusBox
            // 
            this.StatusBox.Controls.Add(this.label2);
            this.StatusBox.Controls.Add(this.TimeProgress);
            this.StatusBox.Controls.SetChildIndex(this.ProgressLabel, 0);
            this.StatusBox.Controls.SetChildIndex(this.CameraStatus, 0);
            this.StatusBox.Controls.SetChildIndex(this.ScanProgress, 0);
            this.StatusBox.Controls.SetChildIndex(this.TimeProgress, 0);
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
            // Collect
            // 
            this.Collect.Text = "Collect TRLD";
            // 
            // Graph
            // 
            this.Graph.InitialYMax = 1F;
            this.Graph.InitialYMin = -1F;
            this.Graph.Size = new System.Drawing.Size(1176, 583);
            this.Graph.XLeft = 1F;
            this.Graph.XRight = 1024F;
            this.Graph.YMax = 1F;
            this.Graph.YMin = -1F;
            // 
            // LeftChildArea
            // 
            this.LeftChildArea.Controls.Add(this.panel2);
            this.LeftChildArea.Controls.Add(this.DdgConfigBox);
            this.LeftChildArea.Location = new System.Drawing.Point(0, 583);
            this.LeftChildArea.Size = new System.Drawing.Size(1176, 238);
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
            // LoadTimes
            // 
            this.LoadTimes.Location = new System.Drawing.Point(3, 2);
            this.LoadTimes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LoadTimes.Name = "LoadTimes";
            this.LoadTimes.Size = new System.Drawing.Size(136, 34);
            this.LoadTimes.TabIndex = 10;
            this.LoadTimes.Text = "Load Times";
            this.LoadTimes.UseVisualStyleBackColor = true;
            this.LoadTimes.Click += new System.EventHandler(this.LoadTimes_Click);
            // 
            // TimesView
            // 
            this.TimesView.AllowUserToAddRows = false;
            this.TimesView.AllowUserToDeleteRows = false;
            this.TimesView.AllowUserToOrderColumns = true;
            this.TimesView.AllowUserToResizeColumns = false;
            this.TimesView.AllowUserToResizeRows = false;
            this.TimesView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.TimesView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.TimesView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TimesView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Value});
            this.TimesView.Location = new System.Drawing.Point(3, 41);
            this.TimesView.MultiSelect = false;
            this.TimesView.Name = "TimesView";
            this.TimesView.RowHeadersVisible = false;
            this.TimesView.RowTemplate.Height = 24;
            this.TimesView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.TimesView.ShowEditingIcon = false;
            this.TimesView.Size = new System.Drawing.Size(136, 150);
            this.TimesView.TabIndex = 11;
            // 
            // Value
            // 
            this.Value.DataPropertyName = "Value";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Value.DefaultCellStyle = dataGridViewCellStyle1;
            this.Value.HeaderText = "Delay (s)";
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
            // DdgConfigBox
            // 
            this.DdgConfigBox.AutoSize = true;
            this.DdgConfigBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DdgConfigBox.Location = new System.Drawing.Point(2, 4);
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
            this.PolarizerBox.Flow.Controls.Add(this.PolarizerConfigBox);
            this.PolarizerBox.Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PolarizerBox.Flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.PolarizerBox.Flow.Location = new System.Drawing.Point(3, 16);
            this.PolarizerBox.Flow.Name = "Flow";
            this.PolarizerBox.Flow.Size = new System.Drawing.Size(294, 143);
            this.PolarizerBox.Flow.TabIndex = 0;
            this.PolarizerBox.Location = new System.Drawing.Point(0, 0);
            this.PolarizerBox.Name = "PolarizerBox";
            this.PolarizerBox.SelectedObject = null;
            this.PolarizerBox.Size = new System.Drawing.Size(300, 162);
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
            // PolarizerConfigBox
            // 
            this.PolarizerConfigBox.Controls.Add(this.buttonConfigTrld);
            this.PolarizerConfigBox.Controls.Add(this.buttonConfigORD);
            this.PolarizerConfigBox.Location = new System.Drawing.Point(3, 76);
            this.PolarizerConfigBox.Name = "PolarizerConfigBox";
            this.PolarizerConfigBox.Size = new System.Drawing.Size(288, 64);
            this.PolarizerConfigBox.TabIndex = 13;
            this.PolarizerConfigBox.TabStop = false;
            // 
            // buttonConfigTrld
            // 
            this.buttonConfigTrld.Location = new System.Drawing.Point(2, 18);
            this.buttonConfigTrld.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonConfigTrld.Name = "buttonConfigTrld";
            this.buttonConfigTrld.Size = new System.Drawing.Size(136, 34);
            this.buttonConfigTrld.TabIndex = 11;
            this.buttonConfigTrld.Text = "rho 45 deg";
            this.buttonConfigTrld.UseVisualStyleBackColor = true;
            this.buttonConfigTrld.Click += new System.EventHandler(this.buttonConfigTrld_Click);
            // 
            // buttonConfigORD
            // 
            this.buttonConfigORD.Location = new System.Drawing.Point(148, 18);
            this.buttonConfigORD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonConfigORD.Name = "buttonConfigORD";
            this.buttonConfigORD.Size = new System.Drawing.Size(136, 34);
            this.buttonConfigORD.TabIndex = 12;
            this.buttonConfigORD.Text = "rho 0 deg";
            this.buttonConfigORD.UseVisualStyleBackColor = true;
            this.buttonConfigORD.Click += new System.EventHandler(this.buttonConfigORD_Click);
            // 
            // TimeProgress
            // 
            this.TimeProgress.Location = new System.Drawing.Point(164, 56);
            this.TimeProgress.Name = "TimeProgress";
            this.TimeProgress.ReadOnly = true;
            this.TimeProgress.Size = new System.Drawing.Size(58, 20);
            this.TimeProgress.TabIndex = 14;
            this.TimeProgress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.LoadTimes);
            this.panel2.Controls.Add(this.TimesView);
            this.panel2.Controls.Add(this.SaveData);
            this.panel2.Location = new System.Drawing.Point(1028, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(142, 232);
            this.panel2.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Times : ";
            // 
            // TrldControl
            // 
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "TrldControl";
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
            ((System.ComponentModel.ISupportInitialize)(this.TimesView)).EndInit();
            this.PolarizerBox.Flow.ResumeLayout(false);
            this.PolarizerBox.Flow.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Beta)).EndInit();
            this.PolarizerConfigBox.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadTimes;
        private System.Windows.Forms.DataGridView TimesView;
        private System.Windows.Forms.Button SaveData;
        private controls.DdgCommandPanel DdgConfigBox;
        private controls.ObjectCommandPanel PolarizerBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label PolarizerBetaLabel;
        private System.Windows.Forms.NumericUpDown Beta;
        protected System.Windows.Forms.TextBox TimeProgress;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button buttonConfigTrld;
        private System.Windows.Forms.Button buttonConfigORD;
        private System.Windows.Forms.GroupBox PolarizerConfigBox;
    }
}
