using Extensions;
using LuiHardware.objects;
using LUI.config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LUI.controls
{
    public class LuiOptionsListDialog<T, P> : LuiOptionsDialog where P : LuiObjectParameters<P>
    {
        Button Add, Remove;

        Dictionary<Type, LuiObjectConfigPanel<P>> ConfigPanels;

        Panel ConfigSubPanel;
        LabeledControl<TextBox> ObjectName;
        LabeledControl<ComboBox> ObjectTypes;
        ListView ObjectView;

        public LuiOptionsListDialog()
        {
            AutoScaleMode = AutoScaleMode.Inherit;
            Init();
        }

        public LuiOptionsListDialog(Size size, bool visibility)
        {
            AutoScaleMode = AutoScaleMode.Inherit;
            Size = size;
            Visible = visibility;
            Init();
        }

        public LuiOptionsListDialog(Size size) : this(size, true)
        {
        }

        public IEnumerable<P> PersistentItems
        {
            get
            {
                // Skip the "New..." row.
                for (var i = 0; i < ObjectView.Items.Count - 1; i++)
                {
                    var it = (LuiObjectItem)ObjectView.Items[i];
                    yield return it.Persistent;
                }
            }
            set
            {
                ObjectView.Items.Clear();
                AddDummyItem(); // Add the "New..." row.
                foreach (var luiParameters in value) AddObject(luiParameters);
                SetDefaultSelectedItems();
            }
        }

        public IEnumerable<P> TransientItems
        {
            get
            {
                for (var i = 0; i < ObjectView.Items.Count - 1; i++)
                    yield return ((LuiObjectItem)ObjectView.Items[i]).Transient;
            }
        }

        void Init()
        {
            SuspendLayout();

            #region Object list and configuration panel setup

            var ConfigPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(ConfigPanel);

            var ListPanel = new Panel
            {
                Dock = DockStyle.Left
            };
            Controls.Add(ListPanel);

            ObjectView = new OptionsListView
            {
                HeaderStyle = ColumnHeaderStyle.None,
                View = View.Details,
                ShowGroups = false,
                Dock = DockStyle.Top,
                HideSelection = false,
                MultiSelect = false
            };
            ObjectView.Columns.Add(new ColumnHeader());
            ObjectView.Columns[0].Width = ObjectView.Width;
            ObjectView.SelectedIndexChanged += SelectedObjectChanged;
            //ObjectView.ItemSelectionChanged += SelectedObjectChanged;
            AddDummyItem(); // Add the "New..." row.

            var ListControlsPanel = new Panel
            {
                Dock = DockStyle.Top
            };

            #region Buttons

            Add = new Button
            {
                Dock = DockStyle.Left,
                Text = "Add"
            };
            Add.Click += Add_Click;
            Add.Click += (s, e) => OnOptionsChanged(s, e);

            Remove = new Button
            {
                Dock = DockStyle.Left,
                Text = "Remove"
            };
            Remove.Click += Remove_Click;
            Remove.Click += (s, e) => OnOptionsChanged(s, e);

            ListControlsPanel.Controls.Add(Remove);
            ListControlsPanel.Controls.Add(Add);

            #endregion Buttons

            ListPanel.Controls.Add(ListControlsPanel);
            ListPanel.Controls.Add(ObjectView);

            #endregion Object list and configuration panel setup

            #region Configuration panel

            ConfigSubPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            ConfigPanel.Controls.Add(ConfigSubPanel);

            ObjectTypes = new LabeledControl<ComboBox>(new ComboBox(), "Type:")
            {
                Dock = DockStyle.Top
            };
            ObjectTypes.Control.DropDownStyle = ComboBoxStyle.DropDownList;
            ObjectTypes.Control.DisplayMember = "Name";

            var AvailableTypes = typeof(T).GetSubclasses(true);
            AvailableTypes.Sort((x, y) => x.Name.CompareTo(y.Name));
            AvailableTypes.ForEach(x => { ObjectTypes.Control.Items.Add(x); });
            ObjectTypes.Control.SelectedIndex = 0;
            ObjectTypes.Control.SelectedIndexChanged += SelectedObjectTypeChanged;
            ObjectTypes.Control.SelectionChangeCommitted += OnOptionsChanged; // Caused by user input.

            ObjectName = new LabeledControl<TextBox>(new TextBox(), "Name:")
            {
                Dock = DockStyle.Top
            };
            ObjectName.Control.TextChanged += SelectedObjectNameChanged;
            ObjectName.Control.KeyDown += OnOptionsChanged; // Caused by user input.
            ConfigPanel.Controls.Add(ObjectName);
            ConfigPanel.Controls.Add(ObjectTypes);

            #endregion Configuration panel

            ConfigPanels = new Dictionary<Type, LuiObjectConfigPanel<P>>();

            ConfigChanged += HandleConfigChanged;

            ResumeLayout(false);
        }

        /// <summary>
        ///     Adds the "New..." item dummy.
        /// </summary>
        void AddDummyItem()
        {
            var nextItem = new LuiObjectItem("New..."); // Dummy item
            ObjectView.Items.Add(nextItem);
        }

        public void AddConfigPanel(LuiObjectConfigPanel<P> c)
        {
            c.FlowDirection = FlowDirection.TopDown;
            c.Dock = DockStyle.Fill;
            c.Visible = false;
            c.OptionsChanged += UpdateSelectedObject;
            c.OptionsChanged += (s, e) => OnOptionsChanged(s, e);
            ConfigSubPanel.Controls.Add(c);
            ConfigPanels.Add(c.Target, c);
        }

        public void SetDefaultSelectedItems()
        {
            ConfigPanels[(Type)ObjectTypes.Control.SelectedItem].Visible = true;
            ObjectView.SelectedIndices.Add(ObjectView.Items.Count - 1);
        }

        void UpdateSelectedObject(object sender, EventArgs e)
        {
            var selectedItem = (LuiObjectItem)ObjectView.SelectedItems[0];
            var luiParameters = selectedItem.Transient;
            ConfigPanels[luiParameters.Type].CopyTo(luiParameters);
        }

        void SelectedObjectChanged(object sender, EventArgs e)
        {
            if (ObjectView.SelectedIndices.Count == 0)
                return;

            var selectedItem = (LuiObjectItem)ObjectView.SelectedItems[0];
            if (selectedItem.Index == ObjectView.Items.Count - 1)
            {
                Remove.Enabled = false;
                Add.Enabled = true;

                if (selectedItem.Transient == null)
                {
                    selectedItem.Transient = (P)Activator.CreateInstance(typeof(P));
                    selectedItem.Transient.Type = (Type)ObjectTypes.Control.SelectedItem;
                }
            }
            else
            {
                Remove.Enabled = true;
                Add.Enabled = false;
            }

            var p = selectedItem.Transient;
            ConfigPanels[p.Type].TriggerEvents = false; // Deactivate LuiConfigPanel's OnOptionsChanged.
            ConfigPanels[p.Type].CopyFrom(p); // No OnOptionsChanged => Apply button not enabled.
            ConfigPanels[p.Type].TriggerEvents = true; // Reactivate OnOptionsChanged.
            ObjectTypes.Control.SelectedItem = p.Type;
            ObjectName.Control.Text = p.Name;
        }

        void ShowConfigPanel(Type type)
        {
            foreach (var t in ConfigPanels.Keys)
                if (t != type)
                    ConfigPanels[t].Visible = false;
            ConfigPanels[type].Visible = true;
        }

        void SelectedObjectTypeChanged(object sender, EventArgs e)
        {
            var selectedItem = (LuiObjectItem)ObjectView.SelectedItems[0];
            var t = (Type)ObjectTypes.Control.SelectedItem;
            selectedItem.Transient.Type = t;
            ShowConfigPanel(t);
        }

        void SelectedObjectNameChanged(object sender, EventArgs e)
        {
            var selectedItem = (LuiObjectItem)ObjectView.SelectedItems[0];
            selectedItem.Transient.Name = ObjectName.Control.Text;
            if (selectedItem.Index != ObjectView.Items.Count - 1) // If not the "New..." item.
                selectedItem.Text = selectedItem.Transient.Name;
        }

        void Remove_Click(object sender, EventArgs e)
        {
            var selectedItem = (LuiObjectItem)ObjectView.SelectedItems[0];
            var idx = Math.Max(selectedItem.Index - 1, 0);
            selectedItem.Selected = false;
            selectedItem.Remove();
            ObjectView.Items[idx].Selected = true;
        }

        void Add_Click(object sender, EventArgs e)
        {
            var dummyRow = (LuiObjectItem)ObjectView.Items[ObjectView.Items.Count - 1];
            AddObject(dummyRow.Transient);
            dummyRow.Transient = null;
            dummyRow.Selected = false;
            ObjectView.Items[dummyRow.Index - 1].Selected = true;
        }

        public void AddObject(P p)
        {
            var newItem = new LuiObjectItem(p.Name)
            {
                Transient = Activator.CreateInstance(typeof(P), p) as P,
                Persistent = p
            };
            ObjectView.Items.Insert(ObjectView.Items.Count - 1, newItem);
        }

        public void Restore()
        {
            throw new NotImplementedException();
        }

        public override void HandleApply(object sender, EventArgs e)
        {
            Config.ReplaceParameters(TransientItems);
        }

        public override void CopyConfigState(LuiConfig config)
        {
            PersistentItems = config.GetParameters<P>();
        }

        public override void CopyConfigState()
        {
            CopyConfigState(Config);
        }

        public override void HandleConfigChanged(object sender, EventArgs e)
        {
            MatchConfig(Config);
        }

        protected override void OnOptionsChanged(object sender, EventArgs e)
        {
            var dummyRow = (LuiObjectItem)ObjectView.Items[ObjectView.Items.Count - 1];
            // If the "New..." item is selected, skip event unless
            // event sent by Remove button. (Correctly enables Apply button).
            if (dummyRow.Selected && sender != Remove) return;
            base.OnOptionsChanged(sender, e);
        }

        /// <summary>
        ///     Extends ListViewItem to hold two generic parameter objects.
        ///     Persistent will be restored
        /// </summary>
        class LuiObjectItem : ListViewItem
        {
            public P Persistent;
            public P Transient;

            public LuiObjectItem(string text) : base(text)
            {
            }
        }
    }
}