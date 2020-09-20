namespace LuiHardware.syringepump
{
    public enum SyringePumpState
    {
        Open,
        Closed
    }

    /// <summary>
    ///     Defines the public operations supported by a pump.
    /// </summary>
    public interface ISyringePump
    {
        SyringePumpState CurrentState { get; }

        SyringePumpState Toggle();

        void SetOpen();

        void SetClosed();

        SyringePumpState GetState();
    }
}