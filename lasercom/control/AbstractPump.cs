using lasercom.objects;
using System;

namespace lasercom.control
{
    /// <summary>
    ///     Base class for all pumps.
    /// </summary>
    public abstract class AbstractPump : LuiObject<PumpParameters>, IPump
    {
        public PumpState CurrentState { get; protected set; }

        public virtual PumpState Toggle()
        {
            switch (CurrentState)
            {
                case PumpState.Open:
                    SetClosed();
                    break;

                case PumpState.Closed:
                    SetOpen();
                    break;
            }

            return CurrentState;
        }

        public virtual void SetOpen()
        {
            CurrentState = PumpState.Open;
            //TODO Which is which?
        }

        public virtual void SetClosed()
        {
            CurrentState = PumpState.Closed;
        }

        public virtual PumpState GetState()
        {
            return CurrentState;
        }

        public override void Update(PumpParameters p)
        {
            throw new NotImplementedException();
        }
    }
}