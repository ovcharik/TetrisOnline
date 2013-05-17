using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class Connection
    {
        private static Connection _instance = null;
        public static Connection Instance
        {
            get
            {
                if (_instance == null) _instance = new Connection();
                return _instance;
            }
        }

        private Data _Data;
        public Data Data { get { return _Data; } }

        private Interface.Client _Client;
        private Socket _socket;

        private Connection()
        {
            this._Client = new Interface.Client();
            this._Data = new Data();

            _Client.RaiseSignedIn += _Data.OnRaiseSignedIn;
            _Client.RaiseSignedOut += _Data.OnRaiseSignedOut;
            _Client.RaiseUpdateUserId += _Data.OnRaiseUpdateId;
            _Client.RaiseUpdateUserList += _Data.OnRaiseUpdateUserList;
        }

        public Interface.Client Client
        {
            get { return this._Client; }
        }

        public void Connect(String host, String port)
        {
            this.Disconnect();
            IPAddress[] ips;
            try
            {
                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ips = Dns.GetHostAddresses(host);
                this._socket.Connect(ips, Convert.ToInt32(port));
            }
            catch (Exception e)
            {
                throw e;
            }

            this._Client.Socket = this._socket;
        }

        public void Disconnect()
        {
            if (this._socket != null && this._socket.Connected)
            {
                this._socket.Close();
                this._socket.Dispose();
            }
        }

        public void Dispose()
        {
            if (this._socket != null)
            {
                this._socket.Close();
                this._socket.Dispose();
            }
            this._Client.Dispose();
        }
    }
}