using lasercom;
using lasercom.control;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    class BeamFlagsConfigPanel : LuiObjectConfigPanel<BeamFlagsParameters>
    {
        readonly LabeledControl<ComboBox> COMPort;
        readonly LabeledControl<NumericUpDown> Delay;

        public BeamFlagsConfigPanel()
        {
            COMPort = new LabeledControl<ComboBox>(new ComboBox(), "COM Port:");
            Util.EnumerateSerialPorts().ForEach(x => COMPort.Control.Items.Add(x));
            COMPort.Control.SelectedIndexChanged += OnOptionsChanged;
            Controls.Add(COMPort);

            Delay = new LabeledControl<NumericUpDown>(new NumericUpDown(), "Delay after open (ms):");
            Delay.Control.Minimum = 25;
            Delay.Control.Maximum = 800;
            Delay.Control.Value = BeamFlags.DefaultDelay;
            Delay.Control.ValueChanged += OnOptionsChanged;
            Controls.Add(Delay);
        }

        public override Type Target => typeof(BeamFlags);

        public override void CopyTo(BeamFlagsParameters other)
        {
            other.PortName = (string)COMPort.Control.SelectedItem;
            other.Delay = (int)Delay.Control.Value;
        }

        public override void CopyFrom(BeamFlagsParameters other)
        {
            COMPort.Control.SelectedItem = other.PortName;
            Delay.Control.Value = other.Delay;
        }
    }
}