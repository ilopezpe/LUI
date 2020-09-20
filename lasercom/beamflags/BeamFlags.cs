using lasercom.objects;
using System;
using System.IO.Ports;
using System.Threading;

namespace lasercom.beamflags
{
    /// <summary>
    /// Class representing BeamFlags operated by numato usbgpio16 controller.
    /// </summary>
    public class BeamFlags : AbstractBeamFlags
    {
        #region Constants 
        //use masks to send commands simultaneously
        public const string gpioOutputs = "00ff";
        public const string gpioMask = "c000";

        //probe light shutter
        //reverse logic, i.e. high = close shutters. low = open shutters.
        public const string OpenFlashCommand = "gpio clear E\r";
        public const string CloseFlashCommand = "gpio set E\r";

        //laser shutter
        public const string OpenLaserCommand = "gpio clear F\r";
        public const string CloseLaserCommand = "gpio set F\r";

        // the following allows switching both io14 and io15 simultaneously.
        public const string OpenLaserAndFlashCommand = "gpio writeall 0000\r";
        public const string CloseLaserAndFlashCommand = "gpio writeall ffff\r";
        #endregion

        // Approximate time in ms for solenoid to switch.
        public const int DefaultDelay = 1000;

        SerialPort _port;

        public BeamFlags(LuiObjectParameters p) : this(p as BeamFlagsParameters)
        {
        }

        public BeamFlags(BeamFlagsParameters p)
        {
            if (p == null || p.PortName == null)
                throw new ArgumentException("PortName must be defined.");
            Init(p.PortName);
        }

        public BeamFlags(string portName)
        {
            Init(portName);
        }

        public int Delay { get; set; } // Time in miliseconds to sleep between commands.

        public string PortName => _port.PortName;

        void Init(string portName)
        {
            Delay = DefaultDelay;
            _port = new SerialPort(portName)
            {
                BaudRate = 9600
            };
            if (!_port.IsOpen)
                _port.Open();

            _port.DiscardInBuffer();
            _port.Write("gpio iodir" + gpioOutputs + "\r");
            Thread.Sleep(10);
            _port.Write("gpio iomask " + gpioMask + "\r");
            Thread.Sleep(10);
            _port.DiscardOutBuffer();

            CloseLaserAndFlash();
        }

        public override void OpenLaser()
        {
            OpenLaser(true);
        }

        /// <summary>
        ///     Opens the laser flag, optionally sleeping to ensure the flag has opened completely.
        ///     LaserState is updated only after sleeping in case of monitoring by another thread.
        /// </summary>
        /// <param name="wait"></param>
        void OpenLaser(bool wait)
        {
            _port.DiscardInBuffer();
            _port.Write(OpenLaserCommand);
            if (wait) Thread.Sleep(Delay);
            LaserState = BeamFlagState.Open;
            _port.DiscardOutBuffer();
        }

        public override void OpenFlash()
        {
            OpenFlash(true);
        }

        void OpenFlash(bool wait)
        {
            _port.DiscardInBuffer();
            _port.Write(OpenFlashCommand);
            if (wait) Thread.Sleep(Delay);
            FlashState = BeamFlagState.Open;
            _port.DiscardOutBuffer();
        }

        public override void OpenLaserAndFlash()
        {
            OpenLaserAndFlash(true);
        }

        void OpenLaserAndFlash(bool wait)
        {
            _port.DiscardInBuffer();
            _port.Write(OpenLaserAndFlashCommand);
            if (wait) Thread.Sleep(Delay);
            LaserState = BeamFlagState.Open;
            FlashState = BeamFlagState.Open;
            _port.DiscardOutBuffer();
        }

        public override void CloseLaser()
        {
            CloseLaser(true);
        }

        void CloseLaser(bool wait)
        {
            _port.DiscardInBuffer();
            _port.Write(CloseLaserCommand);
            if (wait) Thread.Sleep(Delay);
            LaserState = BeamFlagState.Closed;
            _port.DiscardOutBuffer();
        }

        public override void CloseFlash()
        {
            CloseFlash(true);
        }

        void CloseFlash(bool wait)
        {
            _port.DiscardInBuffer();
            _port.Write(CloseFlashCommand);
            if (wait) Thread.Sleep(Delay);
            FlashState = BeamFlagState.Closed;
            _port.DiscardOutBuffer();
        }

        public override void CloseLaserAndFlash()
        {
            CloseLaserAndFlash(true);
        }

        void CloseLaserAndFlash(bool wait)
        {
            _port.DiscardInBuffer();
            _port.Write(CloseLaserAndFlashCommand);
            if (wait) Thread.Sleep(Delay);
            LaserState = BeamFlagState.Closed;
            FlashState = BeamFlagState.Closed;
            _port.DiscardOutBuffer();
        }

        void EnsurePortDisposed()
        {
            if (_port != null)
            {
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseLaserAndFlash();
                EnsurePortDisposed();
            }
        }

        public override void Update(BeamFlagsParameters p)
        {
            Delay = p.Delay;
        }
    }
}