﻿
#if x64
using ATMCD64CS;
#else
using System.Diagnostics;
using System.Linq;
using ATMCD32CS;


#endif

namespace lasercom.camera
{
    public class DummyAndorCamera : AndorCamera
    {
        new AndorSDK AndorSdk = null;

        private int _ReadMode;
        override public int ReadMode
        {
            get { return _ReadMode; }
            set
            {
                _ReadMode = value;
            }
        }

        private int _AcquisitionMode;
        override public int AcquisitionMode
        {
            get { return _AcquisitionMode; }
            set
            {
                _AcquisitionMode = value;
            }
        }

        private int _TriggerMode;
        override public int TriggerMode
        {
            get { return _TriggerMode; }
            set
            {
                _TriggerMode = value;
            }
        }

        private int _TriggerInvert;
        override public int TriggerInvert
        {
            get { return _TriggerInvert; }
            set
            {
                _TriggerInvert = value;
            }
        }

        private float _TriggerLevel;
        override public float TriggerLevel
        {
            get { return _TriggerLevel; }
            set
            {
                _TriggerLevel = value;
            }
        }

        private int _DDGTriggerMode;
        override public int DDGTriggerMode
        {
            get { return _DDGTriggerMode; }
            set
            {
                _DDGTriggerMode = value;
            }
        }

        private int _GateMode;
        override public int GateMode
        {
            get { return _GateMode; }
            set
            {
                _GateMode = value;
            }
        }

        override public bool HasIntensifier
        {
            get
            {
                return true;
            }
        }

        private int _MCPGating;
        override public int MCPGating
        {
            get { return _MCPGating; }
            set
            {
                _MCPGating = value;
            }
        }

        private int _MCPGain;
        override public int IntensifierGain
        {
            get { return _MCPGain; }
            set
            {
                _MCPGain = value;
            }
        }

        private int _NumberAccumulations;
        override public int NumberAccumulations
        {
            get { return _NumberAccumulations; }
            set
            {
                _NumberAccumulations = value;
            }
        }

        private ImageArea _Image;
        override public ImageArea Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
            }
        }

        private uint _Height;
        public override uint Height
        {
            get
            {
                return _Height;
            }
        }

        private uint _Width;
        public override uint Width
        {
            get
            {
                return _Width;
            }
        }

        public DummyAndorCamera(string CalFile = null, int InitialGain = AndorCamera.DefaultMCPGain) : base(CalFile)
        {
            _Width = 1024;
            _Height = 256;
            Calibration = Enumerable.Range(0, (int)Width).Select(x => (double)x).ToArray();
            LoadCalibration(CalFile);
            MinIntensifierGain = 0;
            MaxIntensifierGain = 4095;
            IntensifierGain = InitialGain;
        }

        public override int[] FullResolutionImage()
        {
            return null;
        }

        public override int[] CountsFvb()
        {
            return null;
        }

        public override int[] Acquire()
        {
            int[] data = new int[Width];
            Acquire(data);
            return data;
        }

        public override uint Acquire(int[] DataBuffer)
        {
            System.Threading.Thread.Sleep(250);
            var frame = 1;
            string caller = (new StackFrame(frame, true)).GetFileName();
            if (caller.Contains("DummyAndorCamera"))
            {
                frame++;
            }
            caller = (new StackFrame(frame,true)).GetFileName();
            int line = (new StackFrame(frame,true)).GetFileLineNumber();
            int[] data = null;
            if (caller.Contains("TroaControl"))
            {
                if (line < 320) // Dark.
                {
                    data = Data.Uniform((int)Width, 1000);
                }
                else if (line < 358 || line > 390) // Ground.
                {
                    data = Data.Gaussian((int)Width, 32000, Width * 1 / 3, Width / 10);
                    Data.Accumulate(data, Data.Uniform((int)Width, 1000));
                }
                else if (line < 390) // Excited.
                {
                    data = Data.Gaussian((int)Width, 32000, Width * 2 / 3, Width / 10);
                    Data.Accumulate(data, Data.Uniform((int)Width, 1000));
                }
            }
            else if (caller.Contains("CalibrateControl"))
            {
                if (line < 200) // Dark.
                {
                    data = Data.Uniform((int)Width, 1000);
                }
                else if (line > 200 && line < 220) // Blank.
                {
                    data = Enumerable.Repeat(55000, (int)Width).ToArray();
                    Data.Dissipate(data, Data.Uniform((int)Width, 1000));
                }
                else // Sample.
                {
                    data = Enumerable.Repeat(55000, (int)Width).ToArray();
                    Data.Dissipate(data, Data.Uniform((int)Width, 1000));
                    Data.Dissipate(data, Data.Gaussian((int)Width, 40000, Width * 2 / 3, Width / 10));
                }
            }

            if (data != null) data.CopyTo(DataBuffer, 0);
            return AndorSDK.DRV_SUCCESS;
        }

        public override void Close()
        {
        }
    }
}
