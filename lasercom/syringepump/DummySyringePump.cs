using lasercom.objects;

namespace lasercom.syringepump
{
    public class DummySyringePump : AbstractSyringePump
    {
        public DummySyringePump(LuiObjectParameters p) : this()
        {
        }

        public DummySyringePump()
        {
            SetClosed();
        }

        protected override void Dispose(bool disposing)
        {
            // Nothing to dispose.
        }
    }
}