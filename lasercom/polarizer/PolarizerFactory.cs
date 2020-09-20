using System;

namespace LuiHardware.polarizer
{
    /// <summary>
    /// Instantiate concrete polarizer objects from parameters.
    /// </summary>
    class Polarizerfactory
    {
        public static AbstractPolarizer CreatePolarizer(PolarizerParameters p)
        {
            return (AbstractPolarizer)Activator.CreateInstance(p.Type, p);
        }
    }
}