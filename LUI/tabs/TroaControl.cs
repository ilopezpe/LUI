using lasercom;
using lasercom.camera;
using lasercom.control;
using lasercom.ddg;
using lasercom.io;
using LUI.config;
using LUI.controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace LUI.tabs
{
    public partial class TroaControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_DARK,
            PROGRESS_TIME,
            PROGRESS_TIME_COMPLETE,
            PROGRESS_FLASH,
            PROGRESS_TRANS,
            CALCULATE,
            TEMPERATURE
        }

        public enum PumpMode
        {
            NEVER,
            TRANS,
            ALWAYS
        }

        MatFile DataFile;
        MatVar<double> LuiData;
        MatVar<int> RawData;

        readonly BindingList<TimesRow> TimesList = new BindingList<TimesRow>();

        public TroaControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
            Init();

            TimesList.AllowEdit = false;
            TimesView.DefaultValuesNeeded += (sender, e) => { e.Row.Cells["Value"].Value = 0; };
            TimesView.DataSource = new BindingSource(TimesList, null);
            TimesView.CellValidating += TimesView_CellValidating;
            TimesView.CellEndEdit += TimesView_CellEndEdit;

            SaveData.Click += (sender, e) => SaveOutput();

            DdgConfigBox.Config = Config;
            DdgConfigBox.Commander = Commander;
            DdgConfigBox.AllowZero = false;
            DdgConfigBox.HandleParametersChanged(this, EventArgs.Empty);
        }

        IList<double> Times
        {
            get { return TimesList.Select(x => x.Value).ToList(); }
            set
            {
                TimesList.Clear();
                foreach (var d in value)
                    TimesList.Add(new TimesRow { Value = d });
            }
        }

        void Init()
        {
            SuspendLayout();

            ScanProgress.Text = "0";
            TimeProgress.Text = "0";

            NScan.Minimum = 2;
            NScan.Increment = 2;
            NScan.ValueChanged += (sender, e) =>
            {
                if (NScan.Value % 2 != 0) NScan.Value += 1;
            };

            SaveData.Enabled = false;

            ResumeLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            PumpBox.ObjectChanged += HandlePumpChanged;
            base.OnLoad(e);
        }

        public override void HandleParametersChanged(object sender, EventArgs e)
        {
            base.HandleParametersChanged(sender, e); // Takes care of ObjectSelectPanel.
            DdgConfigBox.HandleParametersChanged(sender, e);

            var PumpsAvailable = Config.GetParameters(typeof(PumpParameters));
            if (PumpsAvailable.Count() > 0)
            {
                var selectedPump = PumpBox.SelectedObject;
                PumpBox.Objects.Items.Clear();
                foreach (var p in PumpsAvailable)
                    PumpBox.Objects.Items.Add(p);
                // One of next two lines will trigger CameraChanged event.
                PumpBox.SelectedObject = selectedPump;
                if (PumpBox.Objects.SelectedItem == null) PumpBox.Objects.SelectedIndex = 0;
                PumpBox.Enabled = true;
            }
            else
            {
                PumpBox.Enabled = false;
            }
        }

        public override void HandleContainingTabSelected(object sender, EventArgs e)
        {
            base.HandleContainingTabSelected(sender, e);
            DdgConfigBox.UpdatePrimaryDelayValue();
        }

        public virtual void HandlePumpChanged(object sender, EventArgs e)
        {
            if (Commander.Pump != null) Commander.Pump.SetClosed();
            Commander.Pump = (IPump)Config.GetObject(PumpBox.SelectedObject);
        }

        protected override void LoadSettings()
        {
            base.LoadSettings();
            var Settings = Config.TabSettings[GetType().Name];
            if (Settings.TryGetValue("PrimaryDelayDdg", out var value) && value != null && value != "")
                DdgConfigBox.PrimaryDelayDdg = (DelayGeneratorParameters)Config.GetFirstParameters(
                    typeof(DelayGeneratorParameters), value);

            if (Settings.TryGetValue("PrimaryDelayDelay", out value) && value != null && value != "")
                DdgConfigBox.PrimaryDelayDelay = value;
        }

        protected override void SaveSettings()
        {
            base.SaveSettings();
            var Settings = Config.TabSettings[GetType().Name];
            Settings["PrimaryDelayDdg"] = DdgConfigBox.PrimaryDelayDdg?.Name;
            Settings["PrimaryDelayDelay"] = DdgConfigBox.PrimaryDelayDelay ?? null;
        }

        /// <summary>
        ///     Create temporary MAT file and initialize variables.
        /// </summary>
        /// <param name="NumChannels"></param>
        /// <param name="NumScans"></param>
        /// <param name="NumTimes"></param>
        void InitDataFile(int NumChannels, int NumScans, int NumTimes)
        {
            var TempFileName = Path.GetTempFileName();
            DataFile = new MatFile(TempFileName);
            RawData = DataFile.CreateVariable<int>("rawdata", NumScans, NumChannels);
            LuiData = DataFile.CreateVariable<double>("luidata", NumTimes + 1, NumChannels + 1);
        }

        protected override void Graph_Click(object sender, MouseEventArgs e)
        {
            var NormalizedCoords = Graph.AxesToNormalized(Graph.ScreenToAxes(new Point(e.X, e.Y)));
            var SelectedChannel = Commander.Camera.CalibrationAscending
                ? (int)Math.Round(NormalizedCoords.X * (Commander.Camera.Width - 1))
                : (int)Math.Round((1 - NormalizedCoords.X) * (Commander.Camera.Width - 1));
            var X = Commander.Camera.Calibration[SelectedChannel];
            var Y = NormalizedCoords.Y;
            Graph.ClearAnnotation();
            Graph.Annotate(GraphControl.Annotation.VERTLINE, Graph.MarkerColor, X);
            Graph.Annotate(GraphControl.Annotation.HORZLINE, Graph.MarkerColor, Y);
            Graph.Invalidate();
        }

        protected override void Collect_Click(object sender, EventArgs e)
        {
            if (DdgConfigBox.PrimaryDelayDdg == null || DdgConfigBox.PrimaryDelayDelay == null)
            {
                MessageBox.Show("Primary delay must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            if (Times.Count < 1)
            {
                MessageBox.Show("Time delay series must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            CameraStatus.Text = "";

            Graph.ClearData();
            Graph.Invalidate();

            var N = (int)NScan.Value;
            PumpMode Mode;
            if (PumpNever.Checked) Mode = PumpMode.NEVER;
            else if (PumpTs.Checked) Mode = PumpMode.TRANS;
            else Mode = PumpMode.ALWAYS;

            Commander.BeamFlag.CloseLaserAndFlash();

            SetupWorker();
            worker.RunWorkerAsync(new WorkArgs(N, Times, DdgConfigBox.PrimaryDelayDelay,
                DdgConfigBox.PrimaryDelayTrigger, Mode, Discard.Checked));
            OnTaskStarted(EventArgs.Empty);
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            DdgConfigBox.Enabled = false;
            PumpBox.Enabled = false;
            LoadTimes.Enabled = SaveData.Enabled = false;
            ScanProgress.Text = "0";
            TimeProgress.Text = "0";
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            DdgConfigBox.Enabled = true;
            PumpBox.Enabled = true;
            LoadTimes.Enabled = SaveData.Enabled = true;
        }

        [Obsolete("Deprecated, use DoWork instead.", true)]
        protected void DoWorkOld(object sender, DoWorkEventArgs e)
        {
            ProgressObject progress;

            progress = new ProgressObject(null, 0, Dialog.TEMPERATURE);
            DoTempCheck(() => PauseCancelProgress(e, 0, progress));

            progress = new ProgressObject(null, 0, Dialog.INITIALIZE);
            if (PauseCancelProgress(e, 0, progress)) return; // Show zero progress.

            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var half = N / 2; // Integer division rounds down.
            var Times = args.Times;
            var AcqSize = Commander.Camera.AcqSize;
            var finalSize = Commander.Camera.ReadMode == AndorCamera.ReadModeImage
                ? AcqSize / Commander.Camera.Image.Height
                : AcqSize;

            // Total scans = dark scans + ground state scans + plus time series scans.
            var TotalScans = 2 * N + Times.Count * N;

            // Create the data store.
            InitDataFile(finalSize, TotalScans, Times.Count);

            // Measure dark current.
            progress = new ProgressObject(null, 0, Dialog.PROGRESS_DARK);
            if (PauseCancelProgress(e, 0, progress)) return;

            // Buffer for acuisition data.
            var DataBuffer = new int[AcqSize];
            var DataRow = new int[finalSize];

            // Dark buffers.
            var Dark = new int[finalSize];

            // Dark scans.
            Commander.BeamFlag.CloseLaserAndFlash();
            for (var i = 0; i < N; i++)
            {
                TryAcquire(DataBuffer);
                Data.ColumnSum(DataRow, DataBuffer);
                RawData.WriteNext(DataRow, 0);
                Data.Accumulate(Dark, DataRow);
                Array.Clear(DataRow, 0, finalSize);
                progress = new ProgressObject(null, 0, Dialog.PROGRESS_DARK);
                if (PauseCancelProgress(e, (i + 1) * 99 / TotalScans, progress)) return;
            }

            Data.DivideArray(Dark, N); // Average dark current.

            // Set delays for GS.
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, 3.2E-8); // Set delay time.

            var Ground = new double[finalSize];

            // Flow-flash.
            if (args.Pump == PumpMode.ALWAYS)
            {
                Commander.Pump.SetOpen();
                if (args.DiscardFirst) TryAcquire(DataBuffer);
            }

            // Ground state scans - first half.
            Commander.BeamFlag.OpenFlash();
            for (var i = 0; i < half; i++)
            {
                TryAcquire(DataBuffer);
                Data.ColumnSum(DataRow, DataBuffer);
                RawData.WriteNext(DataRow, 0);
                Data.Accumulate(Ground, DataRow);
                Array.Clear(DataRow, 0, finalSize);
                progress = new ProgressObject(null, 0, Dialog.PROGRESS_FLASH);
                if (PauseCancelProgress(e, (N + i + 1) * 99 / TotalScans, progress)) return; // Handle new data.
            }

            Commander.BeamFlag.CloseLaserAndFlash();

            Data.DivideArray(Ground, half); // Average GS for first half.
            Data.Dissipate(Ground, Dark); // Subtract average dark from average GS.

            // Excited state buffer.
            var Excited = new double[finalSize];

            // Flow-flash.
            if (args.Pump == PumpMode.TRANS)
            {
                Commander.Pump.SetOpen();
                if (args.DiscardFirst) TryAcquire(DataBuffer);
            }

            // Excited state scans.
            Commander.BeamFlag.OpenLaserAndFlash();
            for (var i = 0; i < Times.Count; i++)
            {
                var Delay = Times[i];
                progress = new ProgressObject(null, Delay, Dialog.PROGRESS_TIME);
                if (PauseCancelProgress(e, (half + i * N) * 99 / TotalScans, progress))
                    return; // Display current delay.
                for (var j = 0; j < N; j++)
                {
                    Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay); // Set delay time.

                    TryAcquire(DataBuffer);
                    Data.ColumnSum(DataRow, DataBuffer);
                    RawData.WriteNext(DataRow, 0);
                    Data.Accumulate(Excited, DataRow);
                    Array.Clear(DataRow, 0, finalSize);
                    progress = new ProgressObject(null, Delay, Dialog.PROGRESS_TRANS);
                    if (PauseCancelProgress(e, (N + half + (i + 1) * (j + 1)) * 99 / TotalScans, progress))
                        return; // Handle new data.
                }

                Data.DivideArray(Excited, N); // Average ES for time point.
                Data.Dissipate(Excited, Dark); // Subtract average dark from time point average.
                var Difference = Data.DeltaOD(Ground, Excited); // Time point diff. spec. w/ current GS average.
                Array.Clear(Excited, 0, finalSize);
                progress = new ProgressObject(Difference, Delay, Dialog.PROGRESS_TIME_COMPLETE);
                if (PauseCancelProgress(e, (N + half + N * Times.Count) * 99 / TotalScans, progress)) return;
            }

            Commander.BeamFlag.CloseLaserAndFlash();

            // Set delays for GS.
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, 3.2E-8); // Set delay time.

            // Flow-flash.
            if (args.Pump == PumpMode.TRANS) // Could close pump before last collect.
                Commander.Pump.SetClosed();

            // Ground state scans - second half.
            Commander.BeamFlag.OpenFlash();
            var half2 = N % 2 == 0 ? half : half + 1; // If N is odd, need 1 more GS scan in the second half.
            for (var i = 0; i < half2; i++)
            {
                TryAcquire(DataBuffer);
                Data.ColumnSum(DataRow, DataBuffer);
                RawData.WriteNext(DataRow, 0);
                Array.Clear(DataRow, 0, finalSize);
                progress = new ProgressObject(null, 0, Dialog.PROGRESS_FLASH);
                if (PauseCancelProgress(e, (N + half + N * Times.Count + i + 1) * 99 / TotalScans, progress)) return;
            }

            Commander.BeamFlag.CloseLaserAndFlash();

            // Flow-flash.
            if (args.Pump == PumpMode.ALWAYS) Commander.Pump.SetClosed(); // Could close pump before last collect.

            // Calculate LuiData matrix
            progress = new ProgressObject(null, 0, Dialog.CALCULATE);
            if (PauseCancelProgress(e, 99, progress)) return;
            // Write dummy value (number of scans).
            LuiData.Write(args.N, new long[] { 0, 0 });
            // Write wavelengths.
            long[] RowSize = { 1, finalSize };
            LuiData.Write(Commander.Camera.Calibration, new long[] { 0, 1 }, RowSize);
            // Write times.
            long[] ColSize = { Times.Count, 1 };
            LuiData.Write(Times.ToArray(), new long[] { 1, 0 }, ColSize);

            // Read ground state values and average.
            Array.Clear(Ground, 0, Ground.Length); // Zero ground state buffer.
            // Read 1st half
            for (var i = 0; i < half; i++)
            {
                RawData.Read(DataRow, new long[] { i, 0 }, RowSize);
                Data.Accumulate(Ground, DataRow);
            }

            // Read 2nd half
            for (var i = N + half + N * Times.Count; i < TotalScans; i++)
            {
                RawData.Read(DataRow, new long[] { i, 0 }, RowSize);
                Data.Accumulate(Ground, DataRow);
            }

            Data.DivideArray(Ground, N); // Average ground state.
            Data.Dissipate(Ground, Dark); // Subtract average dark.

            // Read excited state values, average and compute delta OD.
            Array.Clear(Excited, 0, Excited.Length); // Zero excited state buffer.
            for (var i = 0; i < Times.Count; i++)
            {
                for (var j = 0; j < N; j++)
                {
                    // Read time point j
                    var idx = N + half + i * N + j;
                    RawData.Read(DataRow, new long[] { idx, 0 }, RowSize);
                    Data.Accumulate(Excited, DataRow);
                }

                Data.DivideArray(Excited, N); // Average excited state for time point.
                Data.Dissipate(Excited, Dark); // Subtract average dark.
                // Write the final difference spectrum for time point.
                LuiData.Write(Data.DeltaOD(Ground, Excited),
                    new long[] { i + 1, 1 }, RowSize);
            }

            // Done with everything.
        }

        void DoTempCheck(Func<bool> Breakout)
        {
            if (Commander.Camera is CameraTempControlled)
            {
                var camct = (CameraTempControlled)Commander.Camera;
                if (camct.TemperatureStatus != CameraTempControlled.TemperatureStabilized)
                {
                    var equil = (bool)Invoke(new Func<bool>(TemperatureStabilizedDialog));
                    if (equil)
                        if (camct.EquilibrateUntil(Breakout))
                            return;
                }
            }
        }

        /// <summary>
        ///     Acquire in loop, exit early if breakout function returns true.
        /// </summary>
        /// <param name="AcqBuffer">Array for raw camera data.</param>
        /// <param name="DataBuffer">Array for binned camera data.</param>
        /// <param name="SumBuffer">Array for accumulated binned data.</param>
        /// <param name="N"></param>
        /// <param name="Breakout"></param>
        void DoAcq(int[] AcqBuffer, int[] DataBuffer, int[] SumBuffer, int N, Func<int, bool> Breakout)
        {
            Array.Clear(SumBuffer, 0, SumBuffer.Length);
            for (var i = 0; i < N; i++)
            {
                Array.Clear(DataBuffer, 0, DataBuffer.Length);
                TryAcquire(AcqBuffer);
                Data.ColumnSum(DataBuffer, AcqBuffer);
                Data.Accumulate(SumBuffer, DataBuffer);
                RawData.WriteNext(DataBuffer, 0);
                if (Breakout(i)) return;
            }
        }

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            DoTempCheck(() => PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.TEMPERATURE)));

            if (PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.INITIALIZE)))
                return; // Show zero progress.

            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var half = N / 2; // N is always even.
            var Times = args.Times;
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;

            // Total scans = dark scans + ground state scans + plus time series scans.
            var TotalScans = N + half + Times.Count * (half + N);

            // Create the data store.
            InitDataFile(AcqWidth, TotalScans, Times.Count);

            // Write dummy value (number of scans).
            LuiData.Write(args.N, new long[] { 0, 0 });
            // Write wavelengths.
            long[] RowSize = { 1, AcqWidth };
            LuiData.Write(Commander.Camera.Calibration, new long[] { 0, 1 }, RowSize);
            // Write times.
            long[] ColSize = { Times.Count, 1 };
            LuiData.Write(Times.ToArray(), new long[] { 1, 0 }, ColSize);

            // Buffers for acuisition data.
            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var Drk = new int[AcqWidth];
            var Gnd1 = new int[AcqWidth];
            var Gnd2 = new int[AcqWidth];
            var Exc = new int[AcqWidth];
            var Ground = new double[AcqWidth];
            var Excited = new double[AcqWidth];
            var Dark = new double[AcqWidth];

            // Collect dark.
            Commander.BeamFlag.CloseLaserAndFlash();
            DoAcq(AcqBuffer, AcqRow, Drk, N,
                p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
            Data.Accumulate(Dark, Drk);
            Data.DivideArray(Dark, N);

            // Run data collection scheme.
            if (args.Pump == PumpMode.ALWAYS) OpenPump(args.DiscardFirst);
            // probe light only
            Commander.BeamFlag.OpenFlash();
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName,
                3.2E-8); // Set delay for GS (avoids laser tail).
            DoAcq(AcqBuffer, AcqRow, Gnd1, half,
                p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_FLASH)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;

            for (var i = 0; i < Times.Count; i++)
            {
                var Delay = Times[i];
                if (args.Pump == PumpMode.TRANS) OpenPump(args.DiscardFirst);
                //Commander.BeamFlag.OpenLaserAndFlash();
                Commander.BeamFlag.OpenLaser();
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay); // Set delay time.
                if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                DoAcq(AcqBuffer, AcqRow, Exc, N,
                    p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_TRANS)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                if (args.Pump == PumpMode.TRANS) Commander.Pump.SetClosed();
                //Commander.BeamFlag.OpenFlash();
                Commander.BeamFlag.CloseLaser();
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName,
                    3.2E-8); // Set delay for GS (avoids laser tail).
                if (i % 2 == 0) // Alternate between Gnd1 and Gnd2.
                {
                    DoAcq(AcqBuffer, AcqRow, Gnd2, half,
                        p => PauseCancelProgress(e, p % half + half,
                            new ProgressObject(null, 0, Dialog.PROGRESS_FLASH)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                }
                else
                {
                    DoAcq(AcqBuffer, AcqRow, Gnd1, half,
                        p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_FLASH)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                }

                Data.Accumulate(Ground, Gnd1);
                Data.Accumulate(Ground, Gnd2);
                Data.DivideArray(Ground, N);
                Data.Accumulate(Excited, Exc);
                Data.DivideArray(Excited, N);
                var deltaOD = Data.DeltaOD(Ground, Excited, Dark);
                LuiData.Write(deltaOD, new long[] { i + 1, 1 }, RowSize);
                if (PauseCancelProgress(e, i, new ProgressObject(deltaOD, Delay, Dialog.PROGRESS_TIME_COMPLETE))
                ) return;
                Array.Clear(Ground, 0, Ground.Length);
                Array.Clear(Excited, 0, Excited.Length);
            }

            Commander.BeamFlag.CloseLaserAndFlash();
            if (args.Pump != PumpMode.NEVER) Commander.Pump.SetClosed();
        }

        protected override void WorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var progress = (ProgressObject)e.UserState;
            //StatusProgress.Value = e.ProgressPercentage;
            var progressValue = (e.ProgressPercentage + 1).ToString();
            switch (progress.Status)
            {
                case Dialog.INITIALIZE:
                    ProgressLabel.Text = "Initializing";
                    break;

                case Dialog.PROGRESS:
                    break;

                case Dialog.PROGRESS_DARK:
                    ProgressLabel.Text = "Collecting dark";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
                    break;

                case Dialog.PROGRESS_TIME:
                    DdgConfigBox.PrimaryDelayValue = progress.Delay;
                    break;

                case Dialog.PROGRESS_TIME_COMPLETE:
                    TimeProgress.Text = progressValue + "/" + Times.Count;
                    Display(progress.Data);
                    break;

                case Dialog.PROGRESS_FLASH:
                    ProgressLabel.Text = "Collecting ground";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
                    break;

                case Dialog.PROGRESS_TRANS:
                    ProgressLabel.Text = "Collecting transient";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
                    break;

                case Dialog.CALCULATE:
                    ProgressLabel.Text = "Calculating...";
                    break;

                case Dialog.TEMPERATURE:
                    ProgressLabel.Text = "Waiting for temperature...";
                    break;
            }
        }

        protected override void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Commander.BeamFlag.CloseLaserAndFlash();
            Commander.Pump.SetClosed();
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

            OnTaskFinished(EventArgs.Empty);
        }

        void Display(double[] Y)
        {
            Graph.DrawPoints(Commander.Camera.Calibration, Y);
            Graph.Invalidate();
            Graph.MarkerColor = Graph.NextColor;
        }

        /// <summary>
        ///     Clears row error if user presses ESC while editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimesView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            TimesView.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        /// <summary>
        ///     Validate that times entered by the user are legal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimesView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (TimesView.Columns[e.ColumnIndex].Name == "Value")
            {
                if (!double.TryParse(e.FormattedValue.ToString(), out var value))
                {
                    TimesView.Rows[e.RowIndex].ErrorText = "Time must be a number";
                    e.Cancel = true;
                }

                if (value <= 0)
                {
                    TimesView.Rows[e.RowIndex].ErrorText = "Time must be positive";
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        ///     Load file containing time delays (one per line).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadTimes_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog
            {
                Filter = "Text File|*.txt|All Files|*.*",
                Title = "Load Time Series File"
            };
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

        void SaveOutput()
        {
            var saveFile = new SaveFileDialog
            {
                Filter = "MAT File|*.mat|CSV File|*.csv",
                Title = "Save As"
            };
            var result = saveFile.ShowDialog();

            if (result != DialogResult.OK || saveFile.FileName == "") return;

            if (File.Exists(saveFile.FileName)) File.Delete(saveFile.FileName);

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
                            var Matrix = new double[LuiData.Dims[0], LuiData.Dims[1]];
                            LuiData.Read(Matrix, new long[] { 0, 0 }, LuiData.Dims);
                            FileIO.WriteMatrix(saveFile.FileName, Matrix);
                        }

                        DataFile.Close();
                    }

                    break;
            }
        }

        bool TemperatureStabilizedDialog()
        {
            var result = MessageBox.Show("Camera temperature has not stabilized. Wait before running?", "Error",
                MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel)
                worker.CancelAsync();
            else if (result == DialogResult.Yes) return true;
            return false;
        }

        public class TimesRow : INotifyPropertyChanged
        {
            double _Value;

            public double Value
            {
                get => _Value;
                set
                {
                    _Value = value;
                    NotifyPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public static explicit operator TimesRow(DataRow dr)
            {
                var p = new TimesRow
                {
                    Value = (double)dr.ItemArray[0]
                };
                return p;
            }

            public static explicit operator TimesRow(DataGridViewRow row)
            {
                return new TimesRow
                {
                    Value = (double)row.Cells["Value"].Value
                };
            }
        }

        struct WorkArgs
        {
            public WorkArgs(int N, IList<double> Times, string PrimaryDelayName, string PrimaryDelayTrigger,
                PumpMode Pump, bool DiscardFirst)
            {
                this.N = N;
                this.Times = new List<double>(Times);
                this.PrimaryDelayName = PrimaryDelayName;
                TriggerName = PrimaryDelayTrigger;
                //this.GateName = new Tuple<char, char>(Gate.Delay[0], Gate.Delay[1]);
                //this.GateTriggerName = Gate.Trigger[0];
                //this.Gate = Gate.DelayValue;
                //this.GateDelay = Gate.DelayValue;
                GateName = null;
                GateTriggerName = '\0';
                Gate = double.NaN;
                GateDelay = double.NaN;
                this.Pump = Pump;
                this.DiscardFirst = DiscardFirst;
            }

            public readonly int N;
            public readonly IList<double> Times;
            public readonly string PrimaryDelayName;
            public readonly string TriggerName;
            public readonly Tuple<char, char> GateName;
            public readonly char GateTriggerName;
            public readonly double GateDelay;
            public readonly double Gate;
            public readonly PumpMode Pump;
            public readonly bool DiscardFirst;
        }

        struct ProgressObject
        {
            public ProgressObject(double[] Data, double Delay, Dialog Status)
            {
                this.Data = Data;
                this.Delay = Delay;
                this.Status = Status;
            }

            public readonly double[] Data;
            public readonly double Delay;
            public readonly Dialog Status;
        }
    }
}