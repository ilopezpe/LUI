using LuiHardware;
using LuiHardware.camera;
using LuiHardware.syringepump;
using LUI.config;
using LUI.controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LUI.tabs
{
    public partial class TransientAbsControl : LuiTab
    {
        public enum Dialog
        {
            INITIALIZE,
            PROGRESS,
            PROGRESS_DARK,
            PROGRESS_FLASH,
            PROGRESS_TRANS,
            CALCULATE
        }

        public enum SyringePumpMode
        {
            NEVER,
            TRANS,
            ALWAYS
        }

        int _SelectedChannel = -1;

        double[] Light;

        public TransientAbsControl(LuiConfig Config) : base(Config)
        {
            InitializeComponent();
        }

        int SelectedChannel
        {
            get => _SelectedChannel;
            set
            {
                _SelectedChannel = Math.Max(Math.Min(value, Commander.Camera.Width - 1), 0);
                if (Light != null) CountsDisplay.Text = Light[_SelectedChannel].ToString("n");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            SyringePumpBox.ObjectChanged += HandleSyringePumpChanged;
            base.OnLoad(e);
        }

        public override void HandleParametersChanged(object sender, EventArgs e)
        {
            base.HandleParametersChanged(sender, e);
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

        protected override void Collect_Click(object sender, EventArgs e)
        {
            var N = (int)NScan.Value;
            SyringePumpMode SyringePump;
            if (SyringePumpNever.Checked) SyringePump = SyringePumpMode.NEVER;
            else if (SyringePumpTs.Checked) SyringePump = SyringePumpMode.TRANS;
            else SyringePump = SyringePumpMode.ALWAYS;
            Commander.BeamFlag.CloseLaserAndFlash();
            worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.ProgressChanged += WorkProgress;
            worker.RunWorkerCompleted += WorkComplete;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync(new WorkArgs(N, SyringePump, (int)Discard.Value));
            OnTaskStarted(EventArgs.Empty);
        }

        public override void OnTaskStarted(EventArgs e)
        {
            base.OnTaskStarted(e);
            SyringePumpBox.Enabled = false;
        }

        public override void OnTaskFinished(EventArgs e)
        {
            base.OnTaskFinished(e);
            SyringePumpBox.Enabled = true;
        }

        protected override void DoWork(object sender, DoWorkEventArgs e)
        {
            if (PauseCancelProgress(e, 0, Dialog.INITIALIZE)) return;

            var args = (WorkArgs)e.Argument;
            var N = args.N;

            var TotalScans = 3 * N;

            var AcqSize = Commander.Camera.AcqSize;
            var finalSize = Commander.Camera.ReadMode == AndorCamera.ReadModeImage
                ? AcqSize / Commander.Camera.Image.Height
                : AcqSize;

            if (PauseCancelProgress(e, 0, Dialog.PROGRESS_DARK)) return;

            Commander.BeamFlag.CloseLaserAndFlash();

            var DataBuffer = new int[AcqSize];
            var Dark = new double[finalSize];
            for (var i = 0; i < N; i++)
            {
                TryAcquire(DataBuffer);

                Data.ColumnSum(Dark, DataBuffer);

                if (PauseCancelProgress(e, i + 1, Dialog.PROGRESS_DARK)) return;
            }

            Data.DivideArray(Dark, N);

            if (PauseCancelProgress(e, 0, Dialog.PROGRESS_FLASH)) return;

            // Flow-flash.
            if (args.SyringePump == SyringePumpMode.ALWAYS)
            {
                OpenSyringePump(args.Discard);
                if (PauseCancelProgress(e, -1, Dialog.PROGRESS)) return;
            }

            Commander.BeamFlag.OpenFlash();

            var Ground = new double[finalSize];
            for (var i = 0; i < N; i++)
            {
                TryAcquire(DataBuffer);

                Data.ColumnSum(Ground, DataBuffer);

                if (PauseCancelProgress(e, i + 1, Dialog.PROGRESS_FLASH)) return;
            }

            Data.DivideArray(Ground, N);
            Data.SubArray(Ground, Dark);

            if (PauseCancelProgress(e, 0, Dialog.PROGRESS_TRANS)) return;

            // Flow-flash.
            if (args.SyringePump == SyringePumpMode.TRANS)
            {
                OpenSyringePump(args.Discard);
                if (PauseCancelProgress(e, -1, Dialog.PROGRESS)) return;
            }

            Commander.BeamFlag.OpenLaserAndFlash();

            var Excited = new double[finalSize];
            for (var i = 0; i < N; i++)
            {
                TryAcquire(DataBuffer);

                Data.ColumnSum(Excited, DataBuffer);

                if (PauseCancelProgress(e, i + 1, Dialog.PROGRESS_TRANS)) return;
            }

            Commander.BeamFlag.CloseLaserAndFlash();

            // Flow-flash.
            if (args.SyringePump == SyringePumpMode.TRANS || args.SyringePump == SyringePumpMode.ALWAYS) // Could close pump before last collect.
                Commander.SyringePump.SetClosed();

            Data.DivideArray(Excited, N);
            Data.SubArray(Excited, Dark);

            if (PauseCancelProgress(e, -1, Dialog.CALCULATE)) return;

            e.Result = Data.DeltaOD(Ground, Excited);
        }

        protected override void WorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var operation = (Dialog)Enum.Parse(typeof(Dialog), e.UserState.ToString());
            if (e.ProgressPercentage != -1)
                ScanProgress.Text = e.ProgressPercentage + "/" + NScan.Value;
            switch (operation)
            {
                case Dialog.INITIALIZE:
                    ProgressLabel.Text = "Initializing";
                    break;

                case Dialog.PROGRESS:
                    break;

                case Dialog.PROGRESS_DARK:
                    ProgressLabel.Text = "Collecting dark";
                    break;

                case Dialog.PROGRESS_FLASH:
                    ProgressLabel.Text = "Collecting ground";
                    break;

                case Dialog.PROGRESS_TRANS:
                    ProgressLabel.Text = "Collecting excited";
                    break;

                case Dialog.CALCULATE:
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
                Light = (double[])e.Result;
                Display();
                SelectedChannel = SelectedChannel;
                ProgressLabel.Text = "Complete";
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    if (SelectedChannel > -1)
                    {
                        if (Commander.Camera.CalibrationAscending) SelectedChannel--;
                        else SelectedChannel++;
                    }

                    RedrawLines();
                    break;

                case Keys.Right:
                    if (SelectedChannel > -1)
                    {
                        if (Commander.Camera.CalibrationAscending) SelectedChannel--;
                        else SelectedChannel++;
                    }

                    RedrawLines();
                    break;

                case Keys.Enter:
                    //TODO add calibration point
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void TBKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                var key = (Keys)e.KeyChar;

                if (!(key == Keys.Back || key == Keys.Delete)) e.Handled = true;
            }
        }

        void Display()
        {
            Graph.ClearData();
            Graph.Invalidate();

            if (Light != null) Graph.DrawPoints(Commander.Camera.Calibration, Light);

            Graph.Invalidate();
        }

        struct WorkArgs
        {
            public WorkArgs(int N, SyringePumpMode SyringePump, int Discard)
            {
                this.N = N;
                this.SyringePump = SyringePump;
                this.Discard = Discard;
            }

            public readonly int N;
            public readonly SyringePumpMode SyringePump;
            public readonly int Discard;
        }
    }
}