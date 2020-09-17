using LuiHardware.camera;
using System;

namespace LUI.controls
{
    class DummyCameraConfigPanel : CameraConfigPanel
    {
        public override Type Target => typeof(DummyCamera);

        public override void CopyFrom(CameraParameters other)
        {
            base.CopyFrom(other);
        }

        public override void CopyTo(CameraParameters other)
        {
            base.CopyTo(other);
        }
    }
}