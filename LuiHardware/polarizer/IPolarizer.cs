namespace LuiHardware.polarizer
{
    public enum PolarizerPosition
    {
        Aligned,
        Crossed,
        Plus,
        Minus,
        Zero,
    }

    public enum PolarizerConfig
    {
        Trld,
        Ord,
    }

    /// <summary>
    /// Defines the public operations supported by the computer-controlled polarizer.
    /// </summary>
    public interface IPolarizer
    {

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
        float PolarizerBeta { get; set; }

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

        /// <summary>
        /// Move the polarizer to the -beta position
        /// </summary>
        void PolarizerToZeroBeta();


        /// <summary>
        /// This holds the current config of the polarizer
        /// </summary>
        PolarizerConfig CurrentConfig { get; }

        /// <summary>
        /// Move the polarizer to TRLD configuration
        /// </summary>
        void PolarizerConfigTrld();

        /// <summary>
        /// Move the polarizer to ORD configuration
        /// </summary>
        void PolarizerConfigOrd();

    }
}