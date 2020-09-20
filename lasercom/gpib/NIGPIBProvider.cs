using LuiHardware.objects;
using NationalInstruments.NI4882;
using System;

namespace LuiHardware.gpib
{
    /// <summary>
    ///     Provide GPIB using NI 488.2 PCI card.
    /// </summary>
    public class NIGpibProvider : AbstractGpibProvider
    {
        const string GpibTerminator = "\r\n";

        public NIGpibProvider(LuiObjectParameters p) : this(p as GpibProviderParameters)
        {
        }

        public NIGpibProvider(GpibProviderParameters p)
        {
            if (p == null)
                throw new ArgumentException("GpibProviderParameters null");
            Init(p.BoardNumber);
        }

        public Board Board { get; set; }

        void Init(int _BoardNumber)
        {
            Board = new Board(_BoardNumber);
            Board.SendInterfaceClear();
            Board.BecomeActiveController(true);
            Board.SetRemoteEnableLine();
            Board.SetEndOnWrite = true; // Assert EOI on write.
            Board.SetEndOnEndOfString = false;
            Board.IOTimeout = TimeoutValue.T100ms;
            Board.EndOfStringCharacter = 13; // CR
            Board.TerminateReadOnEndOfString = true;
            //AddressCollection Addresses = Board.FindListeners();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Board.Dispose();
        }

        public override void LoggedWrite(byte address, string command)
        {
            Log.Debug("GPIB Command: " + command);
            try
            {
                Board.Write(new Address(address), command + GpibTerminator);
            }
            catch (GpibException e)
            {
                Log.Error(e.Message);
            }
            catch (InvalidOperationException e)
            {
                Log.Error(e.InnerException.Message);
            }
        }

        public override string LoggedQuery(byte address, string command)
        {
            Log.Debug("GPIB Command: " + command);
            string response = null;
            try
            {
                Board.Write(new Address(address), command + GpibTerminator);
                response = Board.ReadString(new Address(address));
                response = response.TrimEnd(GpibTerminator.ToCharArray());
            }
            catch (GpibException ex)
            {
                Log.Error(ex);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex);
            }

            Log.Debug("GPIB Response: " + response);
            return response;
        }
    }
}