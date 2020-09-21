namespace LUI.tabs
{
    partial class TroaControl
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
            this.LoadTimes = new System.Windows.Forms.Button();
            this.TimesView = new System.Windows.Forms.DataGridView();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveData = new System.Windows.Forms.Button();
            this.DdgConfigBox = new LUI.controls.DdgCommandPanel();
            this.SyringePumpBox = new LUI.controls.ObjectCommandPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SyringePumpAlways = new System.Windows.Forms.RadioButton();
            this.SyringePumpTs = new System.Windows.Forms.RadioButton();
            this.SyringePumpNever = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.DiscardSyringe = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.TimeProgress = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ExperimentConfigBox = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DiscardLaser = new System.Windows.Forms.NumericUpDown();
            this.GsDelay = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.schemeBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
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
            this.SyringePumpBox.Flow.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DiscardSyringe)).BeginInit();
            this.panel2.SuspendLayout();
            this.ExperimentConfigBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DiscardLaser)).BeginInit();
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
            // CommandsBox
            // 
            this.CommandsBox.Controls.Add(this.label5);
            this.CommandsBox.Controls.Add(this.schemeBox);
            this.CommandsBox.Size = new System.Drawing.Size(300, 228);
            this.CommandsBox.Controls.SetChildIndex(this.schemeBox, 0);
            this.CommandsBox.Controls.SetChildIndex(this.label5, 0);
            this.CommandsBox.Controls.SetChildIndex(this.Abort, 0);
            this.CommandsBox.Controls.SetChildIndex(this.Collect, 0);
            this.CommandsBox.Controls.SetChildIndex(this.NScan, 0);
            this.CommandsBox.Controls.SetChildIndex(this.Clear, 0);
            this.CommandsBox.Controls.SetChildIndex(this.Pause, 0);
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(6, 175);
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
            // Abort
            // 
            this.Abort.Location = new System.Drawing.Point(110, 101);
            // 
            // CommonObjectPanel
            // 
            this.CommonObjectPanel.Location = new System.Drawing.Point(0, 350);
            // 
            // Graph
            // 
            this.Graph.InitialScaleHeight = 2F;
            this.Graph.InitialYMax = 1F;
            this.Graph.InitialYMin = -1F;
            this.Graph.ScaleHeight = 2F;
            this.Graph.Size = new System.Drawing.Size(1176, 583);
            this.Graph.XLeft = 1F;
            this.Graph.XRight = 1024F;
            this.Graph.YMax = 1F;
            this.Graph.YMin = -1F;
            // 
            // LeftChildArea
            // 
            this.LeftChildArea.Controls.Add(this.ExperimentConfigBox);
            this.LeftChildArea.Controls.Add(this.panel2);
            this.LeftChildArea.Controls.Add(this.DdgConfigBox);
            this.LeftChildArea.Location = new System.Drawing.Point(0, 583);
            this.LeftChildArea.Size = new System.Drawing.Size(1176, 238);
            // 
            // RightChildArea
            // 
            this.RightChildArea.Controls.Add(this.SyringePumpBox);
            this.RightChildArea.Location = new System.Drawing.Point(0, 580);
            this.RightChildArea.Size = new System.Drawing.Size(300, 241);
            // 
            // Pause
            // 
            this.Pause.Location = new System.Drawing.Point(8, 101);
            // 
            // CameraStatus
            // 
            this.CameraStatus.Size = new System.Drawing.Size(137, 20);
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
            this.DdgConfigBox.Location = new System.Drawing.Point(2, 14);
            this.DdgConfigBox.Margin = new System.Windows.Forms.Padding(2);
            this.DdgConfigBox.MinimumSize = new System.Drawing.Size(0, 60);
            this.DdgConfigBox.Name = "DdgConfigBox";
            this.DdgConfigBox.Size = new System.Drawing.Size(362, 60);
            this.DdgConfigBox.TabIndex = 15;
            // 
            // SyringePumpBox
            // 
            this.SyringePumpBox.AutoSize = true;
            this.SyringePumpBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SyringePumpBox.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // SyringePumpBox.Flow
            // 
            this.SyringePumpBox.Flow.AutoSize = true;
            this.SyringePumpBox.Flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SyringePumpBox.Flow.Controls.Add(this.panel1);
            this.SyringePumpBox.Flow.Controls.Add(this.panel3);
            this.SyringePumpBox.Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SyringePumpBox.Flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SyringePumpBox.Flow.Location = new System.Drawing.Point(3, 16);
            this.SyringePumpBox.Flow.Name = "Flow";
            this.SyringePumpBox.Flow.Size = new System.Drawing.Size(294, 101);
            this.SyringePumpBox.Flow.TabIndex = 0;
            this.SyringePumpBox.Location = new System.Drawing.Point(0, 0);
            this.SyringePumpBox.Name = "SyringePumpBox";
            this.SyringePumpBox.SelectedObject = null;
            this.SyringePumpBox.Size = new System.Drawing.Size(300, 120);
            this.SyringePumpBox.TabIndex = 0;
            this.SyringePumpBox.Text = "Syringe Pump";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.SyringePumpAlways);
            this.panel1.Controls.Add(this.SyringePumpTs);
            this.panel1.Controls.Add(this.SyringePumpNever);
            this.panel1.Location = new System.Drawing.Point(3, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(198, 30);
            this.panel1.TabIndex = 1;
            // 
            // SyringePumpAlways
            // 
            this.SyringePumpAlways.AutoSize = true;
            this.SyringePumpAlways.Location = new System.Drawing.Point(137, 10);
            this.SyringePumpAlways.Name = "SyringePumpAlways";
            this.SyringePumpAlways.Size = new System.Drawing.Size(58, 17);
            this.SyringePumpAlways.TabIndex = 2;
            this.SyringePumpAlways.TabStop = true;
            this.SyringePumpAlways.Text = "Always";
            this.SyringePumpAlways.UseVisualStyleBackColor = true;
            // 
            // SyringePumpTs
            // 
            this.SyringePumpTs.AutoSize = true;
            this.SyringePumpTs.Location = new System.Drawing.Point(68, 10);
            this.SyringePumpTs.Name = "SyringePumpTs";
            this.SyringePumpTs.Size = new System.Drawing.Size(63, 17);
            this.SyringePumpTs.TabIndex = 1;
            this.SyringePumpTs.TabStop = true;
            this.SyringePumpTs.Text = "TS Only";
            this.SyringePumpTs.UseVisualStyleBackColor = true;
            // 
            // SyringePumpNever
            // 
            this.SyringePumpNever.AutoSize = true;
            this.SyringePumpNever.Checked = true;
            this.SyringePumpNever.Location = new System.Drawing.Point(8, 10);
            this.SyringePumpNever.Name = "SyringePumpNever";
            this.SyringePumpNever.Size = new System.Drawing.Size(54, 17);
            this.SyringePumpNever.TabIndex = 0;
            this.SyringePumpNever.TabStop = true;
            this.SyringePumpNever.Text = "Never";
            this.SyringePumpNever.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.DiscardSyringe);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(3, 72);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(109, 26);
            this.panel3.TabIndex = 4;
            // 
            // DiscardSyringe
            // 
            this.DiscardSyringe.Location = new System.Drawing.Point(71, 3);
            this.DiscardSyringe.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.DiscardSyringe.Name = "DiscardSyringe";
            this.DiscardSyringe.Size = new System.Drawing.Size(35, 20);
            this.DiscardSyringe.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Discard first";
            // 
            // TimeProgress
            // 
            this.TimeProgress.Location = new System.Drawing.Point(169, 56);
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
            // ExperimentConfigBox
            // 
            this.ExperimentConfigBox.AutoSize = true;
            this.ExperimentConfigBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ExperimentConfigBox.Controls.Add(this.label9);
            this.ExperimentConfigBox.Controls.Add(this.DiscardLaser);
            this.ExperimentConfigBox.Controls.Add(this.GsDelay);
            this.ExperimentConfigBox.Controls.Add(this.label3);
            this.ExperimentConfigBox.Location = new System.Drawing.Point(369, 14);
            this.ExperimentConfigBox.MinimumSize = new System.Drawing.Size(500, 60);
            this.ExperimentConfigBox.Name = "ExperimentConfigBox";
            this.ExperimentConfigBox.Size = new System.Drawing.Size(500, 63);
            this.ExperimentConfigBox.TabIndex = 18;
            this.ExperimentConfigBox.TabStop = false;
            this.ExperimentConfigBox.Text = "Experiment Configuraton";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(215, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(125, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Collect pump only for N : ";
            // 
            // DiscardLaser
            // 
            this.DiscardLaser.Location = new System.Drawing.Point(346, 24);
            this.DiscardLaser.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.DiscardLaser.Name = "DiscardLaser";
            this.DiscardLaser.Size = new System.Drawing.Size(35, 20);
            this.DiscardLaser.TabIndex = 2;
            // 
            // GsDelay
            // 
            this.GsDelay.Location = new System.Drawing.Point(126, 24);
            this.GsDelay.Name = "GsDelay";
            this.GsDelay.Size = new System.Drawing.Size(65, 20);
            this.GsDelay.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Ground State Delay (s)";
            // 
            // schemeBox
            // 
            this.schemeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.schemeBox.FormattingEnabled = true;
            this.schemeBox.Location = new System.Drawing.Point(151, 69);
            this.schemeBox.Name = "schemeBox";
            this.schemeBox.Size = new System.Drawing.Size(121, 21);
            this.schemeBox.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Collection Scheme : ";
            // 
            // TroaControl
            // 
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "TroaControl";
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
            this.SyringePumpBox.Flow.ResumeLayout(false);
            this.SyringePumpBox.Flow.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DiscardSyringe)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ExperimentConfigBox.ResumeLayout(false);
            this.ExperimentConfigBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DiscardLaser)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadTimes;
        private System.Windows.Forms.DataGridView TimesView;
        private System.Windows.Forms.Button SaveData;
        private controls.DdgCommandPanel DdgConfigBox;
        private controls.ObjectCommandPanel SyringePumpBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton SyringePumpAlways;
        private System.Windows.Forms.RadioButton SyringePumpTs;
        private System.Windows.Forms.RadioButton SyringePumpNever;
        protected System.Windows.Forms.TextBox TimeProgress;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.GroupBox ExperimentConfigBox;
        private System.Windows.Forms.TextBox GsDelay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.NumericUpDown DiscardSyringe;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox schemeBox;
        private System.Windows.Forms.NumericUpDown DiscardLaser;
        private System.Windows.Forms.Label label9;
    }
}
