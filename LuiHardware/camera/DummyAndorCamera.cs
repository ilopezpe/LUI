using ATMCD32CS;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuiHardware.camera
{
    public class DummyAndorCamera : AndorTempControlled
    {
        ImageArea _Image;

        float _TargetTemperature;

        float _Temperature;

        new AndorSDK AndorSdk = null;

        readonly CancellationTokenSource Cts;

        public DummyAndorCamera(CameraParameters p)
        {
            if (p == null)
                throw new ArgumentException("Non-null CameraParameters instance required");

            _MinTemp = -100;
            _MaxTemp = 100;
            _Width = 1024;
            _Height = 256;
            _Image = new ImageArea(1, 1, 0, Width, 0, Height);
            Image = p.Image;
            Calibration = Enumerable.Range(0, Width).Select(x => (double)x).ToArray();
            LoadCalibration(p.CalFile);
            MinIntensifierGain = 0;
            MaxIntensifierGain = 4095;
            IntensifierGain = p.InitialGain;
            ReadMode = p.ReadMode;
            p.Image = Image;
            p.ReadMode = ReadMode;
            _Temperature = 25.0F; // Room temperature.
            Cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (true)
                {
                    if (Cts.IsCancellationRequested) break;
                    _Temperature += 0.01F * (_TargetTemperature - _Temperature);
                    Thread.Sleep(10);
                }
            }, Cts.Token);
            EquilibrateTemperature(p.Temperature);
        }

        public override int ReadMode { get; set; }

        public override int AcquisitionMode { get; set; }

        public override int TriggerMode { get; set; }

        public override int TriggerInvert { get; set; }

        public override float TriggerLevel { get; set; }

        public override int DDGTriggerMode { get; set; }

        public override int GateMode { get; set; }

        public override bool HasIntensifier => true;

        public override int MCPGating { get; set; }

        public override int IntensifierGain { get; set; }

        public override int NumberAccumulations { get; set; }

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
                    hbin = Math.Min(hbin, hcount); // At most image width.
                }

                if (value.vbin == -1)
                {
                    vbin = _Image.vbin;
                }
                else
                {
                    vbin = Math.Max(1, value.vbin); // At least 1.
                    vbin = Math.Min(vbin, vcount); // At most image height.
                }

                _Image = new ImageArea(hbin, vbin,
                    hstart, hcount,
                    vstart, vcount);
            }
        }

        public override int Temperature => (int)_Temperature;

        public override float TemperatureF => _Temperature;

        public override uint TemperatureStatus
        {
            get
            {
                if (Math.Round(_Temperature) == Math.Round(_TargetTemperature))
                    return AndorSDK.DRV_TEMP_STABILIZED;
                if (Math.Abs(_Temperature - _TargetTemperature) <= 3)
                    return AndorSDK.DRV_TEMP_NOT_STABILIZED;
                return AndorSDK.DRV_TEMP_NOT_REACHED;
            }
        }

        int TargetTemperature
        {
            set => _TargetTemperature = value;
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
            var data = new int[Width];
            Acquire(data);
            return data;
        }

        public override uint Acquire(int[] DataBuffer)
        {
            Thread.Sleep(250);
            var frame = 1;
            var caller = new StackFrame(frame, true).GetFileName();
            if (caller.Contains("DummyAndorCamera")) frame++;
            caller = new StackFrame(frame, true).GetMethod().Name;
            while (caller == "DoAcq" || caller == "TryAcquire") caller = new StackFrame(++frame, true).GetMethod().Name;
            caller = new StackFrame(frame, true).GetFileName();
            var line = new StackFrame(frame, true).GetFileLineNumber();
            int[] data = null;
            if (caller.Contains("AbsControl"))
                data = Abs(line);
            else if (caller.Contains("TransientAbsControl"))
                data = TransientAbs(line);
            else if (caller.Contains("TroaControl"))
                data = Troa(line);
            else if (caller.Contains("LdalignControl"))
                data = Troa(line);
            else if (caller.Contains("LdextinctionControl"))
                data = Residuals();
            else if (caller.Contains("TrldControl"))
                data = Troa(line);
            else if (caller.Contains("CalibrateControl"))
                data = Calibrate(line);
            else if (caller.Contains("ResidualsControl"))
                data = Residuals();
            else if (caller.Contains("DetectorTestForm")) data = Blank();

            if (data != null) data.CopyTo(DataBuffer, 0);
            return AndorSDK.DRV_SUCCESS;
        }

        public override uint AcquireImage(int[] DataBuffer)
        {
            return Acquire(DataBuffer);
        }

        int[] Dark(int scale = 1000)
        {
            return ReadMode == ReadModeImage ? DarkImage(scale) : Data.Uniform(Image.Width, scale);
        }

        int[] DarkImage(int scale)
        {
            var data = new int[AcqSize];
            for (var i = 0; i < Image.Height; i++)
            {
                var row = Data.Uniform(Image.Width, scale);
                for (var j = 0; j < Image.Width; j++) data[i * Image.Width + j] = row[j];
            }

            return data;
        }

        int[] Blank(int scale = 55000)
        {
            return ReadMode == ReadModeFVB ? BlankFvb(scale) : BlankImage(scale);
        }

        int[] BlankFvb(int scale)
        {
            var data = Data.Uniform(Width, 1000);
            for (var i = 1; i < 10; i++)
                Data.Accumulate(data, Data.Gaussian(Width, scale, Width * i / 10D, Width / 10D));
            return data;
        }

        int[] BlankImage(int scale)
        {
            var data = new int[AcqSize];
            for (var i = 0; i < Image.Height; i++)
            {
                var ymax = Image.Height / 2;
                var yscale = (int)(scale * (1 - Math.Abs((double)i - ymax) / ymax));
                var row = BlankFvb(yscale);
                for (var j = 0; j < Image.Width; j++) data[i * Image.Width + j] = row[j];
            }

            return data;
        }

        int[] SampleData(double centerNormalized = 0.5, int scale = 32000)
        {
            return ReadMode == ReadModeImage
                ? GenerateDataImage(centerNormalized, scale)
                : GenerateData(centerNormalized, scale);
        }

        int[] GenerateData(double centerNormalized, int scale)
        {
            var data = Data.Gaussian(Width, scale, Width * centerNormalized, Width / 10D);
            return data;
        }

        int[] GenerateDataImage(double centerNormalized, int scale)
        {
            var data = new int[AcqSize];
            for (var i = 0; i < Image.Height; i++)
            {
                var row = Data.Gaussian(Image.Width, scale, Image.Width * centerNormalized, Width / 10);
                for (var j = 0; j < Image.Width; j++) data[i * Image.Width + j] = row[j];
            }

            return data;
        }

        int[] Abs(int line)
        {
            int[] data = null;
            if (line == 155) // Dark.
            {
                data = Dark();
            }
            else if (line == 171) // Blank.
            {
                data = Blank();
            }
            else if (line == 199) // Excited.
            {
                data = Blank();
                Data.Dissipate(data, SampleData());
                for (var i = 0; i < data.Length; i++) data[i] += 1000;
            }

            return data;
        }

        int[] Troa(int line)
        {
            int[] data;
            if (line == 600) // Dark.
            {
                data = Dark();
            }
            else if (line == 609 || line == 626 || line == 631) // Ground.
            {
                data = Blank();
                Data.Dissipate(data, SampleData(0.3333));
                for (var i = 0; i < data.Length; i++) data[i] += 1000;
            }
            else // Excited.
            {
                data = Blank();
                Data.Dissipate(data, SampleData(0.6667));
                for (var i = 0; i < data.Length; i++) data[i] += 1000;
            }

            return data;
        }

        int[] Calibrate(int line)
        {
            int[] data;
            if (line == 207) // Dark.
            {
                data = Dark();
            }
            else if (line == 223) // Blank.
            {
                data = Blank();
            }
            else // Sample.
            {
                data = Blank();
                Data.Dissipate(data, SampleData());
            }

            return data;
        }

        int[] Residuals()
        {
            return Blank();
        }

        int[] TransientAbs(int line)
        {
            int[] data;
            if (line == 132) // Dark.
            {
                data = Dark();
            }
            else if (line == 154) // Ground.
            {
                data = Blank();
                Data.Dissipate(data, SampleData(0.3333));
                for (var i = 0; i < data.Length; i++) data[i] += 1000;
            }
            else // Excited.
            {
                data = Blank();
                Data.Dissipate(data, SampleData(0.6667));
                for (var i = 0; i < data.Length; i++) data[i] += 1000;
            }

            return data;
        }

        public override void Close()
        {
            Cts.Cancel();
        }

        public override void EquilibrateTemperature(int targetTemperature, CancellationToken? token = null)
        {
            if (targetTemperature < MinTemp || targetTemperature > MaxTemp)
                throw new ArgumentException("Temperature out of range.");
            TargetTemperature = targetTemperature;
            while (Math.Abs(_Temperature - _TargetTemperature) > TemperatureEps)
            {
                if (token.HasValue && token.Value.IsCancellationRequested) break;
                Thread.Sleep(50);
            }
        }

        public override void EquilibrateTemperature(CancellationToken? token = null)
        {
            while (TemperatureStatus != AndorSDK.DRV_TEMP_STABILIZED)
            {
                if (token.HasValue && token.Value.IsCancellationRequested) break;
                Thread.Sleep(50);
            }
        }

        public override bool EquilibrateUntil(Func<bool> BreakoutCondition, int PollDelayMs)
        {
            while (TemperatureStatus != AndorSDK.DRV_TEMP_STABILIZED)
            {
                if (BreakoutCondition()) return true;
                Thread.Sleep(PollDelayMs);
            }

            return false;
        }

        public override void WaitForTemperatureIncrease(int thresholdTemperature)
        {
            while (Temperature < thresholdTemperature - TemperatureEps) Thread.Sleep(50);
        }
    }
}