using ATMCD64CS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LUI
{
    class HardwarePoll
    {

        public static void Main(String[] args)
        {
            AndorSDK.AndorCapabilities capabilities = new AndorSDK.AndorCapabilities();
            int capsSize = Marshal.SizeOf(capabilities);
            capabilities.ulSize = (uint)capsSize;
            AndorSDK andorSdk = new AndorSDK();
            uint initFlag = andorSdk.Initialize(args[0]);
            Console.WriteLine(initFlag);
            uint getCapsFlag = andorSdk.GetCapabilities(ref capabilities);
            Console.WriteLine(getCapsFlag);
            //Type capsType = typeof (AndorSDK.AndorCapabilities);
            //FieldInfo[] fields = capsType.GetFields();
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                /*foreach (FieldInfo field in fields)
                {
                    string fieldName = field.Name;
                    uint fieldValue = (uint) field.GetValue(capabilities);
                    string fieldBits = Convert.ToString(fieldValue, 2); // string of uint bits
                    dictionary.Add(fieldName, fieldBits);
                }*/
                { "ulSize", Convert.ToString(capabilities.ulSize, 10) },
                { "ulReadModes", Convert.ToString(capabilities.ulReadModes, 2) },
                { "ulTriggerModes", Convert.ToString(capabilities.ulTriggerModes, 2) },
                { "ulCameraType", Convert.ToString(capabilities.ulCameraType, 10) },
                { "ulPixelMode", Convert.ToString(capabilities.ulPixelMode, 2) },
                { "ulSetFunctions", Convert.ToString(capabilities.ulSetFunctions, 2) },
                { "ulGetFunctions", Convert.ToString(capabilities.ulGetFunctions, 2) },
                { "ulFeatures", Convert.ToString(capabilities.ulFeatures, 2) },
                { "ulPCICard", Convert.ToString(capabilities.ulPCICard, 2) },
                { "ulEMGainCapability", Convert.ToString(capabilities.ulEMGainCapability, 2) }
            };

            using (StreamWriter writer = new StreamWriter(new FileStream(args[1], FileMode.Create)))
            {
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    writer.WriteLine(pair.Key + "\t" + pair.Value);
                }
            }
        }
    }
}
