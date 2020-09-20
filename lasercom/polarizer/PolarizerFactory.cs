using System;

namespace lasercom.polarizer
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