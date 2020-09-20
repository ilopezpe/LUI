using lasercom.objects;
using System;

namespace lasercom.syringepump
{
    /// <summary>
    /// Base class for all syringe pumps.
    /// </summary>
    public abstract class AbstractSyringePump : LuiObject<SyringePumpParameters>, ISyringePump
    {
        public SyringePumpState CurrentState { get; protected set; }

        public virtual SyringePumpState Toggle()
        {
            switch (CurrentState)
            {
                case SyringePumpState.Open:
                    SetClosed();
                    break;

                case SyringePumpState.Closed:
                    SetOpen();
                    break;
            }

            return CurrentState;
        }

        public virtual void SetOpen()
        {
            CurrentState = SyringePumpState.Open;
            //TODO Which is which?
        }

        public virtual void SetClosed()
        {
            CurrentState = SyringePumpState.Closed;
        }

        public virtual SyringePumpState GetState()
        {
            return CurrentState;
        }

        public override void Update(SyringePumpParameters p)
        {
            throw new NotImplementedException();
        }
    }
}