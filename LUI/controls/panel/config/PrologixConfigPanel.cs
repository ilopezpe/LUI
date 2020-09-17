using LuiHardware;
using LuiHardware.gpib;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    class PrologixConfigPanel : LuiObjectConfigPanel<GpibProviderParameters>
    {
        readonly LabeledControl<ComboBox> PrologixCOMPort;
        readonly LabeledControl<NumericUpDown> PrologixTimeout;

        public PrologixConfigPanel()
        {
            PrologixCOMPort = new LabeledControl<ComboBox>(new ComboBox(), "COM Port:");
            Util.EnumerateSerialPorts().ForEach(x => PrologixCOMPort.Control.Items.Add(x));
            PrologixCOMPort.Control.SelectedIndexChanged += (s, e) => OnOptionsChanged(s, e);
            PrologixTimeout = new LabeledControl<NumericUpDown>(new NumericUpDown(), "Timeout (ms):");
            PrologixTimeout.Control.Maximum = 999;
            PrologixTimeout.Control.Increment = 1;
            PrologixTimeout.Control.ValueChanged += (s, e) => OnOptionsChanged(s, e);
            Controls.Add(PrologixCOMPort);
            Controls.Add(PrologixTimeout);
        }

        public override Type Target => typeof(PrologixGpibProvider);

        public override void CopyTo(GpibProviderParameters other)
        {
            other.PortName = (string)PrologixCOMPort.Control.SelectedItem;
            other.Timeout = (int)PrologixTimeout.Control.Value;
        }

        public override void CopyFrom(GpibProviderParameters other)
        {
            TriggerEvents = false;
            PrologixCOMPort.Control.SelectedItem = other.PortName;
            PrologixTimeout.Control.Value = other.Timeout;
        }
    }
}