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
            TROA,
            TRLD,
            LDALIGN,
            LDEXT,
            CALIBRATE,
            ALIGN,
            TA,
            RESIDUALS,
            ABS
        }

        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        readonly CalibrateControl CalibrateControl;
        readonly TabPage CalibrationPage;

        readonly LuiConfig Config;
        readonly TabPage HomePage;
        readonly AbsorbanceControl AbsControl;
        readonly TabPage AbsPage;
        readonly TransientAbsControl TransientAbsControl;
        readonly TabPage TransientAbsPage;
        readonly TroaControl TROAControl;
        readonly TabPage TROAPage;
        readonly LdalignControl LDAlignControl;
        readonly TabPage LDAlignPage;
        readonly LdextinctionControl LDExtinctionControl;
        readonly TabPage LDExtinctionPage;
        readonly TrldControl TRLDControl;
        readonly TabPage TRLDPage;
        readonly ResidualsControl ResidualsControl;
        readonly TabPage ResidualsPage;
        readonly OptionsControl OptionsControl;
        readonly TabPage OptionsPage;

        readonly TabControl Tabs;


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
            AbsPage = new TabPage();
            TransientAbsPage = new TabPage();
            TROAPage = new TabPage();
            LDAlignPage = new TabPage();
            LDExtinctionPage = new TabPage();
            TRLDPage = new TabPage();
            ResidualsPage = new TabPage();
            CalibrationPage = new TabPage();
            OptionsPage = new TabPage();

            #region page properties 
            HomePage.BackColor = SystemColors.Control;
            HomePage.Margin = new Padding(2, 2, 2, 2);
            HomePage.Name = "HomePage";
            HomePage.TabIndex = 2;
            HomePage.Text = "Home";

            AbsPage.BackColor = SystemColors.Control;
            AbsPage.Margin = new Padding(2, 2, 2, 2);
            AbsPage.Name = "AbsPage";
            AbsPage.TabIndex = 3;
            AbsPage.Text = "Absorbance";

            TransientAbsPage.BackColor = SystemColors.Control;
            TransientAbsPage.Margin = new Padding(2, 2, 2, 2);
            TransientAbsPage.Name = "TransientAbsPage";
            TransientAbsPage.TabIndex = 5;
            TransientAbsPage.Text = "Transient Abs";

            TROAPage.BackColor = SystemColors.Control;
            TROAPage.Margin = new Padding(2, 2, 2, 2);
            TROAPage.Name = "TROAPage";
            TROAPage.Padding = new Padding(2, 2, 2, 2);
            TROAPage.TabIndex = 0;
            TROAPage.Text = "TROA";

            LDAlignPage.BackColor = SystemColors.Control;
            LDAlignPage.Margin = new Padding(2, 2, 2, 2);
            LDAlignPage.Name = "LDAlignPage";
            LDAlignPage.Padding = new Padding(2, 2, 2, 2);
            LDAlignPage.TabIndex = 0;
            LDAlignPage.Text = "LD Align";

            LDExtinctionPage.BackColor = SystemColors.Control;
            LDExtinctionPage.Margin = new Padding(2, 2, 2, 2);
            LDExtinctionPage.Name = "LDExtinctionPage";
            LDExtinctionPage.Padding = new Padding(2, 2, 2, 2);
            LDExtinctionPage.TabIndex = 0;
            LDExtinctionPage.Text = "Extinction";

            TRLDPage.BackColor = SystemColors.Control;
            TRLDPage.Margin = new Padding(2, 2, 2, 2);
            TRLDPage.Name = "TRLDPage";
            TRLDPage.Padding = new Padding(2, 2, 2, 2);
            TRLDPage.TabIndex = 0;
            TRLDPage.Text = "TRLD";

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
            CalibrationPage.Text = "WL Cal";

            OptionsPage.BackColor = SystemColors.Control;
            OptionsPage.Margin = new Padding(2, 2, 2, 2);
            OptionsPage.Name = "OptionsPage";
            OptionsPage.TabIndex = 7;
            OptionsPage.Text = "Options";

            Tabs.TabPages.Add(HomePage);
            Tabs.TabPages.Add(AbsPage);
            Tabs.TabPages.Add(TransientAbsPage);
            Tabs.TabPages.Add(TROAPage);
            Tabs.TabPages.Add(LDAlignPage);
            Tabs.TabPages.Add(LDExtinctionPage);
            Tabs.TabPages.Add(TRLDPage);
            Tabs.TabPages.Add(ResidualsPage);
            Tabs.TabPages.Add(CalibrationPage);
            Tabs.TabPages.Add(OptionsPage);
            #endregion

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

            #region add controls
            HomePage.Controls.Add(new Panel()); // Just a placeholder.

            AbsControl = new AbsorbanceControl(Config);
            AbsPage.Controls.Add(AbsControl);

            TransientAbsControl = new TransientAbsControl(Config);
            TransientAbsPage.Controls.Add(TransientAbsControl);

            TROAControl = new TroaControl(Config);
            TROAPage.Controls.Add(TROAControl);

            LDAlignControl = new LdalignControl(Config);
            LDAlignPage.Controls.Add(LDAlignControl);

            LDExtinctionControl = new LdextinctionControl(Config);
            LDExtinctionPage.Controls.Add(LDExtinctionControl);

            TRLDControl = new TrldControl(Config);
            TRLDPage.Controls.Add(TRLDControl);

            CalibrateControl = new CalibrateControl(Config);
            CalibrationPage.Controls.Add(CalibrateControl);

            ResidualsControl = new ResidualsControl(Config);
            ResidualsPage.Controls.Add(ResidualsControl);

            OptionsPage.Controls.Add(OptionsControl);
            OptionsControl.OptionsApplied += HandleOptionsApplied;
            #endregion

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
                if (AbsControl.IsBusy) return TaskState.ABS;
                if (TransientAbsControl.IsBusy) return TaskState.TA;
                if (TROAControl.IsBusy) return TaskState.TROA;
                if (LDAlignControl.IsBusy) return TaskState.LDALIGN;
                if (LDExtinctionControl.IsBusy) return TaskState.LDEXT;
                if (TRLDControl.IsBusy) return TaskState.TRLD;
                if (ResidualsControl.IsBusy) return TaskState.RESIDUALS;
                if (CalibrateControl.IsBusy) return TaskState.CALIBRATE;
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
                DisableTabs(AbsPage,
                            TransientAbsPage,
                            TROAPage,
                            LDAlignPage,
                            LDExtinctionPage,
                            TRLDPage,
                            CalibrationPage,
                            ResidualsPage,
                            OptionsPage);
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

                    case TaskState.TA:
                        Task = "Transient Abs";
                        break;

                    case TaskState.TROA:
                        Task = "TROA program";
                        break;

                    case TaskState.LDALIGN:
                        Task = "LD Align program";
                        break;

                    case TaskState.LDEXT:
                        Task = "LD Extinction program";
                        break;

                    case TaskState.TRLD:
                        Task = "TRLD program";
                        break;

                    case TaskState.RESIDUALS:
                        Task = "Residuals measurement";
                        break;

                    case TaskState.CALIBRATE:
                        Task = "Calibration";
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