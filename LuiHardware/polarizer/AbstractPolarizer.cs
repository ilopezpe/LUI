using LuiHardware.objects;
using System;

namespace LuiHardware.polarizer
{
    /// <summary>
    /// Base class for all beam flag classes.
    /// </summary>
    public abstract class AbstractPolarizer : LuiObject<PolarizerParameters>, IPolarizer
    {

        public PolarizerPosition CurrentPosition { get; protected set; }

        public PolarizerConfig CurrentConfig { get; protected set; }

        public virtual float PolarizerBeta { get; set; }

        public virtual int MinBeta { get; set; }

        public virtual int MaxBeta { get; set; }

        public virtual PolarizerPosition Toggle()
        {
            switch (CurrentPosition)
            {
                case PolarizerPosition.Aligned:
                    PolarizerToCrossed();
                    break;

                case PolarizerPosition.Crossed:
                    PolarizerToAligned();
                    break;
            }
            return CurrentPosition;
        }

        public virtual PolarizerPosition ToggleBeta()
        {
            switch (CurrentPosition)
            {
                case PolarizerPosition.Plus:
                    PolarizerToMinusBeta();
                    break;

                case PolarizerPosition.Minus:
                    PolarizerToPlusBeta();
                    break;
            }
            return CurrentPosition;
        }

        public virtual void PolarizerToAligned()
        {
            CurrentPosition = PolarizerPosition.Aligned;
        }
        public virtual void PolarizerToCrossed()
        {
            CurrentPosition = PolarizerPosition.Crossed;
        }
        public virtual void PolarizerToPlusBeta()
        {
            CurrentPosition = PolarizerPosition.Plus;
        }

        public virtual void PolarizerToMinusBeta()
        {
            CurrentPosition = PolarizerPosition.Minus;
        }

        public virtual void PolarizerToZeroBeta()
        {
            CurrentPosition = PolarizerPosition.Zero;
        }

        public override void Update(PolarizerParameters p)
        {
            throw new NotImplementedException();
        }
        public virtual PolarizerPosition GetPosition()
        {
            return CurrentPosition;
        }


        public virtual void PolarizerConfigTrld()
        {
            CurrentPosition = PolarizerPosition.Crossed;
            CurrentConfig = PolarizerConfig.Trld;
        }

        public virtual void PolarizerConfigOrd()
        {
            CurrentPosition = PolarizerPosition.Crossed;
            CurrentConfig = PolarizerConfig.Ord;
        }
    }
}