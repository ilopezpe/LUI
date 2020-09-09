using lasercom.camera;
using lasercom.syringepump;
using lasercom.beamflags;
using lasercom.ddg;
using log4net;
using System.Collections.Generic;
using System.Reflection;
using lasercom.polarizer;

namespace lasercom
{
    public class Commander
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Commander(ICamera camera = null,
                         IBeamFlags beamFlags = null,
                         IDigitalDelayGenerator ddg = null,
                         ISyringePump pump = null,
                         IPolarizer polarizer = null)
        {
            // Set dummies instead of null values to save a *ton* of null checks elsewhere.
            Camera = camera ?? new DummyCamera();
            BeamFlag = beamFlags ?? new DummyBeamFlags();
            DDG = ddg ?? new DummyDigitalDelayGenerator();
            SyringePump = pump ?? new DummySyringePump();
            Polarizer = polarizer ?? new DummyPolarizer();
        }

        public ICamera Camera { get; set; }
        public IBeamFlags BeamFlag { get; set; }
        public IDigitalDelayGenerator DDG { get; set; }
        public ISyringePump SyringePump { get; set; }
        public IPolarizer Polarizer { get; set; }
        public List<double> Delays { get; set; }

        public void SetDelays(string file)
        {
            // read file
            Delays = new List<double>();
        }

        public int[] Collect(int n)
        {
            for (var i = 0; i < n; i++) Camera.CountsFvb();
            return null;
        }

        public int[] Dark()
        {
            BeamFlag.CloseLaserAndFlash();
            return Camera.Acquire();
        }

        public uint Dark(int[] dataBuffer)
        {
            BeamFlag.CloseLaserAndFlash();
            return Camera.Acquire(dataBuffer);
        }

        public int[] Flash()
        {
            BeamFlag.CloseLaserAndFlash();
            BeamFlag.OpenFlash();
            var data = Camera.Acquire();
            BeamFlag.CloseLaserAndFlash();
            return data;
        }

        public uint Flash(int[] dataBuffer)
        {
            BeamFlag.CloseLaserAndFlash();
            BeamFlag.OpenFlash();
            var ret = Camera.Acquire(dataBuffer);
            BeamFlag.CloseLaserAndFlash();
            return ret;
        }

        public int[] Trans()
        {
            BeamFlag.OpenLaserAndFlash();
            var data = Camera.Acquire();
            BeamFlag.CloseLaserAndFlash();
            return data;
        }

        public uint Trans(int[] dataBuffer)
        {
            BeamFlag.OpenLaserAndFlash();
            var ret = Camera.Acquire(dataBuffer);
            BeamFlag.CloseLaserAndFlash();
            return ret;
        }
    }
}