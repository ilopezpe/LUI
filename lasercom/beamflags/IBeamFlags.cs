namespace lasercom.beamflags
{
    public enum BeamFlagState { Open, Closed }

    /// <summary>
    /// Defines the public operations supported by beam flags.
    /// </summary>
    public interface IBeamFlags
    {
        /// <summary>
        /// This holds the current state of the probe beam shutter
        /// </summary>
        BeamFlagState FlashState { get; }

        /// <summary>
        /// This holds the current state of the laser beam shutter 
        /// </summary>
        BeamFlagState LaserState { get; }

        /// <summary>
        /// This commands the laser beam shutter to switch position
        /// </summary>
        /// <returns>ProbeState</returns>
        BeamFlagState ToggleLaser();

        /// <summary>
        /// This commands the probe beam shutter to switch position
        /// </summary>
        /// <returns>ProbeState</returns>
        BeamFlagState ToggleFlash();

        /// <summary>
        /// This commands laser and probe beam shutters to switch position
        /// </summary>
        void ToggleLaserAndFlash();

        /// <summary>
        /// Opens probe beam shutter
        /// </summary>
        void OpenLaser();

        /// <summary>
        /// Closes laser beam shutter
        /// </summary>
        void CloseLaser();

        /// <summary>
        /// Opens probe beam shutter
        /// </summary>
        void OpenFlash();

        /// <summary>
        /// Closes probe beam shutter
        /// </summary>
        void CloseFlash();

        /// <summary>
        /// Opens laser and probe beam shutters simultaneously
        /// </summary>
        void OpenLaserAndFlash();

        /// <summary>
        /// Closes laser and probe beam shutters simultaneously
        /// </summary>
        void CloseLaserAndFlash();
    }
}