using System;

namespace LuiHardware.objects
{
    /// <summary>
    ///     Aggregates interfaces and defines public operations of LuiObject subtypes.
    /// </summary>
    public interface ILuiObject : IDisposable
    {
    }

    public interface ILuiObject<P> : ILuiObject where P : LuiObjectParameters<P>
    {
        void Update(P p);
    }
}