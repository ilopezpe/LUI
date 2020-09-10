using lasercom.objects;

namespace lasercom.polarizer
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

        public string NewAngle { get; set; }

        public override void SetAngle(float angle)
        {
            NewAngle = angle.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            // Do nothing.
        }
    }
}