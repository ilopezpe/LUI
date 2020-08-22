using System;
using System.Drawing;
using System.Windows.Forms;
using Extensions;
using lasercom.camera;
using lasercom.control;
using lasercom.ddg;
using lasercom.gpib;
using log4net;
using LUI.config;
using LUI.controls;

namespace LUI.tabs
{
    public class OptionsControl : UserControl
    {
        protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ListView OptionsListView;
        LuiOptionsDialog ActiveDialog;
        Button ApplyConfig;
        Button SaveConfig;
        Button LoadConfig;

        LuiConfig Config;

        public event EventHandler OptionsApplied;
        
        public OptionsControl(LuiConfig config)
        {
            SuspendLayout();

            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Name = "OptionsControl";

            #region Panels and list of options dialogs
            Panel OptionsPanel = new Panel
            {
                Dock = DockStyle.Fill // Panel will fill all left-over space.
            }; // Container for options dialogs.
            Controls.Add(OptionsPanel); // Must add the DockStyle.Fill control first.

            Panel ListPanel = new Panel
            {
                Dock = DockStyle.Left // Panel will dock to the left.
            }; 
            Controls.Add(ListPanel);

            OptionsListView = new OptionsListView
            {
                Dock = DockStyle.Fill, // Fill available space.
                HideSelection = false, // Maintain highlighting if user changes control focus.
                MultiSelect = false, // Only select one item at a time.
                HeaderStyle = ColumnHeaderStyle.None,
                View = View.Details,
                ShowGroups = true
            };
            OptionsListView.Columns.Add(new ColumnHeader());
            OptionsListView.SelectedIndexChanged += HandleSelectedOptionsDialogChanged;
            ListPanel.Controls.Add(OptionsListView);
            #endregion

            #region Options dialogs
            ListViewGroup General = new ListViewGroup("General", HorizontalAlignment.Left);
            OptionsListView.Groups.Add(General);
            ListViewGroup Instruments = new ListViewGroup("Instruments", HorizontalAlignment.Left);
            OptionsListView.Groups.Add(Instruments);

            LoggingOptionsDialog LoggingOptionsDialog = new LoggingOptionsDialog(OptionsPanel.Size)
            {
                Dock = DockStyle.Fill
            };
            ListViewItem LoggingOptionsItem = new ListViewItem("Logging", General)
            {
                Tag = LoggingOptionsDialog
            };
            OptionsListView.Items.Add(LoggingOptionsItem);
            OptionsPanel.Controls.Add(LoggingOptionsDialog);

            LuiOptionsListDialog<AbstractBeamFlags,BeamFlagsParameters> BeamFlagOptionsDialog =
                new LuiOptionsListDialog<AbstractBeamFlags, BeamFlagsParameters>(OptionsPanel.Size);
            BeamFlagOptionsDialog.AddConfigPanel(new BeamFlagsConfigPanel());
            BeamFlagOptionsDialog.AddConfigPanel(new DummyBeamFlagsConfigPanel());
            BeamFlagOptionsDialog.SetDefaultSelectedItems();
            BeamFlagOptionsDialog.Dock = DockStyle.Fill;
            ListViewItem BeamFlagOptionsItem = new ListViewItem("Beam Flags", Instruments)
            {
                Tag = BeamFlagOptionsDialog
            };
            OptionsListView.Items.Add(BeamFlagOptionsItem);
            OptionsPanel.Controls.Add(BeamFlagOptionsDialog);

            LuiOptionsListDialog<ICamera, CameraParameters> CameraOptionsDialog =
                new LuiOptionsListDialog<ICamera, CameraParameters>(OptionsPanel.Size);
            CameraOptionsDialog.AddConfigPanel(new AndorCameraConfigPanel());
            CameraOptionsDialog.AddConfigPanel(new CameraTempControlledConfigPanel());
            CameraOptionsDialog.AddConfigPanel(new DummyAndorCameraConfigPanel());
            CameraOptionsDialog.AddConfigPanel(new DummyCameraConfigPanel());
            CameraOptionsDialog.SetDefaultSelectedItems();
            CameraOptionsDialog.Dock = DockStyle.Fill;
            ListViewItem CameraOptionsItem = new ListViewItem("Camera", Instruments)
            {
                Tag = CameraOptionsDialog
            };
            OptionsListView.Items.Add(CameraOptionsItem);
            OptionsPanel.Controls.Add(CameraOptionsDialog);

            var GPIBOptionsDialog = new LuiOptionsListDialog<IGpibProvider, GpibProviderParameters>(OptionsPanel.Size);
            GPIBOptionsDialog.AddConfigPanel(new NIConfigPanel());
            GPIBOptionsDialog.AddConfigPanel(new PrologixConfigPanel());
            GPIBOptionsDialog.AddConfigPanel(new DummyGpibProviderConfigPanel());
            GPIBOptionsDialog.SetDefaultSelectedItems();
            GPIBOptionsDialog.Dock = DockStyle.Fill;
            ListViewItem GPIBOptionsItem = new ListViewItem("GPIB Controllers", Instruments)
            {
                Tag = GPIBOptionsDialog
            };
            OptionsListView.Items.Add(GPIBOptionsItem);
            OptionsPanel.Controls.Add(GPIBOptionsDialog);

            var DDGOptionsDialog = new LuiOptionsListDialog<IDigitalDelayGenerator, DelayGeneratorParameters>(OptionsPanel.Size);
            DDGOptionsDialog.AddConfigPanel(new DG535ConfigPanel(GPIBOptionsDialog));
            DDGOptionsDialog.AddConfigPanel(new DummyDigitalDelayGeneratorConfigPanel());
            DDGOptionsDialog.SetDefaultSelectedItems();
            DDGOptionsDialog.Dock = DockStyle.Fill;
            ListViewItem DDGOptionsItem = new ListViewItem("Digital Delay Generators", Instruments)
            {
                Tag = DDGOptionsDialog
            };
            OptionsListView.Items.Add(DDGOptionsItem);
            OptionsPanel.Controls.Add(DDGOptionsDialog);

            var PumpOptionsDialog = new LuiOptionsListDialog<IPump, PumpParameters>(OptionsPanel.Size);
            PumpOptionsDialog.AddConfigPanel(new HarvardPumpConfigPanel());
            PumpOptionsDialog.AddConfigPanel(new DummyPumpConfigPanel());
            PumpOptionsDialog.SetDefaultSelectedItems();
            PumpOptionsDialog.Dock = DockStyle.Fill;
            ListViewItem PumpOptionsItem = new ListViewItem("Syringe Pumps", Instruments)
            {
                Tag = PumpOptionsDialog
            };
            OptionsListView.Items.Add(PumpOptionsItem);
            OptionsPanel.Controls.Add(PumpOptionsDialog);

            #endregion

            OptionsListView.Columns[0].Width = -1; // Sets width to that of widest item.

            // Note OptionsChanged and ConfigChanged handlers are not yet bound.
            Config = config; // Refers to the global config object.
            SetChildConfig(Config); // Sets options dialogs to reference & match this config.

            #region Buttons
            FlowLayoutPanel ButtonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                AutoSize = true, // Fit to the buttons.
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            }; // Container for the buttons.

