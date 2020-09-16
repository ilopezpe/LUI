using lasercom;
using lasercom.camera;
using lasercom.polarizer;
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
    public partial class TrldControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_DARK,
            PROGRESS_TIME,
            PROGRESS_TIME_COMPLETE,
            PROGRESS_PLUS,
            PROGRESS_MINUS,
            CALCULATE,
            TEMPERATURE
        }

        MatFile DataFile;
        MatVar<double> LuiData;
        MatVar<int> RawData;

        readonly BindingList<TimesRow> TimesList = new BindingList<TimesRow>();

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

        public TrldControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
            Init();
                
            Beta.ValueChanged += Beta_ValueChanged;
                
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
            Commander.Polarizer?.PolarizerToZeroBeta();
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

            if (PolarizerBox.Objects.SelectedItem == null)
            {
                MessageBox.Show("Polarizer controller must be configured.", "Error", MessageBoxButtons.OK);
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

            SetupWorker();
            worker.RunWorkerAsync(new WorkArgs(N, Times, DdgConfigBox.PrimaryDelayDelay,
                DdgConfigBox.PrimaryDelayTrigger));
            OnTaskStarted(EventArgs.Empty);
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            DdgConfigBox.Enabled = false;
            PolarizerBox.Enabled = false;
            LoadTimes.Enabled = SaveData.Enabled = false;
            ScanProgress.Text = "0";
            TimeProgress.Text = "0";
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            DdgConfigBox.Enabled = true;
            PolarizerBox.Enabled = true;
            LoadTimes.Enabled = SaveData.Enabled = true;
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
            DoTempCheck(() => PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.TEMPERATURE)));

            if (PauseCancelProgress(e, 0, new ProgressObject(null, 0, Dialog.INITIALIZE)))
            {
                return; // Show zero progress.
            }

            var args = (WorkArgs)e.Argument;
            var N = args.N; // Save typing for later.
            var Times = args.Times;
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;

            // minus + plus + dark
            var TotalScans = N + Times.Count *N*2;

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
             * A. Collect dark spectrum at 32 ns
             *      1. Set up dark enviornment
             *      2. Acquire data
             * B. Plus beta intensity
             *      1. Set time delay
             *      2. Move polarizer to plus beta
             *      3. Open pump and probe beam shutters
             *      4. Acquire data
             *      5. Close beam shutters
             * C.
             *      1. Move polarizer to minus beta
             *      2. Open pump and probe beam shutters
             *      3. Acquire data
             *      4. Close beam shutters
             */

            // A1. Set up to collect dark spectrum at 32 ns 
            Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, 3.2E-8);
            Commander.BeamFlag.CloseLaserAndFlash();

            // A2. Acquire data 
            DoAcq(AcqBuffer,
                  AcqRow,
                  DarkBuffer,
                  N,
                  p => PauseCancelProgress(e, p, new ProgressObject(null, 0, Dialog.PROGRESS_DARK)));
            if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
            Data.Accumulate(Dark, DarkBuffer);
            Data.DivideArray(Dark, N);

            for (int i = 0; i < Times.Count; i++)
            {
                // B1. Set time delay 
                var Delay = Times[i];
                Commander.DDG.SetDelay(args.PrimaryDelayName, args.TriggerName, Delay);
                // B2. Move polarizer to plus beta 
                Commander.Polarizer.PolarizerToPlusBeta();
                // B3. Open pump and probe beam shutters
                Commander.BeamFlag.OpenLaserAndFlash();
                // B4. Acquire data
                if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                DoAcq(AcqBuffer,
                      AcqRow,
                      PlusBetaBuffer,
                      N,
                      p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_PLUS)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                // B5. Close beam shutters
                Commander.BeamFlag.CloseLaserAndFlash();
                // C1. Move polarizer to minus beta 
                Commander.Polarizer.PolarizerToMinusBeta();
                // C2. Open pump and probe beam shutters
                Commander.BeamFlag.OpenLaserAndFlash();
                // C3. Acquire data
                if (PauseCancelProgress(e, -1, new ProgressObject(null, Delay, Dialog.PROGRESS_TIME))) return;
                DoAcq(AcqBuffer,
                      AcqRow,
                      MinusBetaBuffer,
                      N,
                      p => PauseCancelProgress(e, p, new ProgressObject(null, Delay, Dialog.PROGRESS_MINUS)));
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                // C4. Close beam shutters
                Commander.BeamFlag.CloseLaserAndFlash();

                Data.Accumulate(PlusBeta, PlusBetaBuffer);
                Data.DivideArray(PlusBeta, N);

                Data.Accumulate(MinusBeta, MinusBetaBuffer);
                Data.DivideArray(MinusBeta, N);

                var S = Data.S(PlusBeta, MinusBeta, Dark);
                LuiData.Write(S, new long[] { i + 1, 1 }, RowSize);
                if (PauseCancelProgress(e, i, new ProgressObject(S, Delay, Dialog.PROGRESS_TIME_COMPLETE)))
                {
                    return;
                }
                Array.Clear(PlusBeta, 0, PlusBeta.Length);
                Array.Clear(MinusBeta, 0, MinusBeta.Length);
            }
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

        struct WorkArgs
        {
            public WorkArgs(int N, IList<double> Times, string PrimaryDelayName, string PrimaryDelayTrigger)
            {
                this.N = N;
                this.Times = new List<double>(Times);
                this.PrimaryDelayName = PrimaryDelayName;
                TriggerName = PrimaryDelayTrigger;
                GateName = null;
                GateTriggerName = '\0';
                Gate = double.NaN;
                GateDelay = double.NaN;
            }

            public readonly int N;
            public readonly IList<double> Times;
            public readonly string PrimaryDelayName;
            public readonly string TriggerName;
            public readonly Tuple<char, char> GateName;
            public readonly char GateTriggerName;
            public readonly double GateDelay;
            public readonly double Gate;
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