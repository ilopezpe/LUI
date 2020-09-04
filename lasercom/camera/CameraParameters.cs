using lasercom.objects;
using System;
using System.Runtime.Serialization;

namespace lasercom.camera
{
    [DataContract]
    public class CameraParameters : LuiObjectParameters<CameraParameters>
    {
        public int HBin = -1;
        public int HCount = -1;
        public int HStart = -1;
        public int VBin = -1;
        public int VCount = -1;
        public int VStart = -1;

        public CameraParameters(Type Type)
            : base(Type)
        {
        }

        public CameraParameters()
        {
        }

        public CameraParameters(CameraParameters other)
            : base(other)
        {
        }

        [DataMember] public string CalFile { get; set; }

        [DataMember] public string Dir { get; set; }

        [DataMember] public int Temperature { get; set; }

        [DataMember] public int InitialGain { get; set; }

        [DataMember] public int SaturationLevel { get; set; }

        [DataMember]
        //private ImageArea _Image = new ImageArea(-1, -1, -1, -1, -1, -1);
        public ImageArea Image
        {
            get => new ImageArea(HBin, VBin, HStart, HCount, VStart, VCount);
            set
            {
                //_Image = value;
                HBin = value.hbin;
                VBin = value.vbin;
                HStart = value.hstart;
                HCount = value.hcount;
                VStart = value.vstart;
                VCount = value.vcount;
            }
        }

        [DataMember] public int ReadMode { get; set; }

        public override void Copy(CameraParameters other)
        {
            base.Copy(other);
            //this.Type = other.Type;
            //this.Name = other.Name;
            CalFile = other.CalFile;
            Dir = other.Dir;
            Temperature = other.Temperature;
            InitialGain = other.InitialGain;
            Image = other.Image;
            ReadMode = other.ReadMode;
            SaturationLevel = other.SaturationLevel;
        }

        public override bool NeedsReinstantiation(CameraParameters other)
        {
            var needs = base.NeedsReinstantiation(other); // Type is different.
            if (needs) return true;

            if (Type == typeof(AndorCamera) || Type.IsSubclassOf(typeof(AndorCamera)))
                needs |= other.Dir != Dir; // Or if Dir is different.

            return needs;
        }

        public override bool NeedsUpdate(CameraParameters other)
        {
            var iseq = CalFile == other.CalFile;

            if (Type == typeof(AndorCamera) || Type.IsSubclassOf(typeof(AndorCamera)))
            {
                iseq &= other.InitialGain == InitialGain;
                iseq &= other.ReadMode == ReadMode;
                iseq &= other.HBin == HBin;
                iseq &= other.VBin == VBin;
                iseq &= other.HStart == HStart;
                iseq &= other.HCount == HCount;
                iseq &= other.VStart == VStart;
                iseq &= other.VCount == VCount;
                iseq &= other.SaturationLevel == SaturationLevel;
            }

            if (Type == typeof(CameraTempControlled)) iseq &= Temperature == other.Temperature;
            return !iseq; // True if any of these field differ.
        }
    }
}