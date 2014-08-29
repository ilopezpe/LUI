﻿//  <summary>
//      Constants used in LUI.
//  </summary>    

namespace LUI
{
    public static class Constants
    {
        public const float DefaultTemperatureF = 20F;
        public const int DefaultTemperature = 20;
        public const float TemperatureEps = 0.1F;

        // Serial constants and commands
        public const string OpenFlashCommand = "!0SO1";
        public const string CloseFlashCommand = "!0SO000";

        public const string OpenLaserCommand = "!0SO2";
        public const string CloseLaserCommand = "!0SO000";

        public const string OpenLaserAndFlashCommand = "!0S03";
        public const string CloseLaserAndFlashCommand = "!SO000";

        // Andor constants and commands
        public const int ReadModeFVB = 0;

        public const int AcqModeSingle = 1;
        public const int AcqModeAccumulate = 2;

        public const int TrigModeExternal = 1;
        public const int TrigModeExternalExposure = 7;

        // NI constants and GPIB commands
        public const int BoardNumber = 0;

        // Stanford DDG535
        public static class DDG535
        {
            public const string SetDelayTimeCommand = "DT ";
            public const string TriggerInput = "0";
            public const string T0Output = "1";
            public const string AOutput = "2";
            public const string BOutput = "3";
            public const string ABOutput = "4";
            public const string COutput = "5";
            public const string DOutput = "6";
            public const string CDOutput = "7";
        }
    }
}