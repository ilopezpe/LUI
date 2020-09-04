using lasercom.gpib;
using System;

namespace LUI.controls
{
    public class DummyGpibProviderConfigPanel : LuiObjectConfigPanel<GpibProviderParameters>
    {
        public override Type Target => typeof(DummyGpibProvider);

        public override void CopyTo(GpibProviderParameters other)
        {
        }

        public override void CopyFrom(GpibProviderParameters other)
        {
        }
    }
}