using Extensions;
using LuiHardware.objects;
using LUI.controls.designer;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LUI.controls
{
    [Designer(typeof(ObjectCommandPanelDesigner))]
    public class ObjectCommandPanel : UserControl
    {
        readonly LabeledControl<ComboBox> _Objects;

        readonly GroupBox Group;

        public ObjectCommandPanel()
        {
            SuspendLayout();

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoScaleMode = AutoScaleMode.Inherit;

            Group = new GroupBox
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill
            };

            Flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill
            };

            _Objects = new LabeledControl<ComboBox>(new ComboBox(), "Device:");
            Objects.DropDownStyle = ComboBoxStyle.DropDownList;
            Objects.DisplayMember = "Name";
            Objects.SelectedIndexChanged += OnObjectChanged;
            Flow.Controls.Add(_Objects);

            Group.Controls.Add(Flow);

            Controls.Add(Group);

            ResumeLayout();
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlowLayoutPanel Flow { get; }

        public ComboBox Objects => _Objects.Control;

        public LuiObjectParameters SelectedObject
        {
            get => (LuiObjectParameters)Objects.SelectedItem;
            set => Objects.SelectedItem = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        [Category("Appearance")]
        public override string Text
        {
            get => Group.Text;
            set => Group.Text = value;
        }

        public event EventHandler ObjectChanged;

        void OnObjectChanged(object sender, EventArgs e)
        {
            ObjectChanged.Raise(sender, e);
        }
    }
}