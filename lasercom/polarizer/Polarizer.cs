﻿using lasercom.objects;
using System;
using System.IO.Ports;
using System.Threading;

namespace lasercom.polarizer
{
    /// <summary>
    /// Class representing custom Polarizer operated by serial commands
    /// </summary>
    public class Polarizer : AbstractPolarizer
    {
        #region Constants 
        // Serial commands to move polarizer
        public const float PolarizerAlign = 0.00f;
        public const float PolarizerCross = 90.00f;
        public const string PolarizerMoveCommand = "MOVETO";
        public const string PolarizerSetPosCommand = "SETPOS";
        public const string PolarizerStopCommand = "STOP\r\n";
        #endregion

        private int stepsPerUnit = 50;

        // Approximate time in ms for stepper to complete a move.
        public const int DefaultDelay = 15000; // 15 second

        SerialPort _port;

        public Polarizer(LuiObjectParameters p) : this(p as PolarizerParameters)
        {
        }

        public Polarizer(PolarizerParameters p)
        {
            if (p == null || p.PortName == null)
            {
                throw new ArgumentException("PortName must be defined.");
            }
            Init(p.PortName);
        }

        public Polarizer(string portName)
        {
            Init(portName);
        }

        public override float PolarizerBeta { get; set; } = 10.00F;
        public override int MinBeta { get; set; } = 0;
        public override int MaxBeta { get; set; } = 20;

        public int Delay { get; set; } // Time in miliseconds to sleep between commands.

        public string PortName => _port.PortName;

        void Init(string portName)
        {
            Delay = DefaultDelay;
            _port = new SerialPort(portName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true,
                DtrEnable = false //do not enable! will reset arduino
            };
            if (!_port.IsOpen)
            {
                _port.Open();
            }
            Thread.Sleep(10);

        }

        private float ToStep(float angle)
        {
            float NSteps = angle * stepsPerUnit;
            Math.Round(NSteps, 2);
            return NSteps;
        }

        public override void SetAngle(float angle)
        {
            float NSteps = ToStep(angle);
            _port.DiscardInBuffer();
            _port.Write(PolarizerMoveCommand + " " + NSteps.ToString() + "\r\n");
            Thread.Sleep(Delay);
            _port.DiscardInBuffer();
        }

        public override PolarizerPosition Toggle()
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

        public override PolarizerPosition ToggleBeta()
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

        public override void PolarizerToAligned()
        {
            PolarizerToAligned(true);
        }

        /// <summary>
        /// Move the polarizer to the aligned position,  sleeping to ensure the move has completed.
        /// The PolarizerState is updated only after sleeping in case of monitoring by another thread.
        /// </summary>
        /// <param name="wait"></param>
        void PolarizerToAligned(bool wait)
        {
            float NSteps = ToStep(PolarizerAlign);

            _port.DiscardInBuffer();
            _port.Write(PolarizerMoveCommand + " " + NSteps.ToString() + "\r\n");
            if (wait) Thread.Sleep(Delay*3); // much longer
            CurrentPosition = PolarizerPosition.Aligned;
            _port.DiscardOutBuffer();
        }

        public override void PolarizerToCrossed()
        {
            PolarizerToCrossed(true);
        }

        /// <summary>
        /// Move the polarizer to the crossed position,  sleeping to ensure the move has completed.
        /// The PolarizerState is updated only after sleeping in case of monitoring by another thread.
        /// </summary>
        /// <param name="wait"></param>
        void PolarizerToCrossed(bool wait)
        {
            float NSteps = ToStep(PolarizerCross);

            _port.DiscardInBuffer();
            _port.Write(PolarizerMoveCommand + " " + NSteps.ToString() + "\r\n");
            if (wait) Thread.Sleep(Delay*3);
            CurrentPosition = PolarizerPosition.Crossed;
            _port.DiscardOutBuffer();
        }

        public override void PolarizerToPlusBeta()
        {
            PolarizerToPlusBeta(true);
        }

        /// <summary>
        /// Move the polarizer to the plus beta position,  sleeping to ensure the move has completed.
        /// The PolarizerState is updated only after sleeping in case of monitoring by another thread.
        /// </summary>
        /// <param name="wait"></param>
        void PolarizerToPlusBeta(bool wait)
        {
            float NSteps = ToStep(PolarizerCross + PolarizerBeta);

            _port.DiscardInBuffer();
            _port.Write(PolarizerMoveCommand + " " + NSteps.ToString() + "\r\n");
            if (wait) Thread.Sleep(Delay);
            CurrentPosition = PolarizerPosition.Plus;
            _port.DiscardOutBuffer();
        }

        public override void PolarizerToMinusBeta()
        {
            PolarizerToMinusBeta(true);
        }

        /// <summary>
        /// Move the polarizer to the plus beta position,  sleeping to ensure the move has completed.
        /// The PolarizerState is updated only after sleeping in case of monitoring by another thread.
        /// </summary>
        /// <param name="wait"></param>
        void PolarizerToMinusBeta(bool wait)
        {
            float NSteps = ToStep(PolarizerCross - PolarizerBeta);

            _port.DiscardInBuffer();
            _port.Write(PolarizerMoveCommand + " " + NSteps.ToString() + "\r\n");
            if (wait) Thread.Sleep(Delay);
            CurrentPosition = PolarizerPosition.Minus;
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
                EnsurePortDisposed();
            }
        }

        public override void Update(PolarizerParameters p)
        {
            Delay = p.Delay;
        }
    }
}