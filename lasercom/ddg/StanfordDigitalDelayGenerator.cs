﻿using lasercom.gpib;
using lasercom.objects;
using log4net;
using System.Linq;

//  <summary>
//      Represents a Stanford DDG.
//  </summary>

namespace lasercom.ddg
{
    /// <summary>
    /// Base class for Stanford Instruments DDGs.
    /// </summary>
    public abstract class StanfordDigitalDelayGenerator : AbstractDigitalDelayGenerator
    {
#pragma warning disable CS0108 // 'StanfordDigitalDelayGenerator.Log' hides inherited member 'LuiObject.Log'. Use the new keyword if hiding was intended.
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#pragma warning restore CS0108 // 'StanfordDigitalDelayGenerator.Log' hides inherited member 'LuiObject.Log'. Use the new keyword if hiding was intended.

        public IGpibProvider GPIBProvider { get; set; }
        public byte GPIBAddress { get; set; }

        public StanfordDigitalDelayGenerator(LuiObjectParameters p, params ILuiObject[] dependencies) :
            this(p as DelayGeneratorParameters, dependencies)
        { } //TODO just take IGpibProvider instead of params array.

        public StanfordDigitalDelayGenerator(DelayGeneratorParameters p, params ILuiObject[] dependencies)
        {
            Init(p.GpibAddress, dependencies);
        }

        private void Init(byte _GpibAddress, params ILuiObject[] dependencies)
        {
            GPIBProvider = (IGpibProvider)dependencies.First(d => d is IGpibProvider);
            GPIBAddress = _GpibAddress;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Nothing to dispose of.
            }
        }

        public override void Update(DelayGeneratorParameters p)
        {
            GPIBAddress = p.GpibAddress;
        }

    }
}
