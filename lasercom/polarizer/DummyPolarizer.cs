using LuiHardware.objects;

namespace LuiHardware.polarizer
{
    /// <summary>
    /// Dummy polarizer object implemented using no-ops.
    /// </summary>
    public class DummyPolarizer : AbstractPolarizer
    {
        public DummyPolarizer(LuiObjectParameters p) : this()
        {
        }

        public DummyPolarizer()
        {
        }

        protected override void Dispose(bool disposing)
        {
            // Do nothing.
        }
    }
}