using lasercom;
using lasercom.control;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    class HarvardPumpConfigPanel : LuiObjectConfigPanel<PumpParameters>
    {
        readonly LabeledControl<ComboBox> COMPort;

        public HarvardPumpConfigPanel()
        {
            COMPort = new LabeledControl<ComboBox>(new ComboBox(), "COM Port:");
            Util.EnumerateSerialPorts().ForEach(x => COMPort.Control.Items.Add(x));
            COMPort.Control.SelectedIndexChanged += (s, e) => OnOptionsChanged(s, e);
            Controls.Add(COMPort);
        }

        public override Type Target => typeof(HarvardPump);

        public override void CopyTo(PumpParameters other)
        {
            other.PortName = (string)COMPort.Control.SelectedItem;
        }

        public override void CopyFrom(PumpParameters other)
        {
            COMPort.Control.SelectedItem = other.PortName;
        }
    }
}