using lasercom;
using lasercom.syringepump;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    class HarvardSyringePumpConfigPanel : LuiObjectConfigPanel<SyringePumpParameters>
    {
        readonly LabeledControl<ComboBox> COMPort;

        public HarvardSyringePumpConfigPanel()
        {
            COMPort = new LabeledControl<ComboBox>(new ComboBox(), "COM Port:");
            Util.EnumerateSerialPorts().ForEach(x => COMPort.Control.Items.Add(x));
            COMPort.Control.SelectedIndexChanged += (s, e) => OnOptionsChanged(s, e);
            Controls.Add(COMPort);
        }

        public override Type Target => typeof(HarvardSyringePump);

        public override void CopyTo(SyringePumpParameters other)
        {
            other.PortName = (string)COMPort.Control.SelectedItem;
        }

        public override void CopyFrom(SyringePumpParameters other)
        {
            COMPort.Control.SelectedItem = other.PortName;
        }
    }
}