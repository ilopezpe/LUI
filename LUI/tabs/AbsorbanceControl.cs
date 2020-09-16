using CsvHelper;
using lasercom;
using lasercom.camera;
using lasercom.syringepump;
using lasercom.beamflags;
using lasercom.io;
using LUI.config;
using LUI.controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LUI.tabs
{
    public partial class AbsorbanceControl : LuiTab
    {
        public enum Dialog
        {
            BLANK,
            SAMPLE,
            PROGRESS,
            PROGRESS_BLANK,
            PROGRESS_DARK,
            PROGRESS_DATA,
            PROGRESS_CALC
        }

        public enum SyringePumpMode
        {
            ALWAYS,
            NEVER
        }

        int _SelectedChannel = -1;
        int[] BlankBuffer;

        public AbsorbanceControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();

            CurvesView.Graph = Graph;

            SaveData.Click += (sender, e) => SaveOutput();
            SaveData.Enabled = false;

            ClearBlank.Click += ClearBlank_Click;
            ClearBlank.Enabled = false;
        }

        int SelectedChannel
        {
            get => _SelectedChannel;
            set
            {
                _SelectedChannel = Math.Max(Math.Min(value, Commander.Camera.Width - 1), 0);
                if (CurvesView.SelectedCurve != null)
                    CountsDisplay.Text = CurvesView.SelectedCurve[_SelectedChannel].ToString("n4");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            SyringePumpBox.ObjectChanged += HandleSyringePumpChanged;
            CurvesView.SelectionChanged += CurvesView_SelectionChanged;
            base.OnLoad(e);
        }

        public override void HandleParametersChanged(object sender, EventArgs e)
        {
            base.HandleParametersChanged(sender, e); // Takes care of ObjectSelectPanel.

            var SyringePumpsAvailable = Config.GetParameters(typeof(SyringePumpParameters));
            if (SyringePumpsAvailable.Count() > 0)
            {
                var selectedSyringePump = SyringePumpBox.SelectedObject;
                SyringePumpBox.Objects.Items.Clear();
                foreach (var p in SyringePumpsAvailable)
                    SyringePumpBox.Objects.Items.Add(p);
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

        public virtual void HandleSyringePumpChanged(object sender, EventArgs e)
        {
            if (Commander.SyringePump != null) Commander.SyringePump.SetClosed();
            Commander.SyringePump = (ISyringePump)Config.GetObject(SyringePumpBox.SelectedObject);
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            ClearBlank.Enabled = false;
            SyringePumpBox.Enabled = false;
            SaveData.Enabled = false;
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            ClearBlank.Enabled = true;
            SyringePumpBox.Enabled = true;
            SaveData.Enabled = true;
        }

        protected override void Collect_Click(object sender, EventArgs e)
        {
            var N = (int)NScan.Value;
            SyringePumpMode Mode;
            if (SyringePumpNever.Checked) Mode = SyringePumpMode.NEVER;
            else Mode = SyringePumpMode.ALWAYS;

            Commander.BeamFlag.CloseLaserAndFlash();

            SetupWorker();
            worker.RunWorkerAsync(new WorkArgs(N, Mode, Discard.Checked));
            OnTaskStarted(EventArgs.Empty);
        }

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (WorkArgs)e.Argument;
            var N = args.N;

            var AcqSize = Commander.Camera.AcqSize;
            var finalSize = Commander.Camera.ReadMode == AndorCamera.ReadModeImage
                ? AcqSize / Commander.Camera.Image.Height
                : AcqSize;

            if (PauseCancelProgress(e, 0, Dialog.PROGRESS_DARK.ToString())) return;

            var DataBuffer = new int[AcqSize];
            var DarkBuffer = new int[finalSize];

            Commander.BeamFlag.CloseLaserAndFlash();
            for (var i = 0; i < N; i++)
            {
                TryAcquire(DataBuffer);
                Data.ColumnSum(DarkBuffer, DataBuffer);
                if (PauseCancelProgress(e, i + 1, Dialog.PROGRESS_DARK.ToString())) return;
            }

            if (PauseCancelProgress(e, 0, Dialog.BLANK.ToString())) return;

            if (BlankBuffer == null || BlankBuffer.Length != Commander.Camera.AcqSize)
            {
                Invoke(new Action(BlockingBlankDialog));

                Commander.BeamFlag.OpenFlash();

                BlankBuffer = new int[finalSize];
                for (var i = 0; i < N; i++)
                {
                    TryAcquire(DataBuffer);
                    Data.ColumnSum(BlankBuffer, DataBuffer);
                    if (PauseCancelProgress(e, i + 1, Dialog.PROGRESS_BLANK.ToString())) return;
                }

                Commander.BeamFlag.CloseLaserAndFlash();

                if (PauseCancelProgress(e, 0, Dialog.SAMPLE.ToString())) return;

                Invoke(new Action(BlockingSampleDialog));
            }
            else
            {
                if (PauseCancelProgress(e, 0, Dialog.SAMPLE.ToString())) return;
            }

            if (PauseCancelProgress(e, 0, Dialog.PROGRESS_DATA.ToString())) return;

            Commander.BeamFlag.OpenFlash();

            if (args.SyringePump == SyringePumpMode.ALWAYS) OpenSyringePump(args.DiscardFirst);

            var SampleBuffer = new int[finalSize];
            for (var i = 0; i < N; i++)
            {
                TryAcquire(DataBuffer);
                Data.ColumnSum(SampleBuffer, DataBuffer);
                if (PauseCancelProgress(e, i + 1, Dialog.PROGRESS_DATA.ToString())) return;
            }

            Commander.BeamFlag.CloseLaserAndFlash();

            if (args.SyringePump == SyringePumpMode.ALWAYS) Commander.SyringePump.SetClosed();

            if (PauseCancelProgress(e, -1, Dialog.PROGRESS_CALC.ToString())) return;
            e.Result = Data.OpticalDensity(SampleBuffer, BlankBuffer, DarkBuffer);
        }

        protected override void WorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var operation = (Dialog)Enum.Parse(typeof(Dialog), (string)e.UserState);
            if (e.ProgressPercentage != -1)
                ScanProgress.Text = e.ProgressPercentage + "/" + NScan.Value;
            switch (operation)
            {
                case Dialog.BLANK:
                    ProgressLabel.Text = "Waiting";
                    break;

                case Dialog.SAMPLE:
                    ProgressLabel.Text = "Waiting";
                    break;

                case Dialog.PROGRESS:
                    ProgressLabel.Text = "Busy";
                    break;

                case Dialog.PROGRESS_BLANK:
                    ProgressLabel.Text = "Collecting blank";
                    break;

                case Dialog.PROGRESS_DARK:
                    ProgressLabel.Text = "Collecting dark";
                    break;

                case Dialog.PROGRESS_DATA:
                    ProgressLabel.Text = "Collecting data";
                    break;

                case Dialog.PROGRESS_CALC:
                    ProgressLabel.Text = "Calculating";
                    break;
            }
        }

        protected override void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Commander.BeamFlag.CloseLaserAndFlash();
            Commander.SyringePump.SetClosed();
            if (!e.Cancelled)
            {
                CurvesView.Add(Commander.Camera.Calibration, (double[])e.Result);
                ProgressLabel.Text = "Complete";
                SaveData.Enabled = true;
            }
            else
            {
                ProgressLabel.Text = "Aborted";
            }

            OnTaskFinished(EventArgs.Empty);
        }

        protected override void Graph_Click(object sender, MouseEventArgs e)
        {
            // Selects a *physical channel* on the camera.
            SelectedChannel = Commander.Camera.CalibrationAscending
                ? (int)Math.Round(Graph.AxesToNormalized(Graph.ScreenToAxes(
                    new Point(e.X, e.Y))).X * (Commander.Camera.Width - 1))
                : (int)Math.Round((1 - Graph.AxesToNormalized(Graph.ScreenToAxes(
                    new Point(e.X, e.Y))).X) * (Commander.Camera.Width - 1));
            RedrawLines();
        }

        void RedrawLines()
        {
            Graph.ClearAnnotation();
            Graph.Annotate(GraphControl.Annotation.VERTLINE, Graph.ColorOrder[0],
                Commander.Camera.Calibration[SelectedChannel]);
            Graph.Invalidate();
        }

        void ClearBlank_Click(object sender, EventArgs e)
        {
            BlankBuffer = null;
            ClearBlank.Enabled = false;
        }

        void ExportCurvesToMat(string FileName)
        {
            var DataFile = new MatFile(FileName);
            foreach (var CurveName in CurvesView.SaveCurveNames)
            {
                var curve = CurvesView.FindCurveByName(CurveName);
                var V = DataFile.CreateVariable<double>(CurveName, 1, curve.Length);
                V.WriteNext(curve, 0);
            }

            DataFile.CreateVariable<double>("Wavelength", 1, Commander.Camera.Calibration.Length)
                .WriteNext(Commander.Camera.Calibration, 0);
            DataFile.CreateVariable<int>("Blank", 1, BlankBuffer.Length)
                .WriteNext(BlankBuffer, 0);
            DataFile.Dispose();
        }

        void ExportCurvesToCsv(string FileName)
        {
            using (var writer = new StreamWriter(FileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var headers = CurvesView.SaveCurveNames.ToList();
                    var curves = CurvesView.SaveCurves.ToList();
                    csv.WriteField("Wavelength");
                    csv.WriteField("Blank");
                    foreach (var header in headers) csv.WriteField(header);
                    csv.NextRecord();

                    for (var i = 0; i < curves[0].Count; i++)
                    {
                        csv.WriteField(Commander.Camera.Calibration[i]);
                        csv.WriteField(BlankBuffer[i]);
                        for (var j = 0; j < curves.Count; j++) csv.WriteField(curves[j][i]);
                        csv.NextRecord();
                    }
                }

                writer.Close();
            }
        }

        void SaveOutput()
        {
            if (CurvesView.Count == 0)
            {
                MessageBox.Show("No data available.", "Error", MessageBoxButtons.OK);
                return;
            }

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
                case 1: // MAT file.
                    ExportCurvesToMat(saveFile.FileName);
                    break;

                case 2: // CSV file.
                    ExportCurvesToCsv(saveFile.FileName);
                    break;
            }
        }

        void CurvesView_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectedChannel != -1 && CurvesView.SelectedCurve != null)
                CountsDisplay.Text = CurvesView.SelectedCurve[SelectedChannel].ToString("n4");
        }

        struct WorkArgs
        {
            public WorkArgs(int N, SyringePumpMode SyringePump, bool DiscardFirst)
            {
                this.N = N;
                this.SyringePump = SyringePump;
                this.DiscardFirst = DiscardFirst;
            }

            public readonly int N;
            public readonly SyringePumpMode SyringePump;
            public readonly bool DiscardFirst;
        }
    }
}