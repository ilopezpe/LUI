using LuiHardware.objects;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace LuiHardware.gpib
{
    /// <summary>
    ///     Provide GPIB using Prologix USB GPIB controller.
    /// </summary>
    public class PrologixGpibProvider : AbstractGpibProvider
    {
        const int DefaultTimeout = 500;

        SerialPort _port;

        public PrologixGpibProvider(string PortName)
        {
            Init(PortName, DefaultTimeout);
        }

        public PrologixGpibProvider(LuiObjectParameters p) : this(p as GpibProviderParameters)
        {
        }

        public PrologixGpibProvider(GpibProviderParameters p)
        {
            if (p == null || p.PortName == null) throw new ArgumentException("PortName must be defined.");

            Init(p.PortName, p.Timeout);
        }

        public string PortName => _port.PortName;

        int Timeout { get; set; }

        void Init(string PortName, int Timeout)
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

            #endregion Serial port configuration

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
            var data = PrologixInitiator + command + "\r\n";
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
            var arglist = string.Join(" ", args);
            var data = PrologixInitiator + command + " " + arglist + "\r\n";
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

        public override void LoggedWrite(byte address, string command)
        {
            Log.Debug("GPIB Command: " + command);
            var TX = EscapeString(command) + "\r\n"; // send to instrument
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

        public override string LoggedQuery(byte address, string command)
        {
            Log.Debug("GPIB Command: " + command);
            var TX = EscapeString(command) + "\r\n"; // send to instrument
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
        ///     Read from addressed device until timeout reached between read characters.
        ///     Note GPIB data is stored in 1 character buffer, then sent to 4K USB buffer.
        ///     Thus there are two effective timeout required, one for reading GPIB and one
        ///     for reading from the serial port (USB buffer).
        /// </summary>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        string ReadWithTimeout(int Timeout)
        {
            if (!_port.IsOpen) throw new InvalidOperationException("The specified port is not open");

            ControllerCommand(ReadTimeoutCommand, (Timeout + 1).ToString()); // Allow 1 ms for GPIB -> USB.
            ControllerCommand(PrologixRead, "eoi"); // Read from GPIB until eoi or timeout.

            var builder = new StringBuilder();
            var lastRead = DateTime.Now;
            var elapsedTime = new TimeSpan();

            // 2 second timespan
            var TimeSpan = new TimeSpan(0, 0, 0, 0, Timeout);

            // Read from port until TIMEOUT time has elapsed since
            // last successful read
            while (TimeSpan.CompareTo(elapsedTime) > 0)
            {
                var buffer = _port.ReadExisting();

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
        ///     Escapes GPIB command string for use with Prologix controller.
        ///     CR (13), LF (10), ESC (27), + (43) characters will be escaped.
        /// </summary>
        /// <param name="s">GPIB command</param>
        /// <returns>Escaped string</returns>
        static string EscapeString(string s)
        {
            var builder = new StringBuilder(s.Length);
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == (char)10 || s[i] == (char)13 || s[i] == (char)27 || s[i] == (char)43)
                    builder.Append((char)27); //escape
                builder.Append(s[i]);
                builder.Append('\0'); // Workaround for every-other-character problem.
            }

            return builder.ToString();
        }

        public override void Update(GpibProviderParameters p)
        {
            Timeout = p.Timeout;
        }

        #region Constants

        const string PrologixInitiator = "++";
        const string PrologixAddress = "addr";
        const string PrologixIFC = "ifc";
        const string PrologixMode = "mode 1";
        const string PrologixEOI = "eoi 1";
        const string PrologixEOS = "eos 0"; // 0 = CRLF termination
        const string PrologixRead = "read";
        const string ReadTimeoutCommand = "read_tmo_ms";

        #endregion Constants
    }
}