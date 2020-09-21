using System;
using ATMCD64CS;
using LuiHardware.Camera;
using LuiHardware.objects;

namespace LuiHardware.camera
{
    /// <summary>
    ///     Class representing a generic Andor camera.
    ///     Specific Andor camera types should inherit from this class.
    /// </summary>
    public class AndorCamera : AbstractCamera
    {
        public const int TriggerInvertRising = 0;
        public const int TriggerInvertFalling = 1;
        public const float DefaultTriggerLevel = 3.9F;

        public const int MCPGatingOff = 0;
        public const int MCPGatingOn = 1;
        public const int DefaultMCPGain = 10;

        public const int DefaultADChannel = 0;

        readonly int _BitDepth;

        readonly int _MaxHorizontalBinSize;

        readonly int _MaxVerticalBinSize;

        readonly int _NumberADChannels;

        int _AcquisitionMode;

        int _CurrentADChannel;

        int _DDGTriggerMode;

        int _GateMode;

        protected int _Height;

        ImageArea _Image;

        int _MaxMCPGain;

        int _MCPGain;

        int _MCPGating;

        int _MinMCPGain;

        int _NumberAccumulations;

        int _ReadMode;

        int _SaturationLevel;

        int _TriggerInvert;

        float _TriggerLevel;

        int _TriggerMode;

        protected int _Width;
        public AndorSDK AndorSdk = new AndorSDK();
        public AndorSDK.AndorCapabilities Capabilities;

        public uint InitVal;

        public AndorCamera()
        {
        }

        public AndorCamera(LuiObjectParameters p) :
            this(p as CameraParameters)
        {
        }

        public AndorCamera(CameraParameters p)
        {
            if (p == null) throw new ArgumentException();

            if (p.Dir != null)
            {
                InitVal = AndorSdk.Initialize(p.Dir);
                AndorSdk.GetCapabilities(ref Capabilities);
                AndorSdk.FreeInternalMemory();
                AndorSdk.GetDetector(ref _Width, ref _Height);
                AndorSdk.GetNumberADChannels(ref _NumberADChannels);
                CurrentADChannel = DefaultADChannel;
                AndorSdk.GetBitDepth(CurrentADChannel, ref _BitDepth);
                SaturationLevel = p.SaturationLevel;

                AndorSdk.GetMaximumBinning(ReadModeImage, 0, ref _MaxHorizontalBinSize);
                AndorSdk.GetMaximumBinning(ReadModeImage, 1, ref _MaxVerticalBinSize);

                _Image = new ImageArea(1, 1, 0, Width, 0, Height);
                Image = p.Image;

                GateMode = Constants.GatingModeSMBOnly;
                MCPGating = Constants.MCPGatingOn;

                //TriggerInvert = Constants.TriggerInvertRising;
                //TriggerLevel = Constants.DefaultTriggerLevel; // TTL signal is 4.0V
                AndorSdk.GetMCPGainRange(ref _MinMCPGain, ref _MaxMCPGain);
                IntensifierGain = p.InitialGain;

                AcquisitionMode = AcquisitionModeSingle;
                TriggerMode = TriggerModeExternalExposure;
                DDGTriggerMode = DDGTriggerModeExternal;
                ReadMode = p.ReadMode;
            }

            LoadCalibration(p.CalFile);

            p.Image = Image;
            p.ReadMode = ReadMode;
        }

        public override int ReadMode
        {
            get => _ReadMode;
            set
            {
                _ReadMode = value;
                AndorSdk.SetReadMode(value);
            }
        }

        public override int AcquisitionMode
        {
            get => _AcquisitionMode;
            set
            {
                _AcquisitionMode = value;
                AndorSdk.SetAcquisitionMode(value);
            }
        }

        public override int TriggerMode
        {
            get => _TriggerMode;
            set
            {
                _TriggerMode = value;
                AndorSdk.SetTriggerMode(value);
            }
        }

        public virtual int TriggerInvert
        {
            get => _TriggerInvert;
            set
            {
                _TriggerInvert = value;
                AndorSdk.SetTriggerInvert(value);
            }
        }

        public virtual float TriggerLevel
        {
            get => _TriggerLevel;
            set
            {
                _TriggerLevel = value;
                AndorSdk.SetTriggerLevel(value);
            }
        }