            ApplyConfig = new Button
            {
                Text = "Apply",
                Size = new Size(91, 34),
                Enabled = false
            };
            ApplyConfig.Click += ApplyConfig_Click;


            SaveConfig = new Button
            {
                Text = "Save",
                Size = new Size(91, 34),
                Enabled = false
            };
            SaveConfig.Click += SaveConfig_Click;

            LoadConfig = new Button
            {
                Text = "Load",
                Size = new Size(91, 34)
            };
            LoadConfig.Click += LoadConfig_Click;

            ButtonPanel.Controls.Add(LoadConfig);
            ButtonPanel.Controls.Add(SaveConfig);
            ButtonPanel.Controls.Add(ApplyConfig);

            OptionsPanel.Controls.Add(ButtonPanel);

            ButtonPanel.BringToFront(); // Display on top of any overlapping controls (OptionsPanel).
            #endregion

            ResumeLayout(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            Config.ParametersChanged += HandleParametersChanged;

            foreach (ListViewItem item in OptionsListView.Items)
            {
                var luiOptionsDialog = (LuiOptionsDialog)item.Tag;
                // Set in OnLoad so initialization doesn't trigger the events.
                luiOptionsDialog.OptionsChanged += HandleCanApply;

                // Control initialization doesn't happen unless control is visible,
                // so we defer setting visibility until the control is loaded.
                luiOptionsDialog.Visible = false;
            }
            // Selecting the ListView causes selected item to be highlighted with system color.
            OptionsListView.Select();

            OptionsListView.Items[0].Selected = true; // Select default options dialog.
            base.OnLoad(e); // Forward to base class event handler.
        }

