using LuiHardware.camera;
using System;

namespace LUI.controls
{
    class DummyAndorCameraConfigPanel : AndorCameraConfigPanel
    {
        public override Type Target => typeof(DummyAndorCamera);

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