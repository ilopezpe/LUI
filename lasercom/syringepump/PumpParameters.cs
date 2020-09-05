﻿using lasercom.objects;
using System;
using System.Runtime.Serialization;

namespace lasercom.syringepump
{
    public class PumpParameters : LuiObjectParameters<PumpParameters>
    {
        public PumpParameters(Type Type, string PortName) : base(Type)
        {
            this.PortName = PortName;
        }

        public PumpParameters(Type Type)
            : base(Type)
        {
        }

        public PumpParameters()
        {
        }

        public PumpParameters(PumpParameters other)
            : base(other)
        {
        }

        [DataMember] public string PortName { get; set; }

        public override void Copy(PumpParameters other)
        {
            base.Copy(other);
            PortName = other.PortName;
        }

        public override bool NeedsReinstantiation(PumpParameters other)
        {
            var needs = base.NeedsReinstantiation(other);
            if (needs) return true;

            if (Type == typeof(HarvardPump) || Type.IsSubclassOf(typeof(HarvardPump)))
                needs |= other.PortName != PortName;
            return needs;
        }

        public override bool NeedsUpdate(PumpParameters other)
        {
            return false;
        }
    }
}