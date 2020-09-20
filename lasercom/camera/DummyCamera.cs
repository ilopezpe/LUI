using lasercom.objects;
using System;
using System.Linq;

namespace lasercom.camera
{
    public class DummyCamera : AbstractCamera
    {
        public DummyCamera(LuiObjectParameters p) : this()
        {
        }

        public DummyCamera()
        {
            Height = 255;
            Width = 1024;
            Image = new ImageArea(1, 1, 0, Width, 0, Height);
            Calibration = Enumerable.Range(0, Width).Select(x => (double)x).ToArray();
            ReadMode = AndorCamera.ReadModeFVB;
        }

        public override int Width { get; }

        public override int Height { get; }

        public override ImageArea Image { get; set; }

        public override int AcqWidth
        {
            get
            {
                if (ReadMode == AndorCamera.ReadModeFVB)
                    return Width;
                if (ReadMode == AndorCamera.ReadModeImage)
                    return Image.Width;
                throw new NotImplementedException("Unsupported read mode.");
            }
        }

        public override int AcqHeight
        {
            get
            {
                if (ReadMode == AndorCamera.ReadModeFVB)
                    return Height;
                if (ReadMode == AndorCamera.ReadModeImage)
                    return Image.Height;
                throw new NotImplementedException("Unsupported read mode.");
            }
        }

        public override int AcqSize => throw new NotImplementedException();

        public override int AcquisitionMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override int TriggerMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override int DDGTriggerMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override int ReadMode { get; set; }

        public override bool HasIntensifier => false;

        public override int IntensifierGain
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override int MinIntensifierGain
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
        }

        public override int MaxIntensifierGain
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
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