        public override int DDGTriggerMode
        {
            get => _DDGTriggerMode;
            set
            {
                _DDGTriggerMode = value;
                AndorSdk.SetDDGTriggerMode(value);
            }
        }

        public virtual int GateMode
        {
            get => _GateMode;
            set
            {
                _GateMode = value;
                AndorSdk.SetGateMode(value);
            }
        }

        public override bool HasIntensifier => true;

        public virtual int MCPGating
        {
            get => _MCPGating;
            set
            {
                _MCPGating = value;
                AndorSdk.SetMCPGating(value);
            }
        }

        public override int MinIntensifierGain
        {
            get => _MinMCPGain;
            protected set => _MinMCPGain = value;
        }

        public override int MaxIntensifierGain
        {
            get => _MaxMCPGain;
            protected set => _MaxMCPGain = value;
        }

        public override int IntensifierGain
        {
            get => _MCPGain;
            set
            {
                _MCPGain = value;
                AndorSdk.SetMCPGain(value);
            }
        }

        public virtual int NumberAccumulations
        {
            get => _NumberAccumulations;
            set
            {
                _NumberAccumulations = value;
                AndorSdk.SetNumberAccumulations(value);
            }
        }

        public override ImageArea Image
        {
            get => _Image;
            set
            {
                int hbin, vbin, hstart, hcount, vstart, vcount;

                if (value.hcount == -1)
                {
                    hcount = _Image.hcount;
                }
                else
                {
                    hcount = Math.Max(1, value.hcount); // At least 1.
                    hcount = Math.Min(Width, hcount); // At most Width.
                }

                if (value.vcount == -1)
                {
                    vcount = _Image.vcount;
                }
                else
                {
                    vcount = Math.Max(1, value.vcount); // At least 1.
                    vcount = Math.Min(Height, vcount); // At most Height.
                }

                if (value.hstart == -1)
                {
                    hstart = _Image.hstart;
                }
                else
                {
                    hstart = Math.Max(0, value.hstart); // At least 0.
                    hstart = Math.Min(hstart, Width - 1); // At most Width - 1.
                }

                if (value.vstart == -1)
                {
                    vstart = _Image.vstart;
                }
                else
                {
                    vstart = Math.Max(0, value.vstart); // At least 0.
                    vstart = Math.Min(vstart, Height - 1); // At most Height - 1.
                }

                if (value.hbin == -1)
                {
                    hbin = _Image.hbin;
                }
                else
                {
                    hbin = Math.Max(1, value.hbin); // At least 1.
                    hbin = Math.Min(Math.Min(hbin, hcount), MaxHorizontalBinSize); // At most lesser of image width and max hbin.
                }

                if (value.vbin == -1)
                {
                    vbin = _Image.vbin;
                }
                else
                {
                    vbin = Math.Max(1, value.vbin); // At least 1.
                    vbin = Math.Min(Math.Min(vbin, vcount),MaxVerticalBinSize); // At most lesser of image height and max vbin.
                }

                _Image = new ImageArea(hbin, vbin,
                    hstart, hcount,
                    vstart, vcount);

                var ret = AndorSdk.SetImage(
                    _Image.hbin, _Image.vbin,
                    _Image.hstart + 1, _Image.hstart + _Image.hcount,
                    _Image.vstart + 1, _Image.vstart + _Image.vcount);
                Log.Debug(ErrorCodes.Decoder(ret));
            }
        }

        public override int Height => _Height;

        public override int Width => _Width;

        public int BitDepth => _BitDepth;
        public int NumberADChannels => _NumberADChannels;

        public int CurrentADChannel
        {
            get => _CurrentADChannel;
            set
            {
                //TODO check return from Andor
                //TODO ALL properties in this class need this fix and code reorder to reflect
                AndorSdk.SetADChannel(value);
                _CurrentADChannel = value;
            }
        }

        public int MaxHorizontalBinSize => _MaxHorizontalBinSize;

        public int MaxVerticalBinSize => _MaxVerticalBinSize;

        public override int AcqSize
        {
            get
            {
                if (ReadMode == ReadModeFVB)
                    return Width;
                if (ReadMode == ReadModeImage)
                    return Image.Width * Image.Height;
                throw new NotImplementedException("Unsupported read mode.");
            }
        }

