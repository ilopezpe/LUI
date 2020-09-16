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
            this.SaveData = new System.Windows.Forms.Button();
            this.DdgConfigBox = new LUI.controls.DdgCommandPanel();
            this.SyringePumpBox = new LUI.controls.ObjectCommandPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SyringePumpAlways = new System.Windows.Forms.RadioButton();
            this.SyringePumpTs = new System.Windows.Forms.RadioButton();
            this.SyringePumpNever = new System.Windows.Forms.RadioButton();
            this.Discard = new System.Windows.Forms.CheckBox();
            this.TimeProgress = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.LeftChildArea.Controls.Add(this.panel2);
            this.LeftChildArea.Controls.Add(this.DdgConfigBox);
            this.LeftChildArea.Location = new System.Drawing.Point(0, 583);
            this.LeftChildArea.Size = new System.Drawing.Size(1176, 238);
            // 
            // RightChildArea
            // 
            this.RightChildArea.Controls.Add(this.SyringePumpBox);
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
            this.DdgConfigBox.Location = new System.Drawing.Point(3, 3);
            this.DdgConfigBox.Margin = new System.Windows.Forms.Padding(2);
            this.DdgConfigBox.Name = "DdgConfigBox";
            this.DdgConfigBox.Size = new System.Drawing.Size(362, 59);
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
            this.SyringePumpBox.Flow.Controls.Add(this.Discard);
            this.SyringePumpBox.Flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SyringePumpBox.Flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SyringePumpBox.Flow.Location = new System.Drawing.Point(3, 16);
            this.SyringePumpBox.Flow.Name = "Flow";
            this.SyringePumpBox.Flow.Size = new System.Drawing.Size(294, 92);
            this.SyringePumpBox.Flow.TabIndex = 0;
            this.SyringePumpBox.Location = new System.Drawing.Point(0, 0);
            this.SyringePumpBox.Name = "SyringePumpBox";
            this.SyringePumpBox.SelectedObject = null;
            this.SyringePumpBox.Size = new System.Drawing.Size(300, 111);
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
            // Discard
            // 
            this.Discard.AutoSize = true;
            this.Discard.Location = new System.Drawing.Point(3, 72);
            this.Discard.Name = "Discard";
            this.Discard.Size = new System.Drawing.Size(84, 17);
            this.Discard.TabIndex = 2;
            this.Discard.Text = "Discard First";
            this.Discard.UseVisualStyleBackColor = true;
            // 
            // TimeProgress
            // 
            this.TimeProgress.Location = new System.Drawing.Point(151, 37);
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
            this.label2.Location = new System.Drawing.Point(110, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Times";
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
            this.panel2.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox Discard;
        protected System.Windows.Forms.TextBox TimeProgress;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;

    }
}
