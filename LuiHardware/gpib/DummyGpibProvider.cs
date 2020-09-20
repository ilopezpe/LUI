using LuiHardware.objects;

namespace LuiHardware.gpib
{
    public class DummyGpibProvider : AbstractGpibProvider
    {
        public DummyGpibProvider(LuiObjectParameters p) : this()
        {
        }

        public DummyGpibProvider()
        {
        }

        public override void LoggedWrite(byte address, string command)
        {
        }

        public override string LoggedQuery(byte address, string command)
        {
            return "";
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}