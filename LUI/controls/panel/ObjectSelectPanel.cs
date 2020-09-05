using Extensions;
using lasercom.camera;
using lasercom.syringepump;
using lasercom.beamflags;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    public class ObjectSelectPanel : FlowLayoutPanel
    {
        readonly LabeledControl<ComboBox> _BeamFlags;

        readonly LabeledControl<ComboBox> _Cameras;

        public ObjectSelectPanel()
        {
            FlowDirection = FlowDirection.LeftToRight;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _Cameras = new LabeledControl<ComboBox>(new ComboBox(), "Camera:");
            Cameras.DropDownStyle = ComboBoxStyle.DropDownList;
            Cameras.DisplayMember = "Name";
            Cameras.SelectedIndexChanged += (s, e) => CameraChanged.Raise(s, e);
            Controls.Add(_Cameras);

            _BeamFlags = new LabeledControl<ComboBox>(new ComboBox(), "Beam Flags:");
            BeamFlags.DropDownStyle = ComboBoxStyle.DropDownList;
            BeamFlags.DisplayMember = "Name";
            BeamFlags.SelectedIndexChanged += (s, e) => BeamFlagsChanged.Raise(s, e);
            Controls.Add(_BeamFlags);
        }

        public ComboBox Cameras => _Cameras.Control;

        public CameraParameters SelectedCamera
        {
            get => (CameraParameters)Cameras.SelectedItem;
            set => Cameras.SelectedItem = value;
        }

        public ComboBox BeamFlags => _BeamFlags.Control;

        public BeamFlagsParameters SelectedBeamFlags
        {
            get => (BeamFlagsParameters)BeamFlags.SelectedItem;
            set => BeamFlags.SelectedItem = value;
        }

        public event EventHandler CameraChanged;

        public event EventHandler BeamFlagsChanged;
    }
}