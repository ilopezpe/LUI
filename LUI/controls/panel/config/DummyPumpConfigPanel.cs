using lasercom.control;
using System;

namespace LUI.controls
{
    class DummyPumpConfigPanel : LuiObjectConfigPanel<PumpParameters>
    {
        public override Type Target => typeof(DummyPump);

        public override void CopyTo(PumpParameters other)
        {
        }

        public override void CopyFrom(PumpParameters other)
        {
        }
    }
}