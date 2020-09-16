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
    public partial class CrossControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_ANGLE,
            PROGRESS_WORK,
            PROGRESS_WORK_COMPLETE,
            CALCULATE,
            TEMPERATURE
        }

        MatFile DataFile;
        MatVar<double> LuiData;
        MatVar<int> RawData;

        readonly BindingList<AnglesRow> AnglesList = new BindingList<AnglesRow>();

        IList<double> Angles
        {
            get { return AnglesList.Select(x => x.Value).ToList(); }
            set
            {
                AnglesList.Clear();
                foreach (var d in value)
                    AnglesList.Add(new AnglesRow { Value = d });
            }
        }

        public CrossControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
            Init();

            panel1.Enabled = false;
            Beta.ValueChanged += Beta_ValueChanged;
                
            AnglesList.AllowEdit = false;
            AnglesView.DefaultValuesNeeded += (sender, e) => { e.Row.Cells["Value"].Value = 0; };
            AnglesView.DataSource = new BindingSource(AnglesList, null);
            AnglesView.CellValidating += AnglesView_CellValidating;
            AnglesView.CellEndEdit += AnglesView_CellEndEdit;

            SaveData.Click += (sender, e) => SaveOutput();
        }

        void Init()
        {
            SuspendLayout();

            ScanProgress.Text = "0";
            AngleProgress.Text = "0";

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
                Beta.Value = Commander.Polarizer.PolarizerBeta;

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
            Commander.Polarizer?.PolarizerToCrossed();
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
            if (PolarizerBox.Objects.SelectedItem == null)
            {
                MessageBox.Show("Polarizer controller must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            if (Angles.Count < 1)
            {
                MessageBox.Show("Angles to be scanned must be configured.", "Error", MessageBoxButtons.OK);
                return;
            }

            CameraStatus.Text = "";

            Graph.ClearData();
            Graph.Invalidate();

            var N = (int)NScan.Value;

            Commander.BeamFlag.CloseLaserAndFlash();

            SetupWorker();
            worker.RunWorkerAsync(new WorkArgs(N, Angles));
            OnTaskStarted(EventArgs.Empty);
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            PolarizerBox.Enabled = false;
            LoadAngles.Enabled = SaveData.Enabled = false;
            ScanProgress.Text = "0";
            AngleProgress.Text = "0";
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            PolarizerBox.Enabled = true;
            LoadAngles.Enabled = SaveData.Enabled = true;
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
            var Angles = args.Angles;
            var AcqSize = Commander.Camera.AcqSize;
            var AcqWidth = Commander.Camera.AcqWidth;

            var TotalScans =  Angles.Count * N;

            // Create the data store.
            InitDataFile(AcqWidth, TotalScans, Angles.Count);

            // Write dummy value (number of scans).
            LuiData.Write(args.N, new long[] { 0, 0 });

            // Write wavelengths.
            long[] RowSize = { 1, AcqWidth };
            LuiData.Write(Commander.Camera.Calibration, new long[] { 0, 1 }, RowSize);

            // Write Angles.
            long[] ColSize = { Angles.Count, 1 };
            LuiData.Write(Angles.ToArray(), new long[] { 1, 0 }, ColSize);

            var AcqBuffer = new int[AcqSize];
            var AcqRow = new int[AcqWidth];
            var AngleDataBuffer = new int[AcqWidth];
            var AngleData = new double[AcqWidth];

            // Send to crossed position
            Commander.Polarizer.PolarizerToCrossed();

            for (int i = 0; i < Angles.Count; i++)
            {
                var Angle = Angles[i];
                Commander.Polarizer.SetAngle((float)Angle);
                Commander.BeamFlag.OpenFlash();

                if (PauseCancelProgress(e, -1, new ProgressObject(null, Angle, Dialog.PROGRESS))) return;
                DoAcq(AcqBuffer,
                      AcqRow,
                      AngleDataBuffer,
                      N,
                      p => PauseCancelProgress(e, p, new ProgressObject(null, Angle, Dialog.PROGRESS_WORK)));
                
                if (PauseCancelProgress(e, -1, new ProgressObject(null, 0, Dialog.PROGRESS))) return;
                           
                Data.Accumulate(AngleData, AngleDataBuffer);
                var Y = Data.returnY(AngleData);
                LuiData.Write(Y, new long[] { i + 1, 1 }, RowSize);
                if (PauseCancelProgress(e, i, new ProgressObject(Y, Angle, Dialog.PROGRESS_WORK_COMPLETE)))
                {
                    return;
                }
                Array.Clear(AngleData, 0, AngleData.Length);
                Commander.BeamFlag.CloseFlash();
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

                case Dialog.PROGRESS_WORK:
                    ProgressLabel.Text = "Collecting...";
                    ScanProgress.Text = progressValue + "/" + NScan.Value;
                    //Display(progress.Data);
                    break;

                case Dialog.PROGRESS_WORK_COMPLETE:
                    AngleProgress.Text = progressValue + "/" + Angles.Count;
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

        /// <summary>
        /// Clears row error if user presses ESC while editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AnglesView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AnglesView.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        /// <summary>
        /// Validate that Angles entered by the user are legal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AnglesView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (AnglesView.Columns[e.ColumnIndex].Name == "Value")
            {
                if (!double.TryParse(e.FormattedValue.ToString(), out var value))
                {
                    AnglesView.Rows[e.RowIndex].ErrorText = "Angle must be a number";
                    e.Cancel = true;
                }

                if (value <= 0)
                {
                    AnglesView.Rows[e.RowIndex].ErrorText = "Angle must be positive";
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        ///     Load file containing angles (one per line).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadAngles_Click(object sender, EventArgs e)
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
                Angles = FileIO.ReadTimesFile(openFile.FileName);
            }
            catch (IOException ex)
            {
                Log.Error(ex);
                MessageBox.Show("Couldn't load Angles file at " + openFile.FileName);
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
            public WorkArgs(int N, IList<double> Angles)
            {
                this.N = N;
                this.Angles = new List<double>(Angles);
            }

            public readonly int N;
            public readonly IList<double> Angles;
        }

        struct ProgressObject
        {
            public ProgressObject(double[] Data, double Angle, Dialog Status)
            {
                this.Data = Data;
                this.Angle = Angle;
                this.Status = Status;
            }
            public readonly double[] Data;
            public readonly double Angle;
            public readonly Dialog Status;
        }
    }
}