﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using lasercom;
using lasercom.io;

namespace LUI.controls
{
    public partial class LaserPowerControl : LUIControl
    {
        private BackgroundWorker ioWorker;
        private Dispatcher Dispatcher;

        double[] Light = null;

        int _SelectedChannel = -1;
        int SelectedChannel
        {
            get
            {
                return _SelectedChannel;
            }
            set
            {
                _SelectedChannel = Math.Max(Math.Min(value, (int)Commander.Camera.Width - 1), 0);
                if (Light != null) CountsDisplay.Text = Light[_SelectedChannel].ToString("n");
            }
        }

        public enum Dialog
        {
            BLANK, SAMPLE, PROGRESS, PROGRESS_BLANK, PROGRESS_DARK, PROGRESS_DATA,
            PROGRESS_CALC
        }

        public LaserPowerControl(Commander commander)
        {
            InitializeComponent();
            Commander = commander;
            Load += HandleLoad;
            Graph.MouseClick += new MouseEventHandler(Graph_Click);
        }

        void HandleLoad(object sender, EventArgs e)
        {
            Commander.CalibrationChanged += HandleCalibrationChanged;
            Graph.XLeft = (float)Commander.Calibration[0];
            Graph.XRight = (float)Commander.Calibration[Commander.Calibration.Length - 1];
        }

        public void AlignmentWork(object sender, DoWorkEventArgs e)
        {
            Commander.Camera.AcquisitionMode = AndorCamera.AcquisitionModeSingle;
            Commander.Camera.TriggerMode = AndorCamera.TriggerModeExternalExposure;
            Commander.Camera.ReadMode = AndorCamera.ReadModeFVB;
            int N = (int)e.Argument;

            worker.ReportProgress(0, Dialog.BLANK.ToString());

            int[] BlankBuffer = Commander.Flash();
            for (int i = 0; i < N - 1; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Data.Accumulate(BlankBuffer, Commander.Flash());
                worker.ReportProgress((i / N) * 33, Dialog.PROGRESS_BLANK.ToString());
            }

            worker.ReportProgress(33, Dialog.SAMPLE.ToString());

            wait = true;
            bool[] waitparam = { wait };
            Dispatcher.BeginInvoke(new Action(BlockingBlankDialog), waitparam);
            while (wait) ;

            worker.ReportProgress(33, Dialog.PROGRESS_DARK.ToString());

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            int[] DarkBuffer = Commander.Dark();
            for (int i = 0; i < N - 1; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Data.Accumulate(DarkBuffer, Commander.Dark());
                worker.ReportProgress(33 + (i / N) * 33, Dialog.PROGRESS_DARK.ToString());
            }

            worker.ReportProgress(66, Dialog.PROGRESS.ToString());
            wait = true;
            Dispatcher.BeginInvoke(new Action(BlockingSampleDialog), new bool[] { wait });
            while (wait) ;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            worker.ReportProgress(66, Dialog.PROGRESS_DATA.ToString());

            int[] DataBuffer = Commander.Trans();
            for (int i = 0; i < N - 1; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Data.Accumulate(DataBuffer, Commander.Trans());
                worker.ReportProgress(66 + (i / N) * 33, Dialog.PROGRESS_DATA.ToString());
            }
            worker.ReportProgress(99, Dialog.PROGRESS_CALC.ToString());
            Data.Dissipate(DataBuffer, DarkBuffer);
            worker.ReportProgress(100, Dialog.PROGRESS_CALC.ToString());
            e.Result = DataBuffer;
        }

        private void Collect_Click(object sender, EventArgs e)
        {
            Collect.Enabled = false;
            int N = (int)Averages.Value;
            worker = new BackgroundWorker();
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(AlignmentWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(AlignmentProgress);
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync(N);
        }

        public void AlignmentProgress(object sender, ProgressChangedEventArgs e)
        {
            Dialog operation = (Dialog)Enum.Parse(typeof(Dialog), (string)e.UserState);
            switch (operation)
            {
                case Dialog.BLANK:
                    StatusProgress.Value = e.ProgressPercentage;
                    ProgressLabel.Text = "Waiting";
                    break;
                case Dialog.SAMPLE:
                    StatusProgress.Value = e.ProgressPercentage;
                    ProgressLabel.Text = "Waiting";
                    break;
                case Dialog.PROGRESS:
                    StatusProgress.Value = e.ProgressPercentage;
                    ProgressLabel.Text = "Busy";
                    break;
                case Dialog.PROGRESS_BLANK:
                    StatusProgress.Value = e.ProgressPercentage;
                    ProgressLabel.Text = "Collecting blank";
                    break;
                case Dialog.PROGRESS_DARK:
                    StatusProgress.Value = e.ProgressPercentage;
                    ProgressLabel.Text = "Collecting dark";
                    break;
                case Dialog.PROGRESS_DATA:
                    StatusProgress.Value = e.ProgressPercentage;
                    ProgressLabel.Text = "Collecting data";
                    break;
                case Dialog.PROGRESS_CALC:
                    StatusProgress.Value = e.ProgressPercentage;
                    ProgressLabel.Text = "Calculating";
                    break;
            }
        }

        public void AlignmentComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                Light = Array.ConvertAll((int[])e.Result, x => (double)x);
                Display();
                SelectedChannel = SelectedChannel;
                ProgressLabel.Text = "Complete";
            }
            else
            {
                ProgressLabel.Text = "Aborted";
            }
            StatusProgress.Value = 100;
            Collect.Enabled = true;
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            Graph.Clear();
            Graph.Invalidate();
        }

        private void Graph_Click(object sender, MouseEventArgs e)
        {
            // Selects a *physical channel* on the camera.
            SelectedChannel = (int)Math.Round(Graph.AxesToNormalized(Graph.ScreenToAxes(new Point(e.X, e.Y))).X * (Commander.Camera.Width - 1));
            RedrawLines();
        }

        private void RedrawLines()
        {
            Graph.ClearAnnotation();
            Graph.Annotate(GraphControl.Annotation.VERTLINE, Graph.ColorOrder[0], SelectedChannel);
            Graph.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    if (SelectedChannel > -1)
                    {
                        SelectedChannel--;
                    }
                    RedrawLines();
                    break;
                case Keys.Right:
                    if (SelectedChannel > -1)
                    {
                        SelectedChannel++;
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
            if (!(char.IsDigit(e.KeyChar)))
            {
                Keys key = (Keys)e.KeyChar;

                if (!(key == Keys.Back || key == Keys.Delete))
                {
                    e.Handled = true;
                }
            }
        }

        private void Display()
        {
            Graph.ClearData();
            Graph.Invalidate();

            if (Light != null)
            {
                Graph.DrawPoints(Commander.Calibration, Light);
            }

            Graph.Invalidate();
        }

        public void HandleCalibrationChanged(object sender, EventArgs args)
        {
            Graph.XLeft = (float)Commander.Calibration[0];
            Graph.XRight = (float)Commander.Calibration[Commander.Calibration.Length - 1];
            Graph.ClearAxes();
            Graph.Invalidate();
        }

    }
}
