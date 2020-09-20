using lasercom.objects;
using System;
using System.Runtime.Serialization;

namespace lasercom.polarizer
{
    [DataContract]
    public class PolarizerParameters : LuiObjectParameters<PolarizerParameters>
    {
        public PolarizerParameters(Type Type, string PortName)
            : base(Type)
        {
            this.PortName = PortName;
        }

        public PolarizerParameters(PolarizerParameters other)
            : base(other)
        {
        }

        public PolarizerParameters(Type Type)
            : base(Type)
        {
        }

        public PolarizerParameters()
        {
        }

        [DataMember] public string PortName { get; set; }

        [DataMember] public int Delay { get; set; } = Polarizer.DefaultDelay;

        public override void Copy(PolarizerParameters other)
        {
            base.Copy(other);
            PortName = other.PortName;
            Delay = other.Delay;
        }

        public override bool NeedsReinstantiation(PolarizerParameters other)
        {
            var needs = base.NeedsReinstantiation(other);
            if (needs) return true;

            if (Type == typeof(Polarizer) || Type.IsSubclassOf(typeof(Polarizer))) needs |= other.PortName != PortName;
            return needs;
        }

        public override bool NeedsUpdate(PolarizerParameters other)
        {
            var needs = other.Delay != Delay;
            return needs;
        }
    }
}