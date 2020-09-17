using LuiHardware.objects;
using System;
using System.IO.Ports;

namespace LuiHardware.syringepump
{
    /// <summary>
    ///     Represents a Harvard Apparatus syringe pump using the custom
    ///     flip-flop box and foot-pedal hack.
    /// </summary>
    public class HarvardSyringePump : AbstractSyringePump
    {
        SerialPort _port;

        public HarvardSyringePump(LuiObjectParameters p) :
            this(p as SyringePumpParameters)
        {
        }

        public HarvardSyringePump(SyringePumpParameters p)
        {
            if (p == null || p.PortName == null)
                throw new ArgumentException("PortName must be defined.");
            Init(p.PortName);
        }

        public HarvardSyringePump(string portName)
        {
            Init(portName);
        }

        void Init(string portName)
        {
            // DtrEnable causes DTR pin to go high on port open
            _port = new SerialPort(portName) { DtrEnable = true };
            SetClosed();
        }

        public string GetPortName()
        {
            return _port.PortName;
        }

        public override SyringePumpState Toggle()
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

        public override void SetOpen()
        {
            CurrentState = SyringePumpState.Open;
            _port.Open(); //TODO Which is which?
        }

        public override void SetClosed()
        {
            CurrentState = SyringePumpState.Closed;
            _port.Close();
        }

        public override SyringePumpState GetState()
        {
            return CurrentState;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
            }
        }
    }
}