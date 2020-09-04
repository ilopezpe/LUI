using lasercom.camera;
using System;
using System.Windows.Forms;

namespace LUI.controls
{
    class CameraTempControlledConfigPanel : AndorCameraConfigPanel
    {
        readonly LabeledControl<NumericUpDown> Temperature;

        public CameraTempControlledConfigPanel()
        {
            Temperature = new LabeledControl<NumericUpDown>(new NumericUpDown(), "Temperature (C):");
            Temperature.Control.Increment = 1;
            Temperature.Control.Minimum = -30;
            Temperature.Control.Maximum = 25;
            Temperature.Control.Value = lasercom.Constants.DefaultTemperature;
            Temperature.Control.ValueChanged += (s, e) => OnOptionsChanged(s, e);
            Controls.Add(Temperature);
        }

        public override Type Target => typeof(CameraTempControlled);

        public override void CopyFrom(CameraParameters other)
        {
            base.CopyFrom(other);
            Temperature.Control.Value = other.Temperature;
        }

        public override void CopyTo(CameraParameters other)
        {
            base.CopyTo(other);
            other.Temperature = (int)Temperature.Control.Value;
        }
    }
}