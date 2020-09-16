using lasercom.gpib;
using lasercom.objects;
using System;
using System.Runtime.Serialization;

namespace lasercom.ddg
{
    /// <summary>
    ///     Stores parameters for instantiation of a DDG and provides
    ///     fpr their serialization to XML.
    /// </summary>
    [DataContract]
    public class DelayGeneratorParameters : LuiObjectParameters<DelayGeneratorParameters>
    {
        public DelayGeneratorParameters(Type Type)
            : base(Type)
        {
        }

        public DelayGeneratorParameters()
        {
        }

        public DelayGeneratorParameters(DelayGeneratorParameters other)
            : base(other)
        {
        }

        [DataMember] public byte GpibAddress { get; set; }

        [DataMember] public GpibProviderParameters GpibProvider { get; set; }

        public override LuiObjectParameters[] Dependencies
        {
            get
            {
                if (GpibProvider != null)
                    return new LuiObjectParameters[] { GpibProvider };
                return new LuiObjectParameters[0];
            }
        }

        public override void Copy(DelayGeneratorParameters other)
        {
            base.Copy(other);
            GpibAddress = other.GpibAddress;
            GpibProvider = other.GpibProvider;
        }

        public override bool NeedsReinstantiation(DelayGeneratorParameters other)
        {
            var needs = base.NeedsReinstantiation(other);
            if (needs) return true;

            if (Type == typeof(StanfordDigitalDelayGenerator) ||
                Type.IsSubclassOf(typeof(StanfordDigitalDelayGenerator)))
                needs |= GpibProvider != other.GpibProvider ||
                         GpibProvider != null && !GpibProvider.Equals(other.GpibProvider);
            return needs;
        }

        public override bool NeedsUpdate(DelayGeneratorParameters other)
        {
            var needs = false;

            if (Type == typeof(StanfordDigitalDelayGenerator) ||
                Type.IsSubclassOf(typeof(StanfordDigitalDelayGenerator))) needs |= other.GpibAddress != GpibAddress;

            return needs;
        }
    }
}