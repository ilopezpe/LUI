using LuiHardware;
using LuiHardware.camera;
using LuiHardware.ddg;
using LuiHardware.io;
using LuiHardware.polarizer;
using LUI.config;
using LUI.controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LUI.tabs
{
    public partial class LdalignControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_DARK,
            PROGRESS_COMPLETE,
            PROGRESS_PLUS,
            PROGRESS_MINUS,
            CALCULATE,
            TEMPERATURE
        }

        MatFile DataFile;
        MatVar<double> LuiData;
        MatVar<int> RawData;

        double[] LastGraphTrace;
        int _SelectedChannel = -1;

        public LdalignControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
            Init();

            Beta.ValueChanged += Beta_ValueChanged;

            SaveData.Click += (sender, e) => SaveOutput();

            DdgConfigBox.Config = Config;
            DdgConfigBox.Commander = Commander;
            DdgConfigBox.AllowZero = false;
            DdgConfigBox.HandleParametersChanged(this, EventArgs.Empty);
        }

        int SelectedChannel
        {
            get => _SelectedChannel;
            set
            {
                _SelectedChannel = Math.Max(Math.Min(value, Commander.Camera.Width - 1), 0);
                if (LastGraphTrace != null) CountsDisplay.Text = LastGraphTrace[_SelectedChannel].ToString("n");
            }
        }

        void Init()
        {
            SuspendLayout();

            ScanProgress.Text = "0";

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
            PolarizerBox.ObjectChanged += HandlePolarizerChanged;
            base.OnLoad(e);
        }

        void Beta_ValueChanged(object sender, EventArgs e)
        {
            Commander.Polarizer.PolarizerBeta = (int)Beta.Value;
        }

        public override void HandleParametersChanged(object sender, EventArgs e)
        {
            base.HandleParametersChanged(sender, e); // Takes care of ObjectSelectPanel.
            DdgConfigBox.HandleParametersChanged(sender, e);

            var PolarizersAvailable = Config.GetParameters(typeof(PolarizerParameters));
            if (PolarizersAvailable.Count() > 0)
            {
                var selectedPolarizer = PolarizerBox.SelectedObject;
                PolarizerBox.Objects.Items.Clear();
                foreach (var p in PolarizersAvailable)
                {
                    PolarizerBox.Objects.Items.Add(p);
                }
                // One of next two lines will trigger CameraChanged event.
                PolarizerBox.SelectedObject = selectedPolarizer;
                if (PolarizerBox.Objects.SelectedItem == null)
                {
                    PolarizerBox.Objects.SelectedIndex = 0;
                }

                Beta.Minimum = Commander.Polarizer.MinBeta;
                Beta.Maximum = Commander.Polarizer.MaxBeta;
                Beta.Value = (decimal)Commander.Polarizer.PolarizerBeta;

                PolarizerBox.Enabled = true;
            }
            else
            {
                Beta.Minimum = 0;
                Beta.Maximum = 0;
                Beta.Value = 0;
                PolarizerBox.Enabled = false;
            }
        }

        public override void HandleContainingTabSelected(object sender, EventArgs e)
        {
            base.HandleContainingTabSelected(sender, e);
            DdgConfigBox.UpdatePrimaryDelayValue();
        }

        public virtual void HandlePolarizerChanged(object sender, EventArgs e)
        {
            //Commander.Polarizer?.PolarizerToZeroBeta();
            Commander.Polarizer = (IPolarizer)Config.GetObject(PolarizerBox.SelectedObject);
        }

        protected override void LoadSettings()
        {
            base.LoadSettings();
            var Settings = Config.TabSettings[GetType().Name];
            if (Settings.TryGetValue("PrimaryDelayDdg", out var value) && !string.IsNullOrEmpty(value))
                DdgConfigBox.PrimaryDelayDdg = (DelayGeneratorParameters)Config.GetFirstParameters(
                    typeof(DelayGeneratorParameters), value);

            if (Settings.TryGetValue("PrimaryDelayDelay", out value) && !string.IsNullOrEmpty(value))
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
        void InitDataFile(int NumChannels, int NumScans)
        {
            var TempFileName = Path.GetTempFileName();
            DataFile = new MatFile(TempFileName);
            RawData = DataFile.CreateVariable<int>("rawdata", NumScans, NumChannels);
            LuiData = DataFile.CreateVariable<double>("luidata", 2, NumChannels);
        }

        protected override void Graph_Click(object sender, MouseEventArgs e)
        {
            // Selects a *physical channel* on the camera.
            SelectedChannel = Commander.Camera.CalibrationAscending
                ? (int)Math.Round(Graph.AxesToNormalized(Graph.ScreenToAxes(new Point(e.X, e.Y))).X *
                                   (Commander.Camera.Width - 1))
                : (int)Math.Round((1 - Graph.AxesToNormalized(Graph.ScreenToAxes(new Point(e.X, e.Y))).X) *
                                   (Commander.Camera.Width - 1));
            RedrawLines();
        }

        void RedrawLines()
        {
            Graph.ClearAnnotation();
            Graph.Annotate(GraphControl.Annotation.VERTLINE, Graph.ColorOrder[0],
                Commander.Camera.Calibration[SelectedChannel]);
            Graph.Invalidate();
        }

        protected override void Collect_Click(object sender, EventArgs e)
        {
            if (DdgConfigBox.PrimaryDelayDdg == null || DdgConfigBox.PrimaryDelayDelay == null)
            {
                MessageBox.Show("Primary delay must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            if (PolarizerBox.Objects.SelectedItem == null)
            {
                MessageBox.Show("Polarizer controller must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            DdgConfigBox.ApplyPrimaryDelayValue();

            CameraStatus.Text = "";

            Graph.Invalidate();

            var N = (int)NScan.Value;

            Commander.BeamFlag.CloseLaserAndFlash();
            SetupWorker();
            worker.RunWorkerAsync(new WorkArgs(N));
            OnTaskStarted(EventArgs.Empty);
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            DdgConfigBox.Enabled = false;
            PolarizerBox.Enabled = false;
            SaveData.Enabled = false;
            ScanProgress.Text = "0";
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            DdgConfigBox.Enabled = true;
            PolarizerBox.Enabled = true;
            SaveData.Enabled = true;
        }

        void DoTempCheck(Func<bool> Breakout)
        {
            if (Commander.Camera is AndorTempControlled)
            {
                var camct = (AndorTempControlled)Commander.Camera;
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

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            DoTempCheck(() => PauseCancelProgress(e, 0, new ProgressObject(null, Dialog.TEMPERATURE)));

            if (PauseCancelProgress(e, 0, new ProgressObject(null, Dialog.INITIALIZE)))
            {
                return; // Show zero progress.
            }

            #region collect variables 
            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;

            // minus + plus + dark
            var TotalScans = N * 3;
            #endregion

            #region initialize data files
            // Create the data store.
            InitDataFile(AcqWidth, TotalScans);

            // Write wavelengths.
            LuiData.WriteNext(Commander.Camera.Calibration, 0);

            long[] RowSize = { 1, AcqWidth };
            #endregion

            #region Initialize buffers for acuisition data
            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var DarkBuffer = new int[AcqWidth];
            var PlusBetaBuffer = new int[AcqWidth];
            var MinusBetaBuffer = new int[AcqWidth];
            var PlusBeta = new double[AcqWidth];
            var MinusBeta = new double[AcqWidth];
            var Dark = new double[AcqWidth];
            #endregion

            /* 
            * Collect LD procedure
            * A. Set up to collect dark spectrum
            *      1. Set up dark enviornment
            *      2. Acquire data
            * B. Plus beta intensity
            *      1. Adjust time delay
            *      2. Move polarizer to plus beta
            *      3. Open pump and probe beam shutters
            *      4. Acquire data
            *      5. Close beam shutters
            * C.
            *      1. Move polarizer to minus beta
            *      2. Open pump and probe beam shutters
            *      3. Acquire data
            *      4. Close beam shutters
            *      5. Calculate and plot
            */

            // A1. Set up to collect dark spectrum 
            Commander.Polarizer.PolarizerToZeroBeta();
            // A2. Acquire data 
            DoAcq(AcqBuffer,
                  AcqRow,
                  DarkBuffer,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, Dialog.PROGRESS))) return;
            Data.Accumulate(Dark, DarkBuffer);

            // B2. Move polarizer to plus beta 
            Commander.Polarizer.PolarizerToPlusBeta();
            // B3. Open pump and probe beam shutters
            Commander.BeamFlag.OpenLaserAndFlash();
            // B4. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  PlusBetaBuffer,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, Dialog.PROGRESS_PLUS)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, Dialog.PROGRESS))) return;
            // B5. Close beam shutters
            Commander.BeamFlag.CloseLaserAndFlash();

            // C1. Move polarizer to minus beta 
            Commander.Polarizer.PolarizerToMinusBeta();
            // C2. Open pump and probe beam shutters
            Commander.BeamFlag.OpenLaserAndFlash();
            // C3. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  MinusBetaBuffer,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, Dialog.PROGRESS_MINUS)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, Dialog.PROGRESS))) return;
            // C4. Close beam shutters
            Commander.BeamFlag.CloseLaserAndFlash();

            Data.Accumulate(PlusBeta, PlusBetaBuffer);
            Data.Accumulate(MinusBeta, MinusBetaBuffer);

            var S = Data.S(PlusBeta, MinusBeta, Dark);
            LuiData.Write(S, new long[] { 1, 0 }, RowSize);
            if (PauseCancelProgress(e, -1, new ProgressObject(S, Dialog.PROGRESS_COMPLETE)))
            {
                return;
            }
            Array.Clear(PlusBeta, 0, PlusBeta.Length);
            Array.Clear(MinusBeta, 0, MinusBeta.Length);
        }

        protected override void WorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var progress = (ProgressObject)e.UserState;
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

                case Dialog.PROGRESS_COMPLETE:
                    Display(progress.Data);
                    LastGraphTrace = progress.Data;
                    break;

                case Dialog.PROGRESS_PLUS:
                    ProgressLabel.Text = "Collecting plus Beta";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
                    break;

                case Dialog.PROGRESS_MINUS:
                    ProgressLabel.Text = "Collecting minus Beta";
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
            Commander.Polarizer.PolarizerToZeroBeta();
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
                ProgressLabel.Text = "Complete";
                SelectedChannel = SelectedChannel;
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

        struct WorkArgs
        {
            public WorkArgs(int N)
            {
                this.N = N;
            }
            public readonly int N;
        }

        struct ProgressObject
        {
            public ProgressObject(double[] Data, Dialog Status)
            {
                this.Data = Data;
                this.Status = Status;
            }
            public readonly double[] Data;
            public readonly Dialog Status;
        }
    }
}