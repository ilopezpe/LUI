﻿using System;
using System.Linq;
using lasercom.objects;

namespace lasercom.camera
{
    public class DummyCamera : AbstractCamera
    {
        private uint _Width;
        public override uint Width
        {
            get
            {
                return _Width;
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

        public override ImageArea Image { get; set; }

        public override int AcqWidth
        {
            get
            {
                if (ReadMode == AndorCamera.ReadModeFVB)
                {
                    return (int)Width;
                }
                else if (ReadMode == AndorCamera.ReadModeImage)
                {
                    return Image.Width;
                }
                else
                {
                    throw new NotImplementedException("Unsupported read mode.");
                }
            }
        }

        public override int AcqHeight
        {
            get
            {
                if (ReadMode == AndorCamera.ReadModeFVB)
                {
                    return (int)Height;
                }
                else if (ReadMode == AndorCamera.ReadModeImage)
                {
                    return Image.Height;
                }
                else
                {
                    throw new NotImplementedException("Unsupported read mode.");
                }
            }
        }

        public DummyCamera(LuiObjectParameters p) : this() { }

        public DummyCamera()
        {
            _Height = 255;
            _Width = 1024;
            Image = new ImageArea(1, 1, 0, (int)Width, 0, (int)Height);
            Calibration = Enumerable.Range(0, (int)Width).Select(x=>(double)x).ToArray();
            ReadMode = AndorCamera.ReadModeFVB;
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
            return null;
        }

        public override uint AcqSize
        {
            get { throw new NotImplementedException(); }
        }

        public override int AcquisitionMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int TriggerMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int DDGTriggerMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private int _ReadMode;
        public override int ReadMode
        {
            get
            {
                return _ReadMode;
            }
            set
            {
                _ReadMode = value;
            }
        }

        public override bool HasIntensifier
        {
            get { return false; }
        }

        public override int IntensifierGain
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int MinIntensifierGain
        {
            get { throw new NotImplementedException(); }
            protected set { throw new NotImplementedException(); }
        }

        public override int MaxIntensifierGain
        {
            get { throw new NotImplementedException(); }
            protected set { throw new NotImplementedException(); }
        }

        public override uint Acquire(int[] DataBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        public override string DecodeStatus(uint status)
        {
            return "DUMMY";
        }
    }
}
