using LuiHardware.objects;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LuiHardware.ddg
{
    /// <summary>
    ///     Represents a Stanford Instruments DDG 535.
    /// </summary>
    public class DG535 : StanfordDigitalDelayGenerator
    {
        public const string SetDelayTimeCommand = "DT ";
        public const string TriggerInput = "0";
        public const string TOutput = "1";
        public const string AOutput = "2";
        public const string BOutput = "3";
        public const string ABOutput = "4";
        public const string COutput = "5";
        public const string DOutput = "6";
        public const string CDOutput = "7";
        public const string TName = "T";
        public const string AName = "A";
        public const string BName = "B";
        public const string ABName = "AB";
        public const string CName = "C";
        public const string DName = "D";
        public const string CDName = "CD";

        public const byte DefaultGPIBAddress = 15;
#pragma warning disable CS0108 // 'DG535.Log' hides inherited member 'LuiObject.Log'. Use the new keyword if hiding was intended.
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#pragma warning restore CS0108 // 'DG535.Log' hides inherited member 'LuiObject.Log'. Use the new keyword if hiding was intended.

        string _ADelay;

        string _BDelay;

        string _CDelay;

        string _DDelay;

        public DG535(LuiObjectParameters p, params ILuiObject[] dependencies)
            : this(p as DelayGeneratorParameters, dependencies)
        {
        }

        public DG535(DelayGeneratorParameters p, params ILuiObject[] dependencies) : base(p, dependencies)
        {
            ReadAllDelays();
        }

        public static Dictionary<string, string> DelayMap { get; } = new Dictionary<string, string>
        {
            // Forward lookup
            {TName, TOutput},
            {AName, AOutput},
            {BName, BOutput},
            {ABName, ABOutput},
            {CName, COutput},
            {DName, DOutput},
            {CDName, CDOutput},
            // Reverse lookup
            {TOutput, TName},
            {AOutput, AName},
            {BOutput, BName},
            {ABOutput, ABName},
            {COutput, CName},
            {DOutput, DName},
            {CDOutput, CDName}
        };

        public override string[] Delays
        {
            get { return new[] { AName, BName, CName, DName }; }
        }

        public override string[] DelayPairs
        {
            get { return new[] { ABName, CDName }; }
        }

        public override string[] Triggers
        {
            get { return new[] { TName, AName, BName, CName }; }
        }

        public string ADelay
        {
            get => _ADelay;
            set
            {
                _ADelay = value;
                SetDelay(AOutput, _ADelay);
            }
        }

        public string BDelay
        {
            get => _BDelay;
            set
            {
                _BDelay = value;
                SetDelay(BOutput, _BDelay);
            }
        }

        public string CDelay
        {
            get => _CDelay;
            set
            {
                _CDelay = value;
                SetDelay(COutput, _CDelay);
            }
        }

        public string DDelay
        {
            get => _DDelay;
            set
            {
                _DDelay = value;
                SetDelay(DOutput, _DDelay);
            }
        }

        /// <summary>
        ///     Sets any delay.
        /// </summary>
        /// <param name="DelayName"></param>
        /// <param name="TriggerName"></param>
        /// <param name="Delay"></param>
        public override void SetDelay(string DelayName, string TriggerName, double Delay)
        {
            var DelayOutput = DelayMap[DelayName];
            var TriggerOutput = DelayMap[TriggerName];
            SetNamedDelay(DelayOutput, TriggerOutput, Delay);
        }

        /// <summary>
        ///     Sets paired delays for pulse generation.
        /// </summary>
        /// <param name="DelayPair"></param>
        /// <param name="TriggerName"></param>
        /// <param name="Delay"></param>
        /// <param name="Width"></param>
        public override void SetDelayPulse(Tuple<string, string> DelayPair, string TriggerName, double Delay,
            double Width)
        {
            var DelayOutput1 = DelayMap[DelayPair.Item1];
            var DelayOutput2 = DelayMap[DelayPair.Item2];
            var TriggerOutput = DelayMap[TriggerName];
            SetNamedDelay(DelayOutput1, TriggerOutput, Delay);
            SetNamedDelay(DelayOutput2, DelayOutput1, Width);
        }

        /// <summary>
        ///     Internal use only - calls function to update property and write to device.
        /// </summary>
        /// <param name="DelayOutput"></param>
        /// <param name="TriggerOutput"></param>
        /// <param name="Delay"></param>
        void SetNamedDelay(string DelayOutput, string TriggerOutput, double Delay)
        {
            switch (DelayOutput)
            {
                case AOutput:
                    SetADelay(Delay, TriggerOutput);
                    break;

                case BOutput:
                    SetBDelay(Delay, TriggerOutput);
                    break;

                case COutput:
                    SetCDelay(Delay, TriggerOutput);
                    break;

                case DOutput:
                    SetDDelay(Delay, TriggerOutput);
                    break;

                default:
                    throw new ArgumentException("Illegal delay output given.");
            }
        }

        /// <summary>
        ///     Internal use only - writes delay to device.
        /// </summary>
        /// <param name="DelayOutput"></param>
        /// <param name="setting"></param>
        void SetDelay(string DelayOutput, string setting)
        {
            var command = SetDelayTimeCommand + DelayOutput + "," + setting;
            GPIBProvider.LoggedWrite(GPIBAddress, command);
        }

        /// <summary>
        ///     Sets A delay, may be used externally.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="relative"></param>
        public void SetADelay(double delay, string relative = TOutput)
        {
            ADelay = relative + "," + delay;
        }

        public void SetBDelay(double delay, string relative = TOutput)
        {
            BDelay = relative + "," + delay;
        }

        public void SetCDelay(double delay, string relative = TOutput)
        {
            CDelay = relative + "," + delay;
        }

        public void SetDDelay(double delay, string relative = TOutput)
        {
            DDelay = relative + "," + delay;
        }

        public override string GetDelay(string DelayName)
        {
            var DelayOutput = DelayMap[DelayName];
            return GetNamedDelay(DelayOutput);
        }

        public override string GetDelayTrigger(string DelayName)
        {
            return DelayMap[GetDelay(DelayName).Split(',')[0]];
        }

        public override double GetDelayValue(string DelayName)
        {
            return double.Parse(GetDelay(DelayName).Split(',')[1]);
        }

        /// <summary>
        ///     Internal use only - calls function to read from device.
        /// </summary>
        /// <param name="DelayOutput"></param>
        /// <param name="TriggerOutput"></param>
        /// <param name="Delay"></param>
        string GetNamedDelay(string DelayOutput)
        {
            switch (DelayOutput)
            {
                case AOutput:
                    return ADelay;

                case BOutput:
                    return BDelay;

                case COutput:
                    return CDelay;

                case DOutput:
                    return DDelay;

                default:
                    throw new ArgumentException("Illegal delay output given.");
            }
        }

        void ReadAllDelays()
        {
            ReadADelay();
            ReadBDelay();
            ReadCDelay();
            ReadDDelay();
        }

        void ReadADelay()
        {
            var command = SetDelayTimeCommand + AOutput;
            // e.g. "1,+0.001000000000"
            var response = GPIBProvider.LoggedQuery(GPIBAddress, command);
            _ADelay = response;
        }

        void ReadBDelay()
        {
            var command = SetDelayTimeCommand + BOutput;
            // e.g. "1,+0.001000000000"
            var response = GPIBProvider.LoggedQuery(GPIBAddress, command);
            _BDelay = response;
        }

        void ReadCDelay()
        {
            var command = SetDelayTimeCommand + COutput;
            // e.g. "1,+0.001000000000"
            var response = GPIBProvider.LoggedQuery(GPIBAddress, command);
            _CDelay = response;
        }

        void ReadDDelay()
        {
            var command = SetDelayTimeCommand + DOutput;
            // e.g. "1,+0.001000000000"
            var response = GPIBProvider.LoggedQuery(GPIBAddress, command);
            _DDelay = response;
        }

        public override void Update(DelayGeneratorParameters p)
        {
            base.Update(p);
            ReadAllDelays();
        }
    }
}