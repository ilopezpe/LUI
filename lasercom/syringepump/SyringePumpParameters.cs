using lasercom.objects;
using System;
using System.Runtime.Serialization;

namespace lasercom.syringepump
{
    public class SyringePumpParameters : LuiObjectParameters<SyringePumpParameters>
    {
        public SyringePumpParameters(Type Type, string PortName) : base(Type)
        {
            this.PortName = PortName;
        }

        public SyringePumpParameters(Type Type)
            : base(Type)
        {
        }

        public SyringePumpParameters()
        {
        }

        public SyringePumpParameters(SyringePumpParameters other)
            : base(other)
        {
        }

        [DataMember] public string PortName { get; set; }

        public override void Copy(SyringePumpParameters other)
        {
            base.Copy(other);
            PortName = other.PortName;
        }

        public override bool NeedsReinstantiation(SyringePumpParameters other)
        {
            var needs = base.NeedsReinstantiation(other);
            if (needs) return true;

            if (Type == typeof(HarvardSyringePump) || Type.IsSubclassOf(typeof(HarvardSyringePump)))
                needs |= other.PortName != PortName;
            return needs;
        }

        public override bool NeedsUpdate(SyringePumpParameters other)
        {
            return false;
        }
    }
}