        private void SetChildConfig(LuiConfig config)
        {
            // Set all options dialogs to reference & match the given config.
            foreach (ListViewItem it in OptionsListView.Items)
            {
                ((LuiOptionsDialog)it.Tag).Config = config;
            }
        }

        public void ChildrenMatchConfig(LuiConfig config)
        {
            // Set all options dialogs to match the given config.
            foreach (ListViewItem it in OptionsListView.Items)
            {
                ((LuiOptionsDialog)it.Tag).MatchConfig(config);
            }
        }

        public void ChildrenMatchConfig()
        {
            ChildrenMatchConfig(this.Config);
        }

        //public void ChildrenCopyConfigState()
        //{
        //    // Update all options dialogs to to the state of their configs.
        //    foreach (ListViewItem it in OptionsListView.Items)
        //    {
        //        ((LuiOptionsDialog)it.Tag).CopyConfigState();
        //    }
        //}

        private void HandleParametersChanged(object sender, EventArgs e)
        {
            ChildrenMatchConfig();
        }

        private void HandleCanApply(object sender, EventArgs e)
        {
            ApplyConfig.Enabled = true; // Can apply after options changed
            SaveConfig.Enabled = false; // Can't save until new options applied.
        }

        private void ApplyConfig_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in OptionsListView.Items) ((LuiOptionsDialog)item.Tag).HandleApply(sender, e);
            OptionsApplied.Raise(this, EventArgs.Empty);
            ApplyConfig.Enabled = false; // Can't apply again until options change.
            SaveConfig.Enabled = true; // Can save config after apply.
        }

        private void SaveConfig_Click(object sender, EventArgs e)
        {
            Config.Save();
            SaveConfig.Enabled = false; // Can't save again until new changes made and applied.
        }

        private void LoadConfig_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "XML Files|*.xml",
                FileName = "config.xml"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Update options dialogs to match the new config.
                // The new config is not instantiated and the old config is not replaced.
                ChildrenMatchConfig(LuiConfig.FromFile(ofd.FileName));
                HandleCanApply(sender, e);
            }
        }

        private void HandleSelectedOptionsDialogChanged(object sender, EventArgs e)
        {
            // The SelectedIndexChanged event of the ListView will trigger twice:
            // once as the previous item is deselected, and again as the new item is selected.
            // We skip the first event call by checking for zero selected items.
            if (OptionsListView.SelectedItems.Count != 0)
            {
                if (ActiveDialog != null) ActiveDialog.Visible = false; // Hide previously active dialog.
                ListViewItem selectedItem = OptionsListView.SelectedItems[0];
                ActiveDialog = (LuiOptionsDialog)selectedItem.Tag; // Update the active dialog.

                if (ActiveDialog != null)
                {
                    ActiveDialog.Visible = true; // Show newly active dialog.
                    //ActiveDialog.OnSetActive(); // Hypothetical so far.
                }
            }
        }

    }
}
