using System;

namespace LuiHardware.objects
{
    public class LuiObjectParametersEventArgs : EventArgs
    {
        public LuiObjectParametersEventArgs(LuiObjectParameters p)
        {
            Argument = p;
        }

        public LuiObjectParameters Argument { get; set; }
    }
}