using LuiHardware.syringepump;
using System;

namespace LUI.controls
{
    class DummySyringePumpConfigPanel : LuiObjectConfigPanel<SyringePumpParameters>
    {
        public override Type Target => typeof(DummySyringePump);

        public override void CopyTo(SyringePumpParameters other)
        {
        }

        public override void CopyFrom(SyringePumpParameters other)
        {
        }
    }
}