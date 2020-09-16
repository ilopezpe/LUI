namespace lasercom.polarizer
{
    public enum PolarizerPosition
    {
        Aligned,
        Crossed,
        Plus,
        Minus,
    }

    /// <summary>
    /// Defines the public operations supported by the computer-controlled polarizer.
    /// </summary>
    public interface IPolarizer
    {
        /// <summary>
        /// Absolute move to position, accepts degrees
        /// </summary>
        /// <param name="angle"></param>
        void SetAngle(float angle);

        /// <summary>
        /// This holds the current position of the polarizer
        /// </summary>
        PolarizerPosition CurrentPosition { get; }

        /// <summary>
        /// This toggles the current position of the polarizer between crossed and aligned
        /// </summary>
        PolarizerPosition Toggle();

        /// <summary>
        /// This toggles the current position of the polarizer between plus/minus beta
        /// </summary>
        PolarizerPosition ToggleBeta();

        /// <summary>
        /// Return the current position of the polarizer
        /// </summary>
        PolarizerPosition GetPosition();

        /// <summary>
        /// Move the polarizer to the aligned position
        /// </summary>
        void PolarizerToAligned();

        /// <summary>
        /// Move the polarizer to the crossed position
        /// </summary>
        void PolarizerToCrossed();

        /// <summary>
        /// Current beta setting of polarizers.
        /// </summary>
        int PolarizerBeta { get; set; }

        /// <summary>
        /// Min beta setting of polarizers.
        /// </summary>
        int MinBeta { get; set; }

        /// <summary>
        /// Max beta setting of polarizers.
        /// </summary>
        int MaxBeta { get; set; }

        /// <summary>
        /// Move the polarizer to the +beta position
        /// </summary>
        void PolarizerToPlusBeta();

        /// <summary>
        /// Move the polarizer to the -beta position
        /// </summary>
        void PolarizerToMinusBeta();
    }
}