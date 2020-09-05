using lasercom.objects;

namespace lasercom.syringepump
{
    public class DummyPump : AbstractPump
    {
        //public DummyPump(LuiObjectParameters p) : this()
        //{
        //}

        public DummyPump()
        {
            SetClosed();
        }

        protected override void Dispose(bool disposing)
        {
            // Nothing to dispose.
        }
    }
}