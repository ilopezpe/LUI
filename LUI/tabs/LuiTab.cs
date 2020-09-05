using Extensions;
using lasercom;
using lasercom.camera;
using lasercom.syringepump;
using lasercom.beamflags;
using lasercom.objects;
using log4net;
using LUI.config;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace LUI.tabs
{
    public partial class LuiTab : UserControl
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly ManualResetEvent Paused;
        protected Dispatcher Dispatcher;

        protected BackgroundWorker ioWorker;
        protected bool wait;
        protected BackgroundWorker worker;

        public LuiTab(LuiConfig config)
        {
            Config = config;
            Commander = new Commander();

            InitializeComponent();
            Init();

            Paused = new ManualResetEvent(true);
            CameraGain.ValueChanged += CameraGain_ValueChanged;
            Collect.Click += Collect_Click;
            Abort.Click += Abort_Click;
            Pause.Click += Pause_Click;
            Clear.Click += Clear_Click;
            OpenLaser.Click += OpenLaser_Click;
            CloseLaser.Click += CloseLaser_Click;
            OpenLamp.Click += OpenLamp_Click;
            CloseLamp.Click += CloseLamp_Click;
            Graph.MouseClick += Graph_Click;

            Abort.Enabled = Pause.Enabled = false;
        }

        public LuiTab() : this(null)
        {
        }

        public Commander Commander { get; set; }
        public LuiConfig Config { get; set; }

        protected uint CameraStatusCode
        {
            set
            {
                if (InvokeRequired)
                    BeginInvoke(new Action(() =>
                        CameraStatus.Text = Commander.Camera.DecodeStatus(value)));
                else
                    CameraStatus.Text = Commander.Camera.DecodeStatus(value);
            }
        }

        public bool IsBusy => worker != null && worker.IsBusy;

        public event EventHandler TaskStarted;

        public event EventHandler TaskFinished;

        void Init()
        {
            SuspendLayout();

            //StatusBox.Dock = DockStyle.None;
            //var mw = Math.Max(Math.Max(StatusBox.Width, CommandsBox.Width), CommonObjectPanel.Width);
            //var sz = RightPanel.MinimumSize;
            //sz.Width = mw;
            //RightPanel.MinimumSize = sz;
            //StatusBox.Dock = DockStyle.Top;
            var screenSize = Screen.PrimaryScreen.WorkingArea.Size;
            LeftPanel.MaximumSize = new Size((int)(screenSize.Width * 0.85), 0);

            Width = LeftPanel.Width + RightPanel.Width;

            ResumeLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CameraBox.ObjectChanged += HandleCameraChanged;
            BeamFlagBox.ObjectChanged += HandleBeamFlagsChanged;

            if (!IsInDesignMode())
            {
                Config.ParametersChanged += HandleParametersChanged;
                HandleParametersChanged(this, EventArgs.Empty);
                LoadSettings();
            }

            Graph.XLeft = (float)Math.Min(Commander.Camera.Calibration[0],
                Commander.Camera.Calibration[Commander.Camera.Calibration.Length - 1]);
            Graph.XRight = (float)Math.Max(Commander.Camera.Calibration[0],
                Commander.Camera.Calibration[Commander.Camera.Calibration.Length - 1]);
        }

        public virtual void HandleParametersChanged(object sender, EventArgs e)
        {
            var selectedCamera = CameraBox.SelectedObject;
            CameraBox.Objects.Items.Clear();
            foreach (var p in Config.GetParameters(typeof(CameraParameters)))
                CameraBox.Objects.Items.Add(p);
            // One of next two lines will trigger CameraChanged event.
            CameraBox.SelectedObject = selectedCamera;
            if (CameraBox.Objects.SelectedItem == null) CameraBox.Objects.SelectedIndex = 0;

            var selectedBeamFlags = BeamFlagBox.SelectedObject;
            BeamFlagBox.Objects.Items.Clear();
            foreach (var p in Config.GetParameters(typeof(BeamFlagsParameters)))
                BeamFlagBox.Objects.Items.Add(p);
            BeamFlagBox.SelectedObject = selectedBeamFlags;
            if (BeamFlagBox.Objects.SelectedItem == null) BeamFlagBox.Objects.SelectedIndex = 0;
        }

        public virtual void HandleCameraChanged(object sender, EventArgs e)
        {
            // Replace the Camera property in the Commander.
            if (CameraBox.SelectedObject != null)
                Commander.Camera = (ICamera)Config.GetObject(CameraBox.SelectedObject);

            if (Commander.Camera != null)
            {
                UpdateCollectText();
                if (Commander.Camera.HasIntensifier)
                {
                    CameraGain.Enabled = true;
                    CameraGain.Minimum = Commander.Camera.MinIntensifierGain;
                    CameraGain.Maximum = Commander.Camera.MaxIntensifierGain;
                    CameraGain.Value = Commander.Camera.IntensifierGain;
                }
            }
            else
            {
                CameraGain.Enabled = false;
                CameraGain.Minimum = 0;
                CameraGain.Maximum = 0;
                CameraGain.Value = 0;
            }

            // Update the graph with new camera's calibrated X-axis.
            HandleCalibrationChanged(sender, new LuiObjectParametersEventArgs(CameraBox.SelectedObject));
        }

        public virtual void HandleCalibrationChanged(object sender, LuiObjectParametersEventArgs e)
        {
            // If a different camera is selected, do nothing (until that camera is selected by the user).
            if (!CameraBox.SelectedObject.Equals(e.Argument)) return;

            Graph.XLeft = (float)Math.Min(Commander.Camera.Calibration[0],
                Commander.Camera.Calibration[Commander.Camera.Calibration.Length - 1]);
            Graph.XRight = (float)Math.Max(Commander.Camera.Calibration[0],
                Commander.Camera.Calibration[Commander.Camera.Calibration.Length - 1]);
            Graph.ClearAxes();
            Graph.Invalidate();
        }

        public virtual void HandleBeamFlagsChanged(object sender, EventArgs e)
        {
            if (Commander.BeamFlag != null)
                Commander.BeamFlag.CloseLaserAndFlash();
            if (BeamFlagBox.SelectedObject != null)
                Commander.BeamFlag = (IBeamFlags)Config.GetObject(BeamFlagBox.SelectedObject);
        }

        public virtual void HandleContainingTabSelected(object sender, EventArgs e)
        {
            if (Commander.Camera != null)
            {
                UpdateCollectText();
                if (Commander.Camera.HasIntensifier)
                    CameraGain.Value = Commander.Camera.IntensifierGain;
            }
        }

        public void HandleExit(object sender, EventArgs e)
        {
            if (Config.Saved) SaveSettings();
        }

        protected virtual void LoadSettings()
        {
            var Settings = Config.TabSettings[GetType().Name];
            if (Settings.TryGetValue("Camera", out var value) && value != null && value != "")
                CameraBox.SelectedObject = Config.GetFirstParameters(typeof(CameraParameters), value);
            if (Settings.TryGetValue("BeamFlag", out value) && value != null && value != "")
                BeamFlagBox.SelectedObject = Config.GetFirstParameters(typeof(BeamFlagsParameters), value);
        }

        protected virtual void SaveSettings()
        {
            var Settings = Config.TabSettings[GetType().Name];
            Settings["Camera"] = CameraBox.SelectedObject?.Name;
            Settings["BeamFlag"] = BeamFlagBox.SelectedObject?.Name;
        }

        protected bool PauseCancelProgress(DoWorkEventArgs e, int percentProgress, object progress)
        {
            if (CancelCheck(e)) return true; // If cancelling, set e.Cancel and return true.
            worker.ReportProgress(percentProgress, progress);
            if (WillPause())
            {
                // Going to pause.
                var OldFlashState = Commander.BeamFlag.FlashState;
                var OldLaserState = Commander.BeamFlag.LaserState;
                Commander.BeamFlag.CloseLaserAndFlash();
                var OldPumpState = Commander.Pump.CurrentState;
                Commander.Pump.SetClosed();
                WaitForResume();
                if (OldPumpState == PumpState.Open) Commander.Pump.SetOpen();
                if (OldFlashState == BeamFlagState.Open && OldLaserState == BeamFlagState.Open)
                    Commander.BeamFlag.OpenLaserAndFlash();
                else if (OldFlashState == BeamFlagState.Open)
                    Commander.BeamFlag.OpenFlash();
                else if (OldLaserState == BeamFlagState.Open)
                    Commander.BeamFlag.OpenLaser();
                worker.ReportProgress(percentProgress, progress);
            }

            return false;
        }

        protected bool CancelCheck(DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     If Paused is not set, Waits until Paused is set. Returns true if waiting occurred.
        /// </summary>
        /// <returns></returns>
        protected bool WaitForResume()
        {
            return !Paused.WaitOne(Timeout.Infinite);
        }

        protected bool WaitForResume(int timeout)
        {
            return !Paused.WaitOne(timeout);
        }

        protected bool WillPause()
        {
            return !Paused.WaitOne(0);
        }

        protected void SetupWorker()
        {
            worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.ProgressChanged += WorkProgress;
            worker.RunWorkerCompleted += WorkComplete;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
        }

        protected virtual void Collect_Click(object sender, EventArgs e)
        {
            var N = (int)NScan.Value;
            Commander.BeamFlag.CloseLaserAndFlash();
            SetupWorker();
            worker.RunWorkerAsync(N);
            OnTaskStarted(EventArgs.Empty);
        }

        protected virtual void Pause_Click(object sender, EventArgs e)
        {
            if (Paused.WaitOne(0)) // True if set (running/resumed).
            {
                Paused.Reset(); // Signal pause.
                Pause.Text = "Resume";
            }
            else
            {
                Paused.Set(); // Signal resume.
                Pause.Text = "Pause";
            }
        }

        protected virtual void Abort_Click(object sender, EventArgs e)
        {
            worker.CancelAsync();
        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            Graph.Clear();
            Graph.Invalidate();
        }

        void OpenLaser_Click(object sender, EventArgs e)
        {
            Commander.BeamFlag.OpenLaser();
        }

        void CloseLaser_Click(object sender, EventArgs e)
        {
            Commander.BeamFlag.CloseLaser();
        }

        void OpenLamp_Click(object sender, EventArgs e)
        {
            Commander.BeamFlag.OpenFlash();
        }

        void CloseLamp_Click(object sender, EventArgs e)
        {
            Commander.BeamFlag.CloseFlash();
        }

        void CameraGain_ValueChanged(object sender, EventArgs e)
        {
            Commander.Camera.IntensifierGain = (int)CameraGain.Value;
        }

        protected virtual void Graph_Click(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void WorkProgress(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual void OnTaskStarted(EventArgs e)
        {
            Collect.Enabled = NScan.Enabled = false;
            CameraBox.Enabled = false;
            BeamFlagBox.Objects.Enabled = false;
            Abort.Enabled = Pause.Enabled = true;
            Paused.Set(); // Set running/resumed.
            TaskStarted.Raise(this, e);
        }

        public virtual void OnTaskFinished(EventArgs e)
        {
            Collect.Enabled = NScan.Enabled = true;
            CameraBox.Enabled = true;
            BeamFlagBox.Objects.Enabled = true;
            Abort.Enabled = Pause.Enabled = false;
            TaskFinished.Raise(this, e);
        }

        public ParentForm.TaskState TaskBusy()
        {
            return ((ParentForm)FindForm()).CurrentTask;
        }

        public static bool IsInDesignMode()
        {
            if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1) return true;
            return false;
        }

        protected void BlockingBlankDialog()
        {
            var result = MessageBox.Show("Please insert blank",
                "Blank",
                MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel) worker.CancelAsync();
            wait = false;
        }

        protected void BlockingSampleDialog()
        {
            var result = MessageBox.Show("Please insert sample",
                "Continue",
                MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel) worker.CancelAsync();
            wait = false;
        }

        protected void OpenPump(bool discard)
        {
            Commander.Pump.SetOpen();
            if (discard) TryAcquire();
        }

        protected void TryAcquire()
        {
            var dummy = new int[Commander.Camera.AcqSize];
            TryAcquire(dummy);
        }

        protected void TryAcquire(int[] AcqBuffer)
        {
            try
            {
                CameraStatusCode = Commander.Camera.Acquire(AcqBuffer);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex);
                if (worker != null && worker.IsBusy) worker.CancelAsync();
                BeginInvoke(new Action(() =>
                    MessageBox.Show("Sensor saturation occurred. Aborting run.", "Error", MessageBoxButtons.OK)));
            }
        }

        protected void UpdateCollectText()
        {
            Collect.Text = Commander.Camera.ReadMode == AndorCamera.ReadModeFVB ? "Collect (FVB)" : "Collect (Image)";
        }
    }
}