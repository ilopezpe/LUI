﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lasercom.gpib
{
    public interface IGpibProvider : IDisposable
    {
        void LoggedWrite(byte address, string command);
        string LoggedQuery(byte address, string command);
    }
}
