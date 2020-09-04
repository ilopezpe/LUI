using lasercom.objects;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace lasercom.gpib
{
    /// <summary>
    /// Provide GPIB using Prologix USB GPIB controller.
    /// </summary>
    public class PrologixGpibProvider : AbstractGpibProvider
    {
        #region Constants
        private const string PrologixInitiator = "++";
        private const string PrologixAddress = "addr";
        private const string PrologixIFC = "ifc";
        private const string PrologixMode = "mode 1";
        private const string PrologixEOI = "eoi 1";
        private const string PrologixEOS = "eos 0"; // 0 = CRLF termination
        private const string PrologixRead = "read";
        private const string ReadTimeoutCommand = "read_tmo_ms";
        #endregion

        SerialPort _port;

        public string PortName
        {
            get
            {
                return _port.PortName;
            }
        }

        int Timeout { get; set; }
        const int DefaultTimeout = 500;

        public PrologixGpibProvider(string PortName)
        {
            Init(PortName, DefaultTimeout);
        }

        public PrologixGpibProvider(LuiObjectParameters p) : this(p as GpibProviderParameters) { }

        public PrologixGpibProvider(GpibProviderParameters p)
        {
            if (p == null || p.PortName == null)
            {
                throw new ArgumentException("PortName must be defined.");
            }

            Init(p.PortName, p.Timeout);
        }

        private void Init(string PortName, int Timeout)
        {
            #region Serial port configuration
            _port = new SerialPort(PortName)
            {
                BaudRate = 1200, //9600 ???
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.RequestToSend,
                RtsEnable = true,
                DtrEnable = true,
                Encoding = Encoding.ASCII,
                DiscardNull = false,
                ParityReplace = 0,
                ReadTimeout = Timeout,
                WriteTimeout = Timeout
            };
            #endregion

            this.Timeout = Timeout;
            ControllerCommand(PrologixIFC); // Assert Controller-in-Charge.
            ControllerCommand(PrologixMode); // Disable listen-only mode.
            ControllerCommand(PrologixEOI); // Assert EOI after transmit GPIB data.
            ControllerCommand(PrologixEOS); // Use CRLF as GPIB terminator.
        }

        void Open()
        {
            _port.Open();
        }

        void Close()
        {
            if (_port.IsOpen) _port.Close();
            Thread.Sleep(Constants.SerialPortCloseDelay); // Prevents subsequent call from interfering with close.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
            }
        }

        void ControllerCommand(string command)
        {
            string data = PrologixInitiator + command + "\r\n";
            Log.Debug("USB GPIB Controller Command: " + data);
            try
            {
                if (!_port.IsOpen) _port.Open();
                _port.Write(data);
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }

        void ControllerCommand(string command, params string[] args)
        {
            string arglist = String.Join(" ", args);
            string data = PrologixInitiator + command + " " + arglist + "\r\n";
            Log.Debug("USB GPIB Controller Command: " + data);
            try
            {
                if (!_port.IsOpen) _port.Open();
                _port.Write(data);
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }

        override public void LoggedWrite(byte address, string command)
        {
            Log.Debug("GPIB Command: " + command);
            string TX = EscapeString(command) + "\r\n"; // send to instrument
            try
            {
                if (!_port.IsOpen) _port.Open();
                ControllerCommand(PrologixAddress, address.ToString());

                _port.Write(TX + "\r\n");
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }

        override public string LoggedQuery(byte address, string command)
        {
            Log.Debug("GPIB Command: " + command);
            string TX = EscapeString(command) + "\r\n"; // send to instrument
            string buffer = null;
            try
            {
                if (!_port.IsOpen) _port.Open();
                ControllerCommand(PrologixAddress, address.ToString());
                _port.Write(TX);
                buffer = ReadWithTimeout();
                buffer = buffer.TrimEnd("\r\n".ToCharArray());
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
            Log.Debug("GPIB Response: " + buffer);
            return buffer;
        }

        string ReadWithTimeout()
        {
            return ReadWithTimeout(Timeout);
        }

        /// <summary>
        /// Read from addressed device until timeout reached between read characters.
        /// Note GPIB data is stored in 1 character buffer, then sent to 4K USB buffer.
        /// Thus there are two effective timeout required, one for reading GPIB and one
        /// for reading from the serial port (USB buffer).
        /// </summary>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        string ReadWithTimeout(int Timeout)
        {
            if (!_port.IsOpen) throw new InvalidOperationException("The specified port is not open");

            ControllerCommand(ReadTimeoutCommand, (Timeout + 1).ToString()); // Allow 1 ms for GPIB -> USB.
            ControllerCommand(PrologixRead, "eoi"); // Read from GPIB until eoi or timeout.

            StringBuilder builder = new StringBuilder();
            DateTime lastRead = DateTime.Now;
            TimeSpan elapsedTime = new TimeSpan();

            // 2 second timespan
            TimeSpan TimeSpan = new TimeSpan(0, 0, 0, 0, Timeout);

            // Read from port until TIMEOUT time has elapsed since
            // last successful read
            while (TimeSpan.CompareTo(elapsedTime) > 0)
            {
                string buffer = _port.ReadExisting();

                if (buffer.Length > 0)
                {
                    builder.Append(buffer);
                    lastRead = DateTime.Now;
                }
                elapsedTime = DateTime.Now - lastRead;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Escapes GPIB command string for use with Prologix controller.
        /// CR (13), LF (10), ESC (27), + (43) characters will be escaped.
        /// </summary>
        /// <param name="s">GPIB command</param>
        /// <returns>Escaped string</returns>
        static string EscapeString(string s)
        {
            StringBuilder builder = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == (char)10 || s[i] == (char)13 || s[i] == (char)27 || s[i] == (char)43)
                {
                    builder.Append((char)27); //escape
                }
                builder.Append(s[i]);
                builder.Append('\0'); // Workaround for every-other-character problem.
            }
            return builder.ToString();
        }

        public override void Update(GpibProviderParameters p)
        {
            Timeout = p.Timeout;
        }
    }
}
