using lasercom.ddg;
using System;

namespace LUI.controls
{
    class DummyDigitalDelayGeneratorConfigPanel : LuiObjectConfigPanel<DelayGeneratorParameters>
    {
        public override Type Target => typeof(DummyDigitalDelayGenerator);

        public override void CopyFrom(DelayGeneratorParameters other)
        {
        }

        public override void CopyTo(DelayGeneratorParameters other)
        {
        }
    }
}