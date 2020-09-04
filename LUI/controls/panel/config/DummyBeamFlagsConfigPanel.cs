using lasercom.control;
using System;

namespace LUI.controls
{
    class DummyBeamFlagsConfigPanel : LuiObjectConfigPanel<BeamFlagsParameters>
    {
        public override Type Target => typeof(DummyBeamFlags);

        public override void CopyTo(BeamFlagsParameters other)
        {
        }

        public override void CopyFrom(BeamFlagsParameters other)
        {
        }
    }
}