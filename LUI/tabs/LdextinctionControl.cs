﻿using lasercom;
using lasercom.camera;
using lasercom.ddg;
using lasercom.io;
using lasercom.polarizer;
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
    public partial class LdextinctionControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_DARK,
            PROGRESS_WORK,
            PROGRESS_COMPLETE,
            TEMPERATURE
        }

        MatFile DataFile;
        MatVar<double> LuiData;
        MatVar<int> RawData;

        public LdextinctionControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
            Init();

            Beta.ValueChanged += Beta_ValueChanged;

            SaveData.Click += (sender, e) => SaveOutput();
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
        }

        protected override void SaveSettings()
        {
            base.SaveSettings();
            var Settings = Config.TabSettings[GetType().Name];
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
            if (PolarizerBox.Objects.SelectedItem == null)
            {
                MessageBox.Show("Polarizer controller must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

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
            PolarizerBox.Enabled = false;
            SaveData.Enabled = false;
            ScanProgress.Text = "0";
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            PolarizerBox.Enabled = true;
            SaveData.Enabled = true;
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

            // Number of accumulations + dark
            var TotalScans = N * 2;
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
            var PlusBeta = new double[AcqWidth];
            var Dark = new double[AcqWidth];
            #endregion

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

            // B3. Open pump and probe beam shutters
            Commander.BeamFlag.OpenFlash();
            // B4. Acquire data
            DoAcq(AcqBuffer,
                  AcqRow,
                  PlusBetaBuffer,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, Dialog.PROGRESS_WORK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, Dialog.PROGRESS))) return;
            Commander.BeamFlag.CloseFlash();

            Data.Accumulate(PlusBeta, PlusBetaBuffer);
            Data.DivideArray(PlusBeta, N);

            var Y = Data.Y(PlusBeta, Dark);
            LuiData.Write(Y, new long[] { 1, 0 }, RowSize);
            if (PauseCancelProgress(e, -1, new ProgressObject(Y, Dialog.PROGRESS_COMPLETE)))
            {
                return;
            }
            Array.Clear(PlusBeta, 0, PlusBeta.Length);
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
                    break;

                case Dialog.PROGRESS_WORK:
                    ProgressLabel.Text = "Collecting plus Beta";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
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