        public override int AcqWidth
        {
            get
            {
                if (ReadMode == ReadModeFVB)
                    return Width;
                if (ReadMode == ReadModeImage)
                    return Image.Width;
                throw new NotImplementedException("Unsupported read mode.");
            }
        }

        public override int AcqHeight
        {
            get
            {
                if (ReadMode == ReadModeFVB)
                    return Height;
                if (ReadMode == ReadModeImage)
                    return Image.Height;
                throw new NotImplementedException("Unsupported read mode.");
            }
        }

        public override int SaturationLevel
        {
            get => _SaturationLevel;
            set
            {
                if (value >= Math.Pow(2, BitDepth))
                    throw new ArgumentException("Saturation level may not exceed 2^BitDepth - 1.");
                _SaturationLevel = value;
            }
        }

        public virtual void Close()
        {
            if (AndorSdk != null) AndorSdk.ShutDown();
        }

        public override int[] FullResolutionImage()
        {
            var image = Image;
            var readMode = ReadMode;
            ReadMode = ReadModeImage;
            Image = new ImageArea(1, 1, 0, Width, 0, Height);
            var npx = (uint) (Width * Height);
            var data = new int[npx];
            AndorSdk.StartAcquisition();
            AndorSdk.WaitForAcquisition();
            AndorSdk.GetAcquiredData(data, npx);
            Image = image;
            ReadMode = readMode;
            return data;
        }

        public override int[] CountsFvb()
        {
            var npx = (uint) Width;
            var readMode = ReadMode;
            ReadMode = ReadModeFVB;
            var data = new int[npx];
            AndorSdk.StartAcquisition();
            AndorSdk.WaitForAcquisition();
            AndorSdk.GetAcquiredData(data, npx);
            ReadMode = readMode;
            return data;
        }

        public override int[] Acquire()
        {
            var npx = (uint) AcqSize;
            var data = new int[npx];
            Acquire(data);
            return data;
        }

        /// <summary>
        ///     Acquire data and store in referenced array.
        ///     This overload supports memory efficient acquisition if the same
        ///     array is continually re-passed.
        ///     The array must be a legal size for acquisition.
        /// </summary>
        /// <param name="DataBuffer"></param>
        /// <returns></returns>
        public override uint Acquire(int[] DataBuffer)
        {
            var npx = (uint) DataBuffer.Length;
            AndorSdk.StartAcquisition();
            AndorSdk.WaitForAcquisition();
            var ret = AndorSdk.GetAcquiredData(DataBuffer, npx);
            Log.Debug("Camera returned " + ErrorCodes.Decoder(ret));
            ThrowIfSaturated(DataBuffer);
            return ret;
        }

        public virtual uint AcquireImage(int[] DataBuffer)
        {
            var npx = (uint) DataBuffer.Length;
            AndorSdk.StartAcquisition();
            AndorSdk.WaitForAcquisition();
            var ret = AndorSdk.GetMostRecentImage(DataBuffer, npx);
            Log.Debug("Camera returned " + ErrorCodes.Decoder(ret));
            ThrowIfSaturated(DataBuffer);
            return ret;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Close();
        }

        protected void ThrowIfSaturated(int[] data)
        {
            for (var i = 0; i < data.Length; i++)
                if (data[i] >= SaturationLevel)
                {
                    var ex = new InvalidOperationException("Sensor saturation detected.");
                    ex.Data["Pixel"] = i;
                    ex.Data["Value"] = data[i];
                    throw ex;
                }
        }

        public override string DecodeStatus(uint status)
        {
            throw new NotImplementedException();
        }

        #region Andor Constants

        public const int ReadModeFVB = 0;

        public const int ReadModeMultiTrack = 1;
        public const int ReadModeRandomTrack = 2;
        public const int ReadModeSingleTrack = 3;
        public const int ReadModeImage = 4;

        public const int AcquisitionModeSingle = 1;
        public const int AcquisitionModeAccumulate = 2;

        public const int GatingModeSMBOnly = 2;

        public const int DDGTriggerModeInternal = 0;
        public const int DDGTriggerModeExternal = 1;

        public const int TriggerModeExternal = 1;
        public const int TriggerModeExternalExposure = 7;

        #endregion
    }
}