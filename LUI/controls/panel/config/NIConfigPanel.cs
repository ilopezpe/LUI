using lasercom.gpib;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    class NIConfigPanel : LuiObjectConfigPanel<GpibProviderParameters>
    {
        readonly LabeledControl<ComboBox> NIBoardNumber;

        public NIConfigPanel()
        {
            NIBoardNumber = new LabeledControl<ComboBox>(new ComboBox(), "Board:");
            NIBoardNumber.Control.DropDownStyle = ComboBoxStyle.DropDownList;
            NIBoardNumber.Control.Items.Add(0);
            NIBoardNumber.Control.SelectedIndexChanged += (s, e) => OnOptionsChanged(s, e);
            Controls.Add(NIBoardNumber);
        }

        public override Type Target => typeof(NIGpibProvider);

        public override void CopyTo(GpibProviderParameters other)
        {
            other.BoardNumber = (int)NIBoardNumber.Control.SelectedItem;
            ;
        }

        public override void CopyFrom(GpibProviderParameters other)
        {
            NIBoardNumber.Control.SelectedItem = other.BoardNumber;
        }
    }
}