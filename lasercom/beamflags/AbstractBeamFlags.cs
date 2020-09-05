using lasercom.objects;
using System;

namespace lasercom.beamflags
{
    /// <summary>
    ///     Base class for all beam flag classes.
    /// </summary>
    public abstract class AbstractBeamFlags : LuiObject<BeamFlagsParameters>, IBeamFlags
    {
        public BeamFlagState FlashState { get; set; }
        public BeamFlagState LaserState { get; set; }

        public virtual BeamFlagState ToggleLaser()
        {
            switch (LaserState)
            {
                case BeamFlagState.Closed:
                    OpenLaser();
                    break;

                case BeamFlagState.Open:
                    CloseLaser();
                    break;
            }

            return LaserState;
        }

        public virtual BeamFlagState ToggleFlash()
        {
            switch (FlashState)
            {
                case BeamFlagState.Closed:
                    OpenFlash();
                    break;

                case BeamFlagState.Open:
                    CloseFlash();
                    break;
            }

            return FlashState;
        }

        public virtual void OpenLaser()
        {
            LaserState = BeamFlagState.Open;
        }

        public virtual void CloseLaser()
        {
            LaserState = BeamFlagState.Closed;
        }

        public virtual void OpenFlash()
        {
            FlashState = BeamFlagState.Open;
        }

        public virtual void CloseFlash()
        {
            FlashState = BeamFlagState.Closed;
        }

        public virtual void ToggleLaserAndFlash()
        {
            ToggleFlash();
            ToggleLaser();
        }

        public virtual void OpenLaserAndFlash()
        {
            LaserState = BeamFlagState.Open;
            FlashState = BeamFlagState.Open;
        }

        public virtual void CloseLaserAndFlash()
        {
            LaserState = BeamFlagState.Closed;
            FlashState = BeamFlagState.Closed;
        }

        public override void Update(BeamFlagsParameters p)
        {
            throw new NotImplementedException();
        }
    }
}