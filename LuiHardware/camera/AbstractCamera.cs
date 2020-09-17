﻿using LuiHardware.io;
using LuiHardware.objects;
using System.IO;
using System.Linq;

namespace LuiHardware.camera
{
    public abstract class AbstractCamera : LuiObject<CameraParameters>, ICamera
    {
        public virtual int SaturationLevel { get; set; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract int AcqSize { get; }

        public abstract int AcqWidth { get; }

        public abstract int AcqHeight { get; }

        public abstract int AcquisitionMode { get; set; }

        public abstract int TriggerMode { get; set; }

        public abstract int DDGTriggerMode { get; set; }

        public abstract int ReadMode { get; set; }

        public abstract bool HasIntensifier { get; }

        public abstract int IntensifierGain { get; set; }

        public abstract int MinIntensifierGain { get; protected set; }

        public abstract int MaxIntensifierGain { get; protected set; }

        public double[] Calibration { get; set; }

        public bool CalibrationAscending => Calibration[Calibration.Length - 1] > Calibration[0];

        public abstract ImageArea Image { get; set; }

        public abstract int[] CountsFvb();

        public abstract int[] FullResolutionImage();

        public abstract int[] Acquire();

        public abstract uint Acquire(int[] DataBuffer);

        public abstract string DecodeStatus(uint status);

        public override void Update(CameraParameters p)
        {
            LoadCalibration(p.CalFile);
            ReadMode = p.ReadMode;
            Image = p.Image;
            SaturationLevel = p.SaturationLevel;
            if (HasIntensifier) IntensifierGain = p.InitialGain;
        }

        protected void LoadCalibration(string CalFile)
        {
            if (CalFile == null || CalFile == "")
                Calibration = Enumerable.Range(0, Width).Select(x => (double)x).ToArray();
            else
                try
                {
                    Calibration = FileIO.ReadVector<double>(CalFile);
                }
                catch (IOException ex)
                {
                    Log.Error(ex);
                    Calibration = Enumerable.Range(0, Width).Select(x => (double)x).ToArray();
                    throw;
                }
        }
    }
}