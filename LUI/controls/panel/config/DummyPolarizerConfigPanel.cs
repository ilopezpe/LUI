using lasercom.polarizer;
using System;

namespace LUI.controls
{
    class DummyPolarizerConfigPanel : LuiObjectConfigPanel<PolarizerParameters>
    {
        public override Type Target => typeof(DummyPolarizer);

        public override void CopyTo(PolarizerParameters other)
        {
        }

        public override void CopyFrom(PolarizerParameters other)
        {
        }
    }
}