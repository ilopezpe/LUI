using lasercom;
using lasercom.camera;
using lasercom.polarizer;
using lasercom.ddg;
using lasercom.io;
using LUI.config;
using LUI.controls;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Threading;

namespace LUI.tabs
{
    public partial class LdalignControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_DARK,
            PROGRESS_PLUS,
            PROGRESS_MINUS,
            PROGRESS_WORK_COMPLETE,
            CALCULATE,
            TEMPERATURE
        }

        MatFile DataFile;
        MatVar<double> LuiData;
        MatVar<int> RawData;
        CancellationTokenSource TemperatureCts;

        public LdalignControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
            Init();

            PolarizerBox.Enabled = false;
            Beta.ValueChanged += Beta_ValueChanged;

            CameraTemperature.Enabled = false;

            DdgConfigBox.Config = Config;
            DdgConfigBox.Commander = Commander;
            DdgConfigBox.AllowZero = false;
            DdgConfigBox.HandleParametersChanged(this, EventArgs.Empty);

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

        public override void HandleCameraChanged(object sender, EventArgs e)
        {
            base.HandleCameraChanged(sender, e);
            UpdateReadMode();
            if (Commander.Camera is CameraTempControlled)
            {
                CameraTemperature.Enabled = true;
                var camct = (CameraTempControlled)Commander.Camera;
                CameraTemperature.Minimum = camct.MinTemp;
                CameraTemperature.Maximum = camct.MaxTemp;
                CameraTemperature.Increment = (int)CameraTempControlled.TemperatureEps;
                UpdateCameraTemperature(); // Subscribes ValueChanged.
            }
            else
            {
                CameraTemperature.Enabled = false;
                CameraTemperature.ValueChanged -= CameraTemperature_ValueChanged;
            }
        }

        public override void HandleContainingTabSelected(object sender, EventArgs e)
        {
            base.HandleContainingTabSelected(sender, e);
            DdgConfigBox.UpdatePrimaryDelayValue();
            UpdateReadMode();
            UpdateCameraTemperature();
        }

        public virtual void HandlePolarizerChanged(object sender, EventArgs e)
        {
            Commander.Polarizer?.PolarizerToCrossed();
            Commander.Polarizer = (IPolarizer)Config.GetObject(PolarizerBox.SelectedObject);
        }

        protected override void LoadSettings()
        {
            base.LoadSettings();
            var Settings = Config.TabSettings[GetType().Name];
            if (Settings.TryGetValue("PrimaryDelayDdg", out var value) && value != null && value != "")
            {
                DdgConfigBox.PrimaryDelayDdg = (DelayGeneratorParameters)Config.GetFirstParameters(
                    typeof(DelayGeneratorParameters), value);
            }

            if (Settings.TryGetValue("PrimaryDelayDelay", out value) && value != null && value != "")
            {
                DdgConfigBox.PrimaryDelayDelay = value;
            }
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
        /// <param name="NumAngles"></param>
        void InitDataFile(int NumChannels, int NumScans, int NumAngles)
        {
            var TempFileName = Path.GetTempFileName();
            DataFile = new MatFile(TempFileName);
            RawData = DataFile.CreateVariable<int>("rawdata", NumScans, NumChannels);
            LuiData = DataFile.CreateVariable<double>("luidata", NumAngles + 1, NumChannels + 1);
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
            if (DdgConfigBox.PrimaryDelayDelay == null)
            {
                MessageBox.Show("Primary delay must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            if (PolarizerBox.Objects.SelectedItem == null)
            {
                MessageBox.Show("Polarizer controller must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            CameraStatus.Text = "";

            Graph.ClearData();
            Graph.Invalidate();

            var N = (int)NScan.Value;

            DdgConfigBox.ApplyPrimaryDelayValue();
            Commander.BeamFlag.CloseLaserAndFlash();

            ImageMode_CheckedChanged(sender, e);
            FvbMode_CheckedChanged(sender, e);

            SetupWorker();
            worker.RunWorkerAsync(new WorkArgs(N));
            OnTaskStarted(EventArgs.Empty);
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            DdgConfigBox.Enabled = false;
            PolarizerBox.Enabled = false;
            ScanProgress.Text = "0";
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            DdgConfigBox.Enabled = true;
            PolarizerBox.Enabled = true;
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

            if (PauseCancelProgress(e, 0, new ProgressObject(null,Dialog.INITIALIZE)))
            {
                return; // Show zero progress.
            }

            #region Collect Variables
            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;

            // minus + plus + dark
            var TotalScans = N*3;
            #endregion

            #region Initialize datafile
            // Create the data store.
            InitDataFile(AcqWidth, TotalScans, 0);

            // Write dummy value (number of scans).
            //LuiData.Write(args.N, new long[] { 0, 0 });

            // Write wavelengths.
            //long[] RowSize = { 1, AcqWidth };
            //LuiData.Write(Commander.Camera.Calibration, new long[] { 0, 1 }, RowSize);
            #endregion

            #region Initialize buffers for data
            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var DarkBuffer = new int[AcqWidth];
            var Dark = new double[AcqWidth];
            var PlusBetaBuffer = new int[AcqWidth];
            var MinusBetaBuffer = new int[AcqWidth];
            var PlusBeta = new double[AcqWidth];
            var MinusBeta = new double[AcqWidth];
            #endregion

            /* 
            * Collect LD procedure
            * A. Collect dark spectrum at 32 ns
            *      1. Set up dark enviornment
            *      2. Acquire data
            * B. Plus beta intensity
            *      1. Move polarizer to plus beta
            *      2. Open pump and probe beam shutters
            *      3. Acquire data
            *      4. Close beam shutters
            * C.
            *      1. Move polarizer to minus beta
            *      2. Open pump and probe beam shutters
            *      3. Acquire data
            *      4. Close beam shutters
            *      5. Calculate and plot
            */

            // A1. Set up to collect dark spectrum
            Commander.Polarizer.PolarizerToCrossed();
            Commander.BeamFlag.CloseLaserAndFlash();
            // A2. Acquire data  
            DoAcq(AcqBuffer,
                AcqRow,
                DarkBuffer,
                N,
                p => PauseCancelProgress(e, p, new ProgressObject(null,Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, Dialog.PROGRESS))) return;
            Data.Accumulate(Dark, DarkBuffer);

            // B1. Move polarizer to plus beta 
            Commander.Polarizer.PolarizerToPlusBeta();
            // B2. Open pump and probe beam shutters
            Commander.BeamFlag.OpenLaserAndFlash();
            // B3. Acquire data
            DoAcq(AcqBuffer,
                AcqRow,
                PlusBetaBuffer,
                N,
                p => PauseCancelProgress(e, p, new ProgressObject(null, Dialog.PROGRESS_PLUS)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, Dialog.PROGRESS))) return;
            Data.Accumulate(PlusBeta, PlusBetaBuffer);
            // B4. Close pump and probe beam shutters
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
            Data.Accumulate(MinusBeta, MinusBetaBuffer);
            // C4. Close pump and probe beam shutters
            Commander.BeamFlag.CloseLaserAndFlash();

            double[] S = Data.S(PlusBeta, MinusBeta, Dark);
            //LuiData.WriteNext(S, 0);
            if (PauseCancelProgress(e, -1, new ProgressObject(S, Dialog.PROGRESS_WORK_COMPLETE)))
            {
                return;
            }
            Commander.Polarizer.SetAngle(90.00F); // short wait.
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
                    break;

                case Dialog.PROGRESS_PLUS:
                    ProgressLabel.Text = "Collecting plus...";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
                    //Display(progress.Data);
                    break;

                case Dialog.PROGRESS_MINUS:
                    ProgressLabel.Text = "Collecting minus...";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
                    //Display(progress.Data);
                    break;

                case Dialog.PROGRESS_WORK_COMPLETE:
                    Display(progress.Data);
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
            Commander.Polarizer.PolarizerToCrossed();
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

        async void CameraTemperature_ValueChanged(object sender, EventArgs e)
        {
            var camct = Commander.Camera as CameraTempControlled;
            if (camct != null)
            {
                if (TemperatureCts != null) TemperatureCts.Cancel();
                TemperatureCts = new CancellationTokenSource();

                CameraTemperature.ForeColor = Color.Red;
                await camct.EquilibrateTemperatureAsync((int)CameraTemperature.Value,
                    TemperatureCts.Token); // Wait for 3 deg. threshold.
                CameraTemperature.ForeColor = Color.Goldenrod;
                await camct.EquilibrateTemperatureAsync(TemperatureCts.Token); // Wait for driver signal.
                UpdateCameraTemperature();
                CameraTemperature.ForeColor = Color.Black;

                TemperatureCts = null;
            }
        }

        void UpdateCameraTemperature()
        {
            var camct = Commander.Camera as CameraTempControlled;
            if (camct != null)
            {
                CameraTemperature.ValueChanged -= CameraTemperature_ValueChanged;
                CameraTemperature.Value = camct.Temperature;
                CameraTemperature.ValueChanged += CameraTemperature_ValueChanged;
            }
        }

        void UpdateReadMode()
        {
            if (Commander.Camera.ReadMode == AndorCamera.ReadModeImage)
                ImageMode.Checked = true;
            else
                FvbMode.Checked = true;
        }


        void FvbMode_CheckedChanged(object sender, EventArgs e)
        {
            if (FvbMode.Checked)
            {
                Commander.Camera.ReadMode = AndorCamera.ReadModeFVB;
            }

            UpdateCollectText();
        }
        void ImageMode_CheckedChanged(object sender, EventArgs e)
        {
            if (ImageMode.Checked)
            {
                Commander.Camera.ReadMode = AndorCamera.ReadModeImage;
            }
            UpdateCollectText();
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
            {
                worker.CancelAsync();
            }
            else if (result == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }

        public class AnglesRow : INotifyPropertyChanged
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

            public static explicit operator AnglesRow(DataRow dr)
            {
                var p = new AnglesRow
                {
                    Value = (double)dr.ItemArray[0]
                };
                return p;
            }

            public static explicit operator AnglesRow(DataGridViewRow row)
            {
                return new AnglesRow
                {
                    Value = (double)row.Cells["Value"].Value
                };
            }
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