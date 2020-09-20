using LuiHardware;
using LuiHardware.polarizer;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    class PolarizerConfigPanel : LuiObjectConfigPanel<PolarizerParameters>
    {
        readonly LabeledControl<ComboBox> COMPort;
        readonly LabeledControl<NumericUpDown> Delay;

        public PolarizerConfigPanel()
        {
            COMPort = new LabeledControl<ComboBox>(new ComboBox(), "COM Port:");
            Util.EnumerateSerialPorts().ForEach(x => COMPort.Control.Items.Add(x));
            COMPort.Control.SelectedIndexChanged += OnOptionsChanged;
            Controls.Add(COMPort);

            Delay = new LabeledControl<NumericUpDown>(new NumericUpDown(), "Delay while moving (ms):");
            Delay.Control.Minimum = 1;
            Delay.Control.Maximum = 100000; // 2 minute maximum
            Delay.Control.Value = Polarizer.DefaultDelay;
            Delay.Control.ValueChanged += OnOptionsChanged;
            Controls.Add(Delay);
        }

        public override Type Target => typeof(Polarizer);

        public override void CopyTo(PolarizerParameters other)
        {
            other.PortName = (string)COMPort.Control.SelectedItem;
            other.Delay = (int)Delay.Control.Value;
        }

        public override void CopyFrom(PolarizerParameters other)
        {
            COMPort.Control.SelectedItem = other.PortName;
            Delay.Control.Value = other.Delay;
        }
    }
}