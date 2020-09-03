﻿using System;
using System.Collections.Generic;

using ATMCD32CS;

using log4net;
using lasercom.camera;
using lasercom.control;
using lasercom.ddg;

namespace lasercom
{

    public class Commander
    {
        private ICamera _Camera;
        public ICamera Camera
        {
            get
            {
                return _Camera;
            }
            set
            {
                _Camera = value;
            }
        }
        public IBeamFlags BeamFlag { get; set; }
        public IDigitalDelayGenerator DDG { get; set; }
        public IPump Pump { get; set; }
        public List<Double> Delays { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Commander(ICamera camera = null, IBeamFlags beamFlags = null, IDigitalDelayGenerator ddg = null, IPump pump = null)
        {
            // Set dummies instead of null values to save a *ton* of null checks elsewhere.
            Camera = camera ?? new DummyCamera();
            BeamFlag = beamFlags ?? new DummyBeamFlags();
            DDG = ddg ?? new DummyDigitalDelayGenerator();
            Pump = pump ?? new DummyPump();
        }

        public void SetDelays(string file)
        {
            // read file
            Delays = new List<double>();
        }

        public int[] Collect(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Camera.CountsFvb();
            }
            return null;
        }

        public int[] Dark()
        {
            BeamFlag.CloseLaserAndFlash();
            return Camera.Acquire();
        }

        public uint Dark(int[] DataBuffer)
        {
            BeamFlag.CloseLaserAndFlash();
            return Camera.Acquire(DataBuffer);
        }

        public int[] Flash()
        {
            BeamFlag.CloseLaserAndFlash();
            BeamFlag.OpenFlash();
            int[] data = Camera.Acquire();
            BeamFlag.CloseLaserAndFlash();
            return data;
        }

        public uint Flash(int[] DataBuffer)
        {
            BeamFlag.CloseLaserAndFlash();
            BeamFlag.OpenFlash();
            uint ret = Camera.Acquire(DataBuffer);
            BeamFlag.CloseLaserAndFlash();
            return ret;
        }

        public int[] Trans()
        {
            BeamFlag.OpenLaserAndFlash();
            int[] data = Camera.Acquire();
            BeamFlag.CloseLaserAndFlash();
            return data;
        }

        public uint Trans(int[] DataBuffer)
        {
            BeamFlag.OpenLaserAndFlash();
            uint ret = Camera.Acquire(DataBuffer);
            BeamFlag.CloseLaserAndFlash();
            return ret;
        }

    }
}
