﻿using LuiHardware;
using LuiHardware.camera;
using LuiHardware.ddg;
using LuiHardware.io;
using LuiHardware.syringepump;
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
    /// <summary>
    /// This control is for time-resolved optical absorption
    /// </summary>
    public partial class TroaControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_DARK,
            PROGRESS_LASER,
            PROGRESS_TIME,
            PROGRESS_TIME_COMPLETE,
            PROGRESS_FLASH,
            PROGRESS_TRANS,
            CALCULATE,
            TEMPERATURE
        }

        public enum SyringePumpMode
        {
            NEVER,
            TRANS,
            ALWAYS
        }

        MatFile DataFile;
        MatVar<double> LuiData;
        MatVar<int> RawData;

        public const double DefaultGsDelay = 3.2E-8;

        readonly BindingList<TimesRow> TimesList = new BindingList<TimesRow>();
        struct WorkArgs
        {
            public WorkArgs(int N, IList<double> Times, string PrimaryDelayName, string PrimaryDelayTrigger,
                SyringePumpMode SyringePump, int DiscardSyringe, int DiscardLaser,double GsDelay)
            {
                this.N = N;
                this.Times = new List<double>(Times);
                this.PrimaryDelayName = PrimaryDelayName;
                TriggerName = PrimaryDelayTrigger;
                GateName = null;
                GateTriggerName = '\0';
                Gate = double.NaN;
                GateDelay = double.NaN;
                this.SyringePump = SyringePump;
                this.DiscardSyringe = DiscardSyringe;
                this.DiscardLaser = DiscardLaser;
                this.GsDelay = GsDelay;
            }

            public readonly int N;
            public readonly IList<double> Times;
            public readonly string PrimaryDelayName;
            public readonly string TriggerName;
            public readonly Tuple<char, char> GateName;
            public readonly char GateTriggerName;
            public readonly double GateDelay;
            public readonly double Gate;
            public readonly SyringePumpMode SyringePump;
            public readonly int DiscardSyringe;
            public readonly int DiscardLaser;
            public readonly double GsDelay;
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

            GsDelay.TextChanged += GsDelay_TextChanged;
            GsDelay.LostFocus += GsDelay_LostFocus;
            GsDelay.KeyPress += GsDelay_KeyPress;

            schemeBox.Items.Add("Half GS");
            schemeBox.Items.Add("Full GS");
            schemeBox.Items.Add("Trans X3");
            schemeBox.Items.Add("Trans X6");
            schemeBox.SelectedIndex = 0;
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
            GsDelay.Text = DefaultGsDelay.ToString("E3");
            ResumeLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            SyringePumpBox.ObjectChanged += HandleSyringePumpChanged;
            base.OnLoad(e);
        }

        public override void HandleParametersChanged(object sender, EventArgs e)
        {
            base.HandleParametersChanged(sender, e); // Takes care of ObjectSelectPanel.
            DdgConfigBox.HandleParametersChanged(sender, e);

            var SyringePumpsAvailable = Config.GetParameters(typeof(SyringePumpParameters));
            if (SyringePumpsAvailable.Count() > 0)
            {
                var selectedSyringePump = SyringePumpBox.SelectedObject;
                SyringePumpBox.Objects.Items.Clear();
                foreach (var p in SyringePumpsAvailable)
                {
                    SyringePumpBox.Objects.Items.Add(p);
                }
                // One of next two lines will trigger CameraChanged event.
                SyringePumpBox.SelectedObject = selectedSyringePump;
                if (SyringePumpBox.Objects.SelectedItem == null) SyringePumpBox.Objects.SelectedIndex = 0;
                SyringePumpBox.Enabled = true;
            }
            else
            {
                SyringePumpBox.Enabled = false;
            }
        }

        public override void HandleContainingTabSelected(object sender, EventArgs e)
        {
            base.HandleContainingTabSelected(sender, e);
            DdgConfigBox.UpdatePrimaryDelayValue();
        }

        public virtual void HandleSyringePumpChanged(object sender, EventArgs e)
        {
            Commander.SyringePump?.SetClosed();
            Commander.SyringePump = (ISyringePump)Config.GetObject(SyringePumpBox.SelectedObject);
        }

        protected override void LoadSettings()
        {
            base.LoadSettings();
            var Settings = Config.TabSettings[GetType().Name];
            if (Settings.TryGetValue("PrimaryDelayDdg", out var value) && !string.IsNullOrEmpty(value))
            {
                DdgConfigBox.PrimaryDelayDdg = (DelayGeneratorParameters)Config.GetFirstParameters(
                    typeof(DelayGeneratorParameters), value);
            }

            if (Settings.TryGetValue("PrimaryDelayDelay", out value) && !string.IsNullOrEmpty(value))
            {
                DdgConfigBox.PrimaryDelayDelay = value;
            }

            if (Settings.TryGetValue("GsDelay", out value) && value != null && value != "")
            {
                GsDelay.Text = value;
            }
        }

        protected override void SaveSettings()
        {
            base.SaveSettings();
            var Settings = Config.TabSettings[GetType().Name];
            Settings["PrimaryDelayDdg"] = DdgConfigBox.PrimaryDelayDdg?.Name;
            Settings["PrimaryDelayDelay"] = DdgConfigBox.PrimaryDelayDelay ?? null;
            Settings["GsDelay"] = GsDelay.Text;
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
            if (schemeBox.SelectedIndex == -1)
            {
                MessageBox.Show("Data scheme must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }
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
            SyringePumpMode Mode;
            if (SyringePumpNever.Checked) Mode = SyringePumpMode.NEVER;
            else if (SyringePumpTs.Checked) Mode = SyringePumpMode.TRANS;
            else Mode = SyringePumpMode.ALWAYS;

            Commander.BeamFlag.CloseLaserAndFlash();

            SetupWorker();
            worker.RunWorkerAsync(new WorkArgs(N, Times, DdgConfigBox.PrimaryDelayDelay,
                DdgConfigBox.PrimaryDelayTrigger, Mode, (int)DiscardSyringe.Value, (int)DiscardLaser.Value, double.Parse(GsDelay.Text)));
            OnTaskStarted(EventArgs.Empty);
        }

        protected override void SetupWorker()
        {
            worker = new BackgroundWorker();

            if (schemeBox.SelectedIndex == 0)
            {
                worker.DoWork += DoWork0;
            }
            else if (schemeBox.SelectedIndex == 1)
            {
                worker.DoWork += DoWork1;
            }
            else if (schemeBox.SelectedIndex == 2)
            {
                worker.DoWork += DoWork2;
            }
            else if (schemeBox.SelectedIndex == 3)
            {
                worker.DoWork += DoWork3;
            }
            worker.ProgressChanged += WorkProgress;
            worker.RunWorkerCompleted += WorkComplete;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            DdgConfigBox.Enabled = false;
            SyringePumpBox.Enabled = false;
            LoadTimes.Enabled = SaveData.Enabled = false;
            ExperimentConfigBox.Enabled = false;
            schemeBox.Enabled = false;
            ScanProgress.Text = "0";
            TimeProgress.Text = "0";
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            DdgConfigBox.Enabled = true;
            SyringePumpBox.Enabled = true;
            LoadTimes.Enabled = SaveData.Enabled = true;
            ExperimentConfigBox.Enabled = true;
            schemeBox.Enabled = true;
        }

        void DoTempCheck(Func<bool> Breakout)
        {
            if (Commander.Camera is AndorTempControlled camct)
            {
                if (camct.TemperatureStatus != AndorTempControlled.TemperatureStabilized)
                {
                    var equil = (bool)Invoke(new Func<bool>(TemperatureStabilizedDialog));
                    if (equil)
                        if (camct.EquilibrateUntil(Breakout))
                            return;
                }
            }
        }

        /// <summary>
        /// Acquire in loop, exit early if breakout function returns true.
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

        /// <summary>
        ///  Scheme 0 Dark, 1/2 GS, ES
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DoWork0(object sender, DoWorkEventArgs e)
        {
            DoTempCheck(() => PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.TEMPERATURE)));

            if (PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.INITIALIZE))) return; // Show zero progress.

            #region Initialize constants
            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var half = N / 2; // N is always even.
            var Times = args.Times;
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;
            #endregion

            #region Initialize data files
            // Total scans = dark scans + ground state scans + plus time series scans.
            var TotalScans = N + half + Times.Count * (half + N) + args.DiscardLaser*N*2;

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
            #endregion

            #region Initialize buffers for acuisition data
            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var Drk = new int[AcqWidth];
            var Gnd1 = new int[AcqWidth];
            var Gnd2 = new int[AcqWidth];
            var Exc = new int[AcqWidth];
            var Lase = new int[AcqWidth];
            var Dark = new double[AcqWidth];
            var Ground = new double[AcqWidth];
            var Excited = new double[AcqWidth];
            var Laser = new double[AcqWidth];
            #endregion

            #region Collect dark spectrum 
            // A0. Reset GS delay for dark
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);

            // A1. Close the pump and probe beam shutters 
            Commander.BeamFlag.CloseLaserAndFlash();

            // A2. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  Drk,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
            #endregion
            Data.Accumulate(Dark, Drk);
            Data.DivideArray(Dark, N);

            // Check syringe pump
            if (args.SyringePump == SyringePumpMode.ALWAYS) OpenSyringePump(args.DiscardSyringe);

            // B. Collect half ground state  
            // B1. Open probe beam shutter
            Commander.BeamFlag.OpenFlash();
            // B2. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  Gnd1,
                  half,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_FLASH)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;

            for (int i = 0; i < Times.Count; i++)
            {
                var Delay = Times[i];

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.ALWAYS) OpenSyringePump(args.DiscardSyringe);

                // C1. Set time delay.
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay);

                #region Collect Excited state
                // C2. Open pump beam shutter
                Commander.BeamFlag.OpenLaser();

                // C3. Acquire excited state data
                if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                DoAcq(AcqBuffer,
                      AcqRow,
                      Exc,
                      N,
                      p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_TRANS)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                Data.Accumulate(Excited, Exc);
                Data.DivideArray(Excited, N);

                // C4. Close pump beam shutter
                Commander.BeamFlag.CloseLaser();

                #endregion

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.TRANS) Commander.SyringePump.SetClosed();

                // C5. Go back and collect half ground state 
                // Update ground state every other time point.
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);
                if (i % 2 == 0)
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

                // Collect pump only X times
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    // C1. Set time delay.
                    Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay);

                    #region Collect pump only/laser only
                    // C2. Open pump beam shutter
                    Commander.BeamFlag.OpenLaser();

                    // C3. Acquire pump only data
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                    DoAcq(AcqBuffer,
                          AcqRow,
                          Lase,
                          N,
                          p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_LASER)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                    Data.Accumulate(Laser, Lase);
                    Data.DivideArray(Laser, N);

                    // C4. Close pump beam shutter
                    Commander.BeamFlag.CloseLaser();
                    #endregion
                }

                double[] deltaOD;
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Laser);
                    Array.Clear(Laser, 0, Laser.Length);
                }
                else
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Dark);
                }

                LuiData.Write(deltaOD, new long[] { i + 1, 1 }, RowSize);
                if (PauseCancelProgress(e, i, new ProgressObject(deltaOD, Delay, Dialog.PROGRESS_TIME_COMPLETE)))
                {
                    return;
                }
                Array.Clear(Ground, 0, Ground.Length);
                Array.Clear(Excited, 0, Excited.Length);
            }
            if (args.SyringePump != SyringePumpMode.NEVER) Commander.SyringePump.SetClosed();
            Commander.BeamFlag.CloseLaserAndFlash();
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);
        }

        /// <summary>
        /// Scheme I Dark, GS, ES
        /// w/ option to collect laser only background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DoWork1(object sender, DoWorkEventArgs e)
        {
            DoTempCheck(() => PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.TEMPERATURE)));

            if (PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.INITIALIZE))) return; // Show zero progress.

            #region Initialize constants
            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var Times = args.Times;
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;
            #endregion

            #region Initialize data files
            // Total scans = dark scans + plus time series scans.
            int TotalScans = N + Times.Count*N*2 + args.DiscardLaser*N*2;               

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
            #endregion

            #region Initialize buffers for acuisition data
            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var Drk = new int[AcqWidth];
            var Gnd = new int[AcqWidth];
            var Exc = new int[AcqWidth];
            var Lase = new int[AcqWidth];
            var Dark = new double[AcqWidth];
            var Ground = new double[AcqWidth];
            var Excited = new double[AcqWidth];
            var Laser = new double[AcqWidth];
            #endregion

            #region Collect dark spectrum 
            // A0. Reset GS delay for dark
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);

            // A1. Close the pump and probe beam shutters 
            Commander.BeamFlag.CloseLaserAndFlash();

            // A2. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  Drk,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
            Data.Accumulate(Dark, Drk);
            Data.DivideArray(Dark, N);
            #endregion

            for (int i = 0; i < Times.Count; i++)
            {
                var Delay = Times[i];

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.ALWAYS) OpenSyringePump(args.DiscardSyringe);

                // B0. Set time delay w/o laser background
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);

                #region B Collect ground state
                // B1. Open probe beam shutter
                Commander.BeamFlag.OpenFlash();

                // B2. Acquire data
                DoAcq(AcqBuffer,
                        AcqRow,
                        Gnd,
                        N,
                        p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_FLASH)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                Data.Accumulate(Ground, Gnd);
                Data.DivideArray(Ground, N);
                #endregion

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.TRANS) OpenSyringePump(args.DiscardSyringe);

                // C1. Set time delay.
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay);

                #region C. Collect excited state 

                // C2. Open pump beam shutter
                Commander.BeamFlag.OpenLaser();

                // C3. Acquire excited state data
                if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                DoAcq(AcqBuffer,
                      AcqRow,
                      Exc,
                      N,
                      p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_TRANS)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                Data.Accumulate(Excited, Exc);
                Data.DivideArray(Excited, N);

                // C4. Close pump beam shutter
                Commander.BeamFlag.CloseLaserAndFlash();

                #endregion

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.TRANS) Commander.SyringePump.SetClosed();

                // Collect pump only X times
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    #region Collect pump only/laser only
                    // C2. Open pump beam shutter
                    Commander.BeamFlag.OpenLaser();

                    // C3. Acquire pump only data
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                    DoAcq(AcqBuffer,
                          AcqRow,
                          Lase,
                          N,
                          p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_LASER)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                    Data.Accumulate(Laser, Lase);
                    Data.DivideArray(Laser, N);

                    // C4. Close pump beam shutter
                    Commander.BeamFlag.CloseLaser();
                    #endregion
                }

                double[] deltaOD;
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Laser);
                    Array.Clear(Laser, 0, Laser.Length);
                }
                else
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Dark);
                }

                LuiData.Write(deltaOD, new long[] { i + 1, 1 }, RowSize);
                if (PauseCancelProgress(e, i, new ProgressObject(deltaOD, Delay, Dialog.PROGRESS_TIME_COMPLETE)))
                {
                    return;
                }
                Array.Clear(Ground, 0, Ground.Length);
                Array.Clear(Excited, 0, Excited.Length);
            }
            if (args.SyringePump != SyringePumpMode.NEVER) Commander.SyringePump.SetClosed();
            Commander.BeamFlag.CloseLaserAndFlash();
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);
        }

        /// <summary>
        /// Scheme II Dark, GS, ES, ES, ES, GS
        /// w/ option to collect laser only background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DoWork2(object sender, DoWorkEventArgs e)
        {
            DoTempCheck(() => PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.TEMPERATURE)));

            if (PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.INITIALIZE))) return; // Show zero progress.

            #region Initialize constants
            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var Times = args.Times;
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;
            #endregion

            #region Initialize data files
            // Total scans = dark scans + plus time series scans.
            int TotalScans = N + Times.Count * N + Times.Count * N / (3 - 1) + args.DiscardLaser * N * 2;

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
            #endregion

            #region Initialize buffers for acuisition data
            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var Drk = new int[AcqWidth];
            var Gnd = new int[AcqWidth];
            var Exc = new int[AcqWidth];
            var Lase = new int[AcqWidth];
            var Dark = new double[AcqWidth];
            var Ground = new double[AcqWidth];
            var Excited = new double[AcqWidth];
            var Laser = new double[AcqWidth];
            #endregion

            #region Collect dark spectrum 
            // A0. Reset GS delay for dark
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);

            // A1. Close the pump and probe beam shutters 
            Commander.BeamFlag.CloseLaserAndFlash();

            // A2. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  Drk,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
            Data.Accumulate(Dark, Drk);
            Data.DivideArray(Dark, N);
            #endregion

            for (int i = 0; i < Times.Count; i++)
            {
                var Delay = Times[i];

                if (i % 3 == 0)
                {   // Check syringe pump
                    if (args.SyringePump == SyringePumpMode.ALWAYS) OpenSyringePump(args.DiscardSyringe);

                    // B0. Set time delay w/o laser background
                    Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);

                    #region B Collect ground state
                    // B1. Open probe beam shutter
                    Commander.BeamFlag.OpenFlash();

                    // B2. Acquire data
                    Array.Clear(Ground, 0, Ground.Length);
                    DoAcq(AcqBuffer,
                            AcqRow,
                            Gnd,
                            N,
                            p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_FLASH)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                    Data.Accumulate(Ground, Gnd);
                    Data.DivideArray(Ground, N);
                    #endregion
                }

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.TRANS) OpenSyringePump(args.DiscardSyringe);

                // C1. Set time delay.
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay);

                #region C. Collect excited state 

                // C2. Open pump beam shutter
                Commander.BeamFlag.OpenLaserAndFlash();

                // C3. Acquire excited state data
                if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                DoAcq(AcqBuffer,
                      AcqRow,
                      Exc,
                      N,
                      p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_TRANS)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                Data.Accumulate(Excited, Exc);
                Data.DivideArray(Excited, N);

                // C4. Close pump beam shutter
                Commander.BeamFlag.CloseLaserAndFlash();

                #endregion

                // Collect pump only X times
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    #region Collect pump only/laser only
                    // C2. Open pump beam shutter
                    Commander.BeamFlag.OpenLaser();

                    // C3. Acquire pump only data
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                    DoAcq(AcqBuffer,
                          AcqRow,
                          Lase,
                          N,
                          p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_LASER)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                    Data.Accumulate(Laser, Lase);
                    Data.DivideArray(Laser, N);

                    // C4. Close pump beam shutter
                    Commander.BeamFlag.CloseLaser();
                    #endregion
                }

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.TRANS) Commander.SyringePump.SetClosed();

                double[] deltaOD;
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Laser);
                    Array.Clear(Laser, 0, Laser.Length);
                }
                else
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Dark);
                }

                LuiData.Write(deltaOD, new long[] { i + 1, 1 }, RowSize);
                if (PauseCancelProgress(e, i, new ProgressObject(deltaOD, Delay, Dialog.PROGRESS_TIME_COMPLETE)))
                {
                    return;
                }
                Array.Clear(Excited, 0, Excited.Length);
            }
            if (args.SyringePump != SyringePumpMode.NEVER) Commander.SyringePump.SetClosed();
            Commander.BeamFlag.CloseLaserAndFlash();
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);
        }

        /// <summary>
        /// Scheme II Dark, GS, ES X 6, GS
        /// w/ option to collect laser only background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DoWork3(object sender, DoWorkEventArgs e)
        {
            DoTempCheck(() => PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.TEMPERATURE)));

            if (PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.INITIALIZE))) return; // Show zero progress.

            #region Initialize constants
            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var Times = args.Times;
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;
            #endregion

            #region Initialize data files
            // N_dark + N_tp*n_trans + N_tp*N_gs
            int TotalScans = N + Times.Count * N + Times.Count*N/(6-1) + args.DiscardLaser * N *2;

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
            #endregion

            #region Initialize buffers for acuisition data
            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var Drk = new int[AcqWidth];
            var Gnd = new int[AcqWidth];
            var Exc = new int[AcqWidth];
            var Lase = new int[AcqWidth];
            var Dark = new double[AcqWidth];
            var Ground = new double[AcqWidth];
            var Excited = new double[AcqWidth];
            var Laser = new double[AcqWidth];
            #endregion

            #region Collect dark spectrum 
            // A0. Reset GS delay for dark
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);

            // A1. Close the pump and probe beam shutters 
            Commander.BeamFlag.CloseLaserAndFlash();

            // A2. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  Drk,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
            Data.Accumulate(Dark, Drk);
            Data.DivideArray(Dark, N);
            #endregion

            for (int i = 0; i < Times.Count; i++)
            {
                var Delay = Times[i];

                if (i % 6 == 0)
                {   // Check syringe pump
                    if (args.SyringePump == SyringePumpMode.ALWAYS) OpenSyringePump(args.DiscardSyringe);

                    // B0. Set time delay w/o laser background
                    Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);

                    #region B Collect ground state
                    // B1. Open probe beam shutter
                    Commander.BeamFlag.OpenFlash();

                    // B2. Acquire data
                    Array.Clear(Ground, 0, Ground.Length);
                    DoAcq(AcqBuffer,
                            AcqRow,
                            Gnd,
                            N,
                            p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_FLASH)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                    Data.Accumulate(Ground, Gnd);
                    Data.DivideArray(Ground, N);
                    #endregion
                }

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.TRANS) OpenSyringePump(args.DiscardSyringe);

                // C1. Set time delay.
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay);

                #region C. Collect excited state 

                // C2. Open pump beam shutter
                Commander.BeamFlag.OpenLaserAndFlash();

                // C3. Acquire excited state data
                if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                DoAcq(AcqBuffer,
                      AcqRow,
                      Exc,
                      N,
                      p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_TRANS)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                Data.Accumulate(Excited, Exc);
                Data.DivideArray(Excited, N);

                // C4. Close pump beam shutter
                Commander.BeamFlag.CloseLaserAndFlash();

                #endregion

                // Collect pump only X times
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    #region Collect pump only/laser only
                    // C2. Open pump beam shutter
                    Commander.BeamFlag.OpenLaser();

                    // C3. Acquire pump only data
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                    DoAcq(AcqBuffer,
                          AcqRow,
                          Lase,
                          N,
                          p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_LASER)));
                    if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                    Data.Accumulate(Laser, Lase);
                    Data.DivideArray(Laser, N);

                    // C4. Close pump beam shutter
                    Commander.BeamFlag.CloseLaser();
                    #endregion
                }

                // Check syringe pump
                if (args.SyringePump == SyringePumpMode.TRANS) Commander.SyringePump.SetClosed();

                double[] deltaOD;
                if (i < args.DiscardLaser || (i > Times.Count - args.DiscardLaser - 1))
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Laser);
                    Array.Clear(Laser, 0, Laser.Length);
                }
                else
                {
                    deltaOD = Data.DeltaOD(Ground, Excited, Dark);
                }

                LuiData.Write(deltaOD, new long[] { i + 1, 1 }, RowSize);
                if (PauseCancelProgress(e, i, new ProgressObject(deltaOD, Delay, Dialog.PROGRESS_TIME_COMPLETE)))
                {
                    return;
                }
                Array.Clear(Excited, 0, Excited.Length);
            }
            if (args.SyringePump != SyringePumpMode.NEVER) Commander.SyringePump.SetClosed();
            Commander.BeamFlag.CloseLaserAndFlash();
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, args.GsDelay);
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

                case Dialog.PROGRESS_LASER:
                    ProgressLabel.Text = "Collecting laser only";
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
            Commander.SyringePump.SetClosed();
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
            DataFile?.Close();

            OnTaskFinished(EventArgs.Empty);
        }

        void Display(double[] Y)
        {
            Graph.DrawPoints(Commander.Camera.Calibration, Y);
            Graph.Invalidate();
            Graph.MarkerColor = Graph.NextColor;
        }

        /// <summary>
        /// Clears row error if user presses ESC while editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimesView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            TimesView.Rows[e.RowIndex].ErrorText = string.Empty;
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

        void GsDelay_LostFocus(object sender, EventArgs e)
        {
            if (!double.TryParse(GsDelay.Text, out _))
            {
                GsDelay.Text = GsDelay.Tag != null ? (string)GsDelay.Tag : DefaultGsDelay.ToString("E3");
            }
        }

        void GsDelay_TextChanged(object sender, EventArgs e)
        {
            if (!double.TryParse(GsDelay.Text, out double value))
            {
                GsDelay.ForeColor = Color.Red;
            }
            else
            {
                GsDelay.ForeColor = Color.Black;
                GsDelay.Tag = value.ToString("E3");
            }
        }

        void GsDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            Keys key = (Keys)e.KeyChar;
            if (key == Keys.Enter)
            {
                GsDelay.Text = GsDelay.Tag != null ? (string)GsDelay.Tag : DefaultGsDelay.ToString("E3");
                e.Handled = true;
            }
            if (key == Keys.Escape)
            {
                GsDelay.Text = GsDelay.Tag != null ? (string)GsDelay.Tag : DefaultGsDelay.ToString("E3");
                e.Handled = true;
            }
        }

    }
}