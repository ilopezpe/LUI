﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using lasercom.camera;
using lasercom.ddg;
using LUI.config;
using lasercom;
using lasercom.io;
using System.IO;
using LUI.controls;

namespace LUI.tabs
{
    public partial class TroaControl : LUI.tabs.LuiTab
    {
        public class RoleRow : INotifyPropertyChanged
        {
            private string _Role;
            public string Role
            {
                get
                {
                    return _Role;
                }
                set
                {
                    _Role = value;
                    NotifyPropertyChanged();
                }
            }

            private DelayGeneratorParameters _DDG;
            public DelayGeneratorParameters DDG
            {
                get
                {
                    return _DDG;
                }
                set
                {
                    _DDG = value;
                    NotifyPropertyChanged();
                }
            }

            private string _Delay;
            public string Delay
            {
                get
                {
                    return _Delay;
                }
                set
                {
                    if (value != _Delay)
                    {
                        _Delay = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string _Trigger;
            public string Trigger
            {
                get
                {
                    return _Trigger;
                }
                set
                {
                    _Trigger = value;
                    NotifyPropertyChanged();
                }
            }

            private double _DelayValue;
            public double DelayValue
            {
                get
                {
                    return _DelayValue;
                }
                set
                {
                    _DelayValue = value;
                    NotifyPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public static explicit operator RoleRow(DataRow dr)
            {
                RoleRow p = new RoleRow();
                p.Role = (string)dr.ItemArray[0];
                p.DDG = (DelayGeneratorParameters)dr.ItemArray[1];
                p.Delay = (string)dr.ItemArray[2];
                p.Trigger = (string)dr.ItemArray[3];
                p.DelayValue = (double)dr.ItemArray[4];
                return p;
            }

            public static explicit operator RoleRow(DataGridViewRow row)
            {
                return new RoleRow()
                {
                    Role = (string)row.Cells["Role"].Value,
                    DDG = (DelayGeneratorParameters)row.Cells["DDG"].Value,
                    Delay = (string)row.Cells["Delay"].Value,
                    Trigger = (string)row.Cells["Trigger"].Value,
                    DelayValue = (double)row.Cells["DelayValue"].Value
                };
            }
        }

        public class TimesRow : INotifyPropertyChanged
        {
            private double _Value;
            public double Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                    NotifyPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public static explicit operator TimesRow(DataRow dr)
            {
                TimesRow p = new TimesRow();
                p.Value = (double)dr.ItemArray[0];
                return p;
            }
            public static explicit operator TimesRow(DataGridViewRow row)
            {
                return new TimesRow()
                {
                    Value = (double)row.Cells["Value"].Value,
                };
            }
        }

        struct WorkArgs
        {
            public WorkArgs(int N, IList<double> Times, RoleRow PrimaryDelay, RoleRow Gate)
            {
                this.N = N;
                this.Times = new List<double>(Times);
                this.PrimaryDelayName = PrimaryDelay.Delay[0];
                this.TriggerName = PrimaryDelay.Trigger[0];
                //this.GateName = new Tuple<char, char>(Gate.Delay[0], Gate.Delay[1]);
                //this.GateTriggerName = Gate.Trigger[0];
                //this.Gate = Gate.DelayValue;
                //this.GateDelay = Gate.DelayValue;
                this.GateName = null;
                this.GateTriggerName = '\0';
                this.Gate = double.NaN;
                this.GateDelay = double.NaN;
            }
            public readonly int N;
            public readonly IList<double> Times;
            public readonly char PrimaryDelayName;
            public readonly char TriggerName;
            public readonly Tuple<char,char> GateName;
            public readonly char GateTriggerName;
            public readonly double GateDelay;
            public readonly double Gate;
        }

        struct ProgressObject
        {
            public ProgressObject(double[] Data, string CameraStatus, double Delay, Dialog Status)
            {
                this.Data = Data;
                this.CameraStatus = CameraStatus;
                this.Delay = Delay;
                this.Status = Status;
            }
            public readonly double[] Data;
            public readonly string CameraStatus;
            public readonly double Delay;
            public readonly Dialog Status;
        }

        IList<double> Times
        {
            get
            {
                return TimesList.Select(x => x.Value).ToList();
            }
            set
            {
                TimesList.Clear();
                foreach (double d in value)
                    TimesList.Add(new TimesRow() { Value = d });
            }
        }

        public enum Dialog
        {
            INITIALIZE, PROGRESS, PROGRESS_DARK, PROGRESS_TIME, 
            PROGRESS_TIME_COMPLETE, PROGRESS_FLASH, PROGRESS_TRANS,
            CALCULATE
        }

        private BindingList<RoleRow> RoleList = new BindingList<RoleRow>();
        private BindingList<TimesRow> TimesList = new BindingList<TimesRow>();
        RoleRow PrimaryDelay;
        RoleRow Gate;
        MatFile DataFile;
        MatVar<int> RawData;
        MatVar<double> LuiData;

        public TroaControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
            Init();

            TimesList.AllowEdit = true;
            TimesView.DefaultValuesNeeded += (sender, e) => { e.Row.Cells["Value"].Value = 0; };
            TimesView.DataSource = new BindingSource(TimesList, null);
            TimesView.CellValidating += TimesView_CellValidating;
            TimesView.CellEndEdit += TimesView_CellEndEdit;
            
            RoleList.AllowEdit = true;
            RoleListView.DefaultValuesNeeded += DDGListView_DefaultValuesNeeded;
            RoleListView.DataSource = new BindingSource(RoleList, null);

            PrimaryDelay = new RoleRow();
            PrimaryDelay.Role = "Primary Delay";
            RoleList.Add(PrimaryDelay);

            Gate = new RoleRow();
            Gate.Role = "Gate";
            //RoleList.Add(Gate);

            PrimaryDelay.PropertyChanged += Role_PropertyChanged;
            Gate.PropertyChanged += Role_PropertyChanged;

            SaveData.Click += (sender, e) => SaveOutput();
        }

        private void Init()
        {
            SuspendLayout();
            SaveData.Enabled = false;
            //DataGridViewComboBoxColumn Role = (DataGridViewComboBoxColumn)RoleListView.Columns["Role"];
            //Role.Items.Add("Primary Delay");
            //Role.Items.Add("Gate");
            DataGridViewComboBoxColumn DDG = (DataGridViewComboBoxColumn)RoleListView.Columns["DDG"];
            DDG.DisplayMember = "Name";
            DDG.ValueMember = "Self";

            ResumeLayout();
        }

        /// <summary>
        /// Create temporary MAT file and initialize variables.
        /// </summary>
        /// <param name="NumChannels"></param>
        /// <param name="NumScans"></param>
        /// <param name="NumTimes"></param>
        private void InitDataFile(int NumChannels, int NumScans, int NumTimes)
        {
            string TempFileName = Path.GetTempFileName();
            TempFileName = TempFileName.Replace(".tmp", ".mat");
            DataFile = new MatFile(TempFileName);
            RawData = DataFile.CreateVariable<int>("rawdata", NumScans, NumChannels);
            LuiData = DataFile.CreateVariable<double>("luidata", NumTimes + 1, NumChannels + 1);
        }

        protected override void Graph_Click(object sender, MouseEventArgs e)
        {
            var NormalizedCoords = Graph.AxesToNormalized(Graph.ScreenToAxes(new Point(e.X, e.Y)));
            int SelectedChannel = (int)Math.Round(NormalizedCoords.X * (Commander.Camera.Width - 1));
            float Y = NormalizedCoords.Y;
            Graph.ClearAnnotation();
            Graph.Annotate(GraphControl.Annotation.VERTLINE, Graph.MarkerColor, SelectedChannel);
            Graph.Annotate(GraphControl.Annotation.HORZLINE, Graph.MarkerColor, Y);
            Graph.Invalidate();
        }

        protected override void Collect_Click(object sender, EventArgs e)
        {
            Collect.Enabled = NScan.Enabled = CameraGain.Enabled = false;
            Abort.Enabled = true;

            CameraStatus.Text = "";

            Graph.ClearData();
            Graph.Invalidate();

            int N = (int)NScan.Value;
            worker = new BackgroundWorker();
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(WorkProgress);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(WorkComplete);
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync(new WorkArgs(N, Times, PrimaryDelay, Gate));
        }

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            var progress = new ProgressObject(null, null, 0, Dialog.INITIALIZE);
            worker.ReportProgress(0, progress); // Show zero progress.

            // Set camera for external gate and full vertical binning.
            if (Commander.Camera is AndorCamera)
            {
                Commander.Camera.AcquisitionMode = AndorCamera.AcquisitionModeSingle;
                Commander.Camera.TriggerMode = AndorCamera.TriggerModeExternalExposure;
                Commander.Camera.DDGTriggerMode = AndorCamera.DDGTriggerModeExternal;
                Commander.Camera.ReadMode = AndorCamera.ReadModeFVB;
            }

            var args = (WorkArgs)e.Argument;
            int N = args.N; // Save typing for later.
            int half = N / 2; // Integer division rounds down.
            IList<double> Times = args.Times;

            // Total scans = dark scans + ground state scans + plus time series scans.
            int TotalScans = 2*N + Times.Count * N;

            // Create the data store.
            InitDataFile((int)Commander.Camera.AcqSize, TotalScans, Times.Count);

            // Measure dark current.
            progress = new ProgressObject(null, null, 0, Dialog.PROGRESS_DARK);
            worker.ReportProgress(0, progress);

            int[] DarkBuffer = new int[Commander.Camera.AcqSize];
            int[] Dark = new int[Commander.Camera.AcqSize];
            for (int i = 0; i < N; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                uint ret = Commander.Dark(DarkBuffer);
                RawData.WriteNext(DarkBuffer, 0);
                Data.Accumulate(Dark, DarkBuffer);

                progress = new ProgressObject(null, Commander.Camera.DecodeStatus(ret), 0, Dialog.PROGRESS_DARK);
                worker.ReportProgress((i + 1) * 99 / TotalScans, progress);
            }
            Data.DivideArray(Dark, N); // Average dark current.

            // Buffer for acuisition data.
            int[] DataBuffer = new int[Commander.Camera.AcqSize];
            double[] Ground = new double[Commander.Camera.AcqSize];

            // Ground state scans - first half.
            for (int i = 0; i < half; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                uint ret = Commander.Flash(DataBuffer);
                RawData.WriteNext(DataBuffer, 0);
                Data.Accumulate(Ground, DataBuffer);

                progress = new ProgressObject(null, Commander.Camera.DecodeStatus(ret), 0, Dialog.PROGRESS_FLASH);
                worker.ReportProgress((N + (i+1)) * 99 / TotalScans, progress); // Handle new data.
            }
            Data.DivideArray(Ground, half); // Average GS for first half.
            Data.Dissipate(Ground, Dark); // Subtract average dark from average GS.

            // Excited state buffer.
            double[] Excited = new double[Commander.Camera.AcqSize];

            // Excited state scans.
            for (int i = 0; i < Times.Count; i++)
            {
                double Delay = Times[i];
                progress = new ProgressObject(null, null, Delay, Dialog.PROGRESS_TIME);
                worker.ReportProgress((half + i * N) * 99 / TotalScans, progress); // Display current delay.
                for (int j = 0; j < N; j++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay); // Set delay time.
                    
                    uint ret = Commander.Trans(DataBuffer);
                    RawData.WriteNext(DataBuffer, 0);
                    Data.Accumulate(Excited, DataBuffer);

                    progress = new ProgressObject(null, Commander.Camera.DecodeStatus(ret), Delay, Dialog.PROGRESS_TRANS);
                    worker.ReportProgress( (N + half + (i+1) * (j+1)) * 99 / TotalScans , progress); // Handle new data.
                }
                Data.DivideArray(Excited, N); // Average ES for time point.
                Data.Dissipate(Excited, Dark); // Subtract average dark from time point average.
                double[] Difference = Data.DeltaOD(Ground, Excited); // Time point diff. spec. w/ current GS average.
                progress = new ProgressObject(Difference, null, Delay, Dialog.PROGRESS_TIME_COMPLETE);
                worker.ReportProgress((N + half + N * Times.Count) * 99 / TotalScans, progress);
            }

            // Ground state scans - second half.
            int half2 = N % 2 == 0 ? half : half + 1; // If N is odd, need 1 more GS scan in the second half.

            for (int i = 0; i < half2; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                uint ret = Commander.Flash(DataBuffer);
                RawData.WriteNext(DataBuffer, 0);

                progress = new ProgressObject(null, Commander.Camera.DecodeStatus(ret), 0, Dialog.PROGRESS_FLASH);
                worker.ReportProgress( (N + half + (N * Times.Count) + (i+1)) * 99 / TotalScans, progress);
            }

            // Calculate LuiData matrix
            progress = new ProgressObject(null, null, 0, Dialog.CALCULATE);
            worker.ReportProgress(99, progress);
            // Write dummy value.
            LuiData.Write(-8D, new long[] { 0, 0 });
            // Write wavelengths.
            long[] RowSize = { 1, Commander.Camera.AcqSize };
            LuiData.Write(Commander.Camera.Calibration, new long[] { 0, 1 }, RowSize);
            // Write times.
            long[] ColSize = { Times.Count, 1 };
            LuiData.Write(Times.ToArray(), new long[] { 1, 0 }, ColSize);
            
            // Read ground state values and average.
            Ground.Initialize(); // Zero ground state buffer.
            // Read 1st half
            for (int i = 0; i < half; i++)
            {
                RawData.Read(DataBuffer, new long[] { i, 0 }, RowSize);
                Data.Accumulate(Ground, DataBuffer);
            }
            // Read 2nd half
            for (int i = (N + half + N * Times.Count); i < TotalScans; i++)
            {   
                RawData.Read(DataBuffer, new long[] { i, 0 }, RowSize);
                Data.Accumulate(Ground, DataBuffer);
            }
            Data.DivideArray(Ground, N); // Average ground state.
            Data.Dissipate(Ground, Dark); // Subtract average dark.

            // Read excited state values, average and compute delta OD.
            Excited.Initialize(); // Zero excited state buffer.
            for (int i = 0; i < Times.Count; i++ )
            {
                for (int j = 0; j < N; j++)
                {
                    // Read time point j
                    int idx = N + half + (i * N) + j;
                    RawData.Read(DataBuffer, new long[] { idx, 0 }, RowSize);
                    Data.Accumulate(Excited, DataBuffer);
                }
                Data.DivideArray(Excited, N); // Average excited state for time point.
                Data.Dissipate(Excited, Dark); // Subtract average dark.
                // Write the final difference spectrum for time point.
                LuiData.Write(Data.DeltaOD(Ground, Excited),
                    new long[] { i + 1, 1 }, RowSize);
            }

            // Done with everything.
        }

        protected override void WorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var progress = (ProgressObject)e.UserState;
            StatusProgress.Value = e.ProgressPercentage;
            switch (progress.Status)
            {
                case Dialog.INITIALIZE:
                    ProgressLabel.Text = "Initializing";
                    break;
                case Dialog.PROGRESS:
                    break;
                case Dialog.PROGRESS_DARK:
                    ProgressLabel.Text = "Collecting dark";
                    CameraStatus.Text = progress.CameraStatus;
                    break;
                case Dialog.PROGRESS_TIME:
                    PrimaryDelay.DelayValue = progress.Delay;
                    break;
                case Dialog.PROGRESS_TIME_COMPLETE:
                    Display(progress.Data);
                    break;
                case Dialog.PROGRESS_FLASH:
                    ProgressLabel.Text = "Collecting ground";
                    CameraStatus.Text = progress.CameraStatus;
                    break;
                case Dialog.PROGRESS_TRANS:
                    ProgressLabel.Text = "Collecting transient";
                    CameraStatus.Text = progress.CameraStatus;
                    break;
                case Dialog.CALCULATE:
                    ProgressLabel.Text = "Calculating...";
                    break;
            }
        }

        protected override void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Handle the exception thrown in the worker thread.
                MessageBox.Show(e.Error.ToString());
            }
            else if (e.Cancelled)
            {
                ProgressLabel.Text = "Aborted";
            }
            else
            {
                SaveOutput();
                ProgressLabel.Text = "Complete";
            }

            // Ensure the temp file is always closed.
            if (DataFile != null) DataFile.Close();

            SaveData.Enabled = true;

            StatusProgress.Value = 100;
            Collect.Enabled = NScan.Enabled = CameraGain.Enabled = true;
            Abort.Enabled = false;
        }

        private void Display(double[] Y)
        {
            Graph.DrawPoints(Commander.Camera.Calibration, Y);
            Graph.Invalidate();
            Graph.MarkerColor = Graph.NextColor;
        }

        private void DDGListView_DefaultValuesNeeded(object sender,
            System.Windows.Forms.DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Role"] = null;
            e.Row.Cells["Delay"].Value = null;
            e.Row.Cells["Trigger"] = null;
            e.Row.Cells["DelayValue"].Value = 0.0;
        }

        public override void HandleParametersChanged(object sender, EventArgs e)
        {
            base.HandleParametersChanged(sender, e); // Takes care of ObjectSelectPanel.

            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)RoleListView.Columns["DDG"];
            col.Items.Clear();
            var parameters = Config.GetParameters(typeof(DelayGeneratorParameters));
            foreach (var p in parameters)
            {
                col.Items.Add(p);
            }
        }

        /// <summary>
        /// Called when any property of any role row is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Role_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var row = (RoleRow)sender;
            var dgvRow = RoleListView.Rows[FindRowByRoleName(row.Role)];
            if (e.PropertyName == "DDG") // Changed selected DDG
            {
                IDigitalDelayGenerator DDG = (IDigitalDelayGenerator)Config.GetObject(row.DDG);
                // Re-populate the available delay and trigger choices.
                var cell = (DataGridViewComboBoxCell)dgvRow.Cells["Delay"];
                cell.Items.Clear();
                if (row == PrimaryDelay)
                {
                    Commander.DDG = DDG;
                    foreach (string d in DDG.Delays) cell.Items.Add(d);
                }
                else if (row == Gate)
                {
                    foreach (string d in DDG.DelayPairs) cell.Items.Add(d);
                }
                cell = (DataGridViewComboBoxCell)dgvRow.Cells["Trigger"];
                cell.Items.Clear();
                foreach (string d in DDG.Triggers) cell.Items.Add(d);
            }
            else if (e.PropertyName == "Delay" || e.PropertyName == "Trigger" || e.PropertyName == "DelayValue")
            {
                // Get row.DDG object, set object's row.Delay = row.Trigger + row.DelayValue
            }
        }

        /// <summary>
        /// Retrieve row for a role by name (e.g. "PrimaryDelay").
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        private int FindRowByRoleName(string searchValue)
        {
            int rowIndex = -1;

            DataGridViewRow row = RoleListView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells["Role"].Value.ToString().Equals(searchValue))
                .First();

            rowIndex = row.Index;

            return rowIndex;
        }

        /// <summary>
        /// Clears row error if user presses ESC while editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimesView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            TimesView.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        /// <summary>
        /// Validate that times entered by the user are legal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimesView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (TimesView.Columns[e.ColumnIndex].Name == "Value")
            {
                double value;
                if (!double.TryParse(e.FormattedValue.ToString(), out value))
                {
                    TimesView.Rows[e.RowIndex].ErrorText = "Time must be a number";
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Load file containing time delays (one per line).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadTimes_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text File|*.txt|All Files|*.*";
            openFile.Title = "Load Time Series File";
            openFile.ShowDialog();

            if (openFile.FileName == "") return;

            try
            {
                Times = FileIO.ReadTimesFile(openFile.FileName);
            }
            catch (IOException ex)
            {
                Log.Error(ex);
                MessageBox.Show("Couldn't load times file at " + openFile.FileName);
            }
            catch (FormatException ex)
            {
                Log.Error(ex);
                MessageBox.Show("Couldn't parse file: " + ex.Message);
            }
                
        }

        private void SaveOutput()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "MAT File|*.mat|CSV File|*.csv";
            saveFile.Title = "Data Data File";
            saveFile.ShowDialog();

            if (saveFile.FileName == "") return;

            switch (saveFile.FilterIndex)
            {
                case 1: // MAT file; just move temporary MAT file.
                    if (DataFile != null && !DataFile.Closed) DataFile.Close();
                    try
                    {
                        File.Copy(DataFile.FileName, saveFile.FileName);
                    }
                    catch (IOException ex)
                    {
                        Log.Error(ex);
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case 2: // CSV file; read LuiData to CSV file.
                    if (DataFile != null)
                    {
                        if (DataFile.Closed) DataFile.Reopen();
                        //MatVar<double> luiData = (MatVar<double>)DataFile["LuiData"];

                        if (!LuiData.Closed)
                        {
                            double[,] Matrix = new double[LuiData.Dims[0], LuiData.Dims[1]];
                            LuiData.Read(Matrix, new long[] { 0, 0 }, LuiData.Dims);
                            FileIO.WriteMatrix(saveFile.FileName, Matrix);
                        }

                        DataFile.Close();
                    }
                    break;
            }
        }
    }
}
