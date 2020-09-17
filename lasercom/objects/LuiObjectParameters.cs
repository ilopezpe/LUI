using Extensions;
using System;
using System.Runtime.Serialization;

namespace LuiHardware.objects
{
    /// <summary>
    ///     Nongeneric base for all instrument parameters.
    ///     Permits access of fields shared by all instrument parameters without
    ///     knowing the exact runtime parameters class.
    /// </summary>
    [DataContract]
    [KnownType("GetKnownTypes")]
    public abstract class LuiObjectParameters
    {
        [DataMember] public string Name { get; set; }

        public Type Type { get; set; }

        [DataMember]
        public string TypeName
        {
            get => Type.FullName;
            set => Type = Type.GetType(value);
        }

        public virtual LuiObjectParameters[] Dependencies => new LuiObjectParameters[0];

        static Type[] GetKnownTypes()
        {
            return typeof(LuiObjectParameters).GetSubclasses(true).ToArray();
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            return Equals(other as LuiObjectParameters);
        }

        public virtual bool Equals(LuiObjectParameters other)
        {
            if (other == null || GetType() != other.GetType())
                return false;
            var iseq = Type == other.Type &&
                       Name == other.Name;
            return iseq;
        }

        public override int GetHashCode()
        {
            return Util.Hash(Type, Name);
        }
    }

    /// <summary>
    ///     Self-constrained generic base class for all instrument parameters.
    /// </summary>
    /// <typeparam name="P"></typeparam>
    [DataContract]
    public abstract class LuiObjectParameters<P> : LuiObjectParameters,
        IEquatable<P> where P : LuiObjectParameters<P>
    {
        public LuiObjectParameters()
        {
        }

        public LuiObjectParameters(P other)
        {
            Copy(other);
        }

        public LuiObjectParameters(Type t)
        {
            Type = t;
        }

        public virtual bool Equals(P other)
        {
            var iseq = base.Equals(other);
            return iseq;
        }

        public virtual void Copy(P other)
        {
            Type = other.Type;
            Name = other.Name;
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            return Equals(other as P);
        }

        public override bool Equals(LuiObjectParameters other)
        {
            return Equals(other as P);
        }

        public override int GetHashCode()
        {
            return Util.Hash(Type, Name);
        }

        public virtual bool NeedsReinstantiation(P other)
        {
            return Type != other.Type;
        }

        public abstract bool NeedsUpdate(P other);
    }
}