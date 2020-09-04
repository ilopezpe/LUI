using log4net;
using LUI.config;
using LUI.tabs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LUI
{
    /// <summary>
    ///     Windows Form containing the entire LUI application.
    ///     The TabControl's pages are populated with UserControls which handle
    ///     the various features of the application.
    /// </summary>
    public class ParentForm : Form
    {
        public enum TaskState
        {
            IDLE,
            TROS,
            CALIBRATE,
            ALIGN,
            POWER,
            RESIDUALS,
            SPEC
        }

        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        readonly CalibrateControl CalibrateControl;
        readonly TabPage CalibrationPage;

        readonly LuiConfig Config;
        readonly TabPage HomePage;
        readonly LaserPowerControl LaserPowerControl;
        readonly OptionsControl OptionsControl;
        readonly TabPage OptionsPage;
        readonly TabPage PowerPage;
        readonly ResidualsControl ResidualsControl;
        readonly TabPage ResidualsPage;
        readonly SpecControl SpecControl;
        readonly TabPage SpecPage;

        readonly TabControl Tabs;

        readonly TroaControl TROSControl;
        readonly TabPage TROSPage;

        public ParentForm(LuiConfig config)
        {
            Config = config;

            SuspendLayout();

            // Dispose resources when the form is closed;
            FormClosed += (s, e) => Config.Dispose();

            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.WindowsDefaultLocation;
            Margin = new Padding(2, 2, 2, 2);
            Name = "ParentForm";
            Text = "LUI " + Assembly.GetExecutingAssembly().GetName().Version;

            #region Setup tabs

            Tabs = new TabControl();
            Tabs.SuspendLayout();
            Tabs.Location = new Point(0, 0);
            Tabs.Margin = new Padding(2, 2, 2, 2);
            Tabs.Name = "Tabs";
            Tabs.SelectedIndex = 0;
            //Tabs.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            Tabs.Dock = DockStyle.Fill;
            Tabs.TabIndex = 0;

            HomePage = new TabPage();
            SpecPage = new TabPage();
            TROSPage = new TabPage();
            ResidualsPage = new TabPage();
            CalibrationPage = new TabPage();
            PowerPage = new TabPage();
            OptionsPage = new TabPage();

            HomePage.BackColor = SystemColors.Control;
            HomePage.Margin = new Padding(2, 2, 2, 2);
            HomePage.Name = "HomePage";
            HomePage.TabIndex = 2;
            HomePage.Text = "Home";

            SpecPage.BackColor = SystemColors.Control;
            SpecPage.Margin = new Padding(2, 2, 2, 2);
            SpecPage.Name = "SpecPage";
            TROSPage.Padding = new Padding(2, 2, 2, 2);
            SpecPage.TabIndex = 3;
            SpecPage.Text = "Spectrum";

            TROSPage.BackColor = SystemColors.Control;
            TROSPage.Margin = new Padding(2, 2, 2, 2);
            TROSPage.Name = "TROSPage";
            TROSPage.Padding = new Padding(2, 2, 2, 2);
            TROSPage.TabIndex = 0;
            TROSPage.Text = "TROS";

            ResidualsPage.BackColor = SystemColors.Control;
            ResidualsPage.Margin = new Padding(2, 2, 2, 2);
            ResidualsPage.Name = "ResidualsPage";
            ResidualsPage.TabIndex = 4;
            ResidualsPage.Text = "Residuals";

            CalibrationPage.BackColor = SystemColors.Control;
            CalibrationPage.Margin = new Padding(2, 2, 2, 2);
            CalibrationPage.Name = "CalibrationPage";
            CalibrationPage.Padding = new Padding(2, 2, 2, 2);
            CalibrationPage.TabIndex = 1;
            CalibrationPage.Text = "Calibration";

            PowerPage.BackColor = SystemColors.Control;
            PowerPage.Margin = new Padding(2, 2, 2, 2);
            PowerPage.Name = "PowerPage";
            PowerPage.TabIndex = 5;
            PowerPage.Text = "Laser Power";

            OptionsPage.BackColor = SystemColors.Control;
            OptionsPage.Margin = new Padding(2, 2, 2, 2);
            OptionsPage.Name = "OptionsPage";
            OptionsPage.TabIndex = 7;
            OptionsPage.Text = "Options";

            Tabs.TabPages.Add(HomePage);
            Tabs.TabPages.Add(SpecPage);
            Tabs.TabPages.Add(TROSPage);
            Tabs.TabPages.Add(ResidualsPage);
            Tabs.TabPages.Add(CalibrationPage);
            Tabs.TabPages.Add(PowerPage);
            Tabs.TabPages.Add(OptionsPage);

            Controls.Add(Tabs);

            Tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            Tabs.DrawItem += HandleTabDrawItem;
            Tabs.Selecting += HandleTabSelecting;
            Tabs.Selected += HandleTabSelected;

            #endregion Setup tabs

            OptionsControl = new OptionsControl(Config)
            {
                Dock = DockStyle.Fill
            };
            OptionsPage.Controls.Add(OptionsControl);
            OptionsControl.OptionsApplied += HandleOptionsApplied;

            CalibrateControl = new CalibrateControl(Config);
            CalibrationPage.Controls.Add(CalibrateControl);

            TROSControl = new TroaControl(Config);
            TROSPage.Controls.Add(TROSControl);

            LaserPowerControl = new LaserPowerControl(Config);
            PowerPage.Controls.Add(LaserPowerControl);

            ResidualsControl = new ResidualsControl(Config);
            ResidualsPage.Controls.Add(ResidualsControl);

            SpecControl = new SpecControl(Config);
            SpecPage.Controls.Add(SpecControl);

            HomePage.Controls.Add(new Panel()); // Just a placeholder.

            foreach (TabPage page in Tabs.TabPages)
            {
                if (page != HomePage && page != OptionsPage) page.Enabled = false;
                if (page.Controls[0] is LuiTab luiTab)
                {
                    luiTab.Load += (se, ev) => CalibrateControl.CalibrationChanged += luiTab.HandleCalibrationChanged;
                    FormClosing += luiTab.HandleExit;
                    luiTab.TaskStarted += HandleTaskStarted;
                    luiTab.TaskFinished += HandleTaskFinished;
                }
            }

            Tabs.SelectedTab = HomePage;

            Resize += HandleResize;

            Tabs.ResumeLayout();
            ResumeLayout();
        }

        public TaskState CurrentTask
        {
            get
            {
                if (ResidualsControl.IsBusy) return TaskState.RESIDUALS;
                if (CalibrateControl.IsBusy) return TaskState.CALIBRATE;
                if (TROSControl.IsBusy) return TaskState.TROS;
                if (LaserPowerControl.IsBusy) return TaskState.POWER;
                if (SpecControl.IsBusy) return TaskState.SPEC;
                return TaskState.IDLE;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Calculate largest dimensions.
            var height = 0;
            var width = 0;
            foreach (TabPage c in Tabs.TabPages)
            {
                height = Math.Max(height, c.Controls[0].Height);
                width = Math.Max(width, c.Controls[0].Width);
            }

            var FormSize = new Size(width + 8, height + Tabs.ItemSize.Height + 8);
            ClientSize = FormSize;

            HandleOptionsApplied(this, EventArgs.Empty);
        }

        void HandleResize(object sender, EventArgs e)
        {
            foreach (TabPage c in Tabs.TabPages) c.Controls[0].Width = ClientSize.Width;
        }

        async void HandleOptionsApplied(object sender, EventArgs e)
        {
            try
            {
                DisableTabs(TROSPage, CalibrationPage, ResidualsPage, PowerPage, SpecPage, OptionsPage);
                var Instantiation = Config.InstantiateConfigurationAsync();
                await Instantiation;
                Config.OnParametersChanged(sender, e);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("Bad configuration or no configuration.\r\nError message:\r\n" + ex.Message);
                Tabs.SelectedTab = OptionsPage;
            }
            finally
            {
                EnableTabs(Tabs.TabPages.Cast<TabPage>());
            }
        }

        void DisableTabs(params TabPage[] tabs)
        {
            DisableTabs(tabs.AsEnumerable());
        }

        void DisableTabs(IEnumerable<TabPage> tabs)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IEnumerable<TabPage>>(DisableTabs), tabs);
            }
            else
            {
                foreach (var page in tabs) page.Enabled = false;
                Tabs.Invalidate();
            }
        }

        void EnableTabs(params TabPage[] tabs)
        {
            EnableTabs(tabs.AsEnumerable());
        }

        void EnableTabs(IEnumerable<TabPage> tabs)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IEnumerable<TabPage>>(EnableTabs), tabs);
            }
            else
            {
                foreach (var page in tabs) page.Enabled = true;
                Tabs.Invalidate();
            }
        }

        void HandleTaskStarted(object sender, EventArgs e)
        {
            DisableTabs(Tabs.TabPages.Cast<TabPage>()
                .Except(Enumerable.Repeat((TabPage)((Control)sender).Parent, 1)));
        }

        void HandleTaskFinished(object sender, EventArgs e)
        {
            EnableTabs(Tabs.TabPages.Cast<TabPage>());
        }

        void HandleTabSelected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Controls[0] as LuiTab != null)
                (e.TabPage.Controls[0] as LuiTab).HandleContainingTabSelected(sender, e);
        }

        void HandleTabSelecting(object sender, TabControlCancelEventArgs e)
        {
            if (!e.TabPage.Enabled) e.Cancel = true;
        }

        void HandleTabDrawItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = sender as TabControl;
            var tabPage = tabControl.TabPages[e.Index];

            if (!tabPage.Enabled)
                using (var brush =
                    new SolidBrush(SystemColors.GrayText))
                {
                    e.Graphics.DrawString(tabPage.Text, tabPage.Font, brush,
                        e.Bounds.X + 3, e.Bounds.Y + 3);
                }
            else
                using (var brush = new SolidBrush(tabPage.ForeColor))
                {
                    e.Graphics.DrawString(tabPage.Text, tabPage.Font, brush,
                        e.Bounds.X + 3, e.Bounds.Y + 3);
                }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (CurrentTask != TaskState.IDLE)
            {
                var BusyMsg = " is still running. Please abort if you wish to exit.";
                string Task = null;
                switch (CurrentTask)
                {
                    case TaskState.ALIGN:
                        Task = "Alignment";
                        break;

                    case TaskState.CALIBRATE:
                        Task = "Calibration";
                        break;

                    case TaskState.POWER:
                        Task = "Laser power";
                        break;

                    case TaskState.RESIDUALS:
                        Task = "Residuals measurement";
                        break;

                    case TaskState.TROS:
                        Task = "TROS program";
                        break;
                }

                var result = MessageBox.Show(Task + BusyMsg,
                    "Task Running",
                    MessageBoxButtons.OK);
                e.Cancel = true;
                return; // Don't call raise FormClosing event.
            }

            if (!Config.Saved)
            {
                var result = MessageBox.Show("Configuration has not been saved. Quit anyway?",
                    "Save Configuration",
                    MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return; // Don't call raise FormClosing event.
                }
            }

            // Proceed with closing (save tab state, etc.).
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Config.Saved) Config.Save(); // Saves TabSettings.
            base.OnFormClosed(e);
        }
    }
}