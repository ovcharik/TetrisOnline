using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.ServerSide
{
    // Singleton
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

        private Connection()
        {
            this._ClientSide = new Interface.ClientSide();
            this._Data = new Data();

            _ClientSide.RaiseSignedIn += _Data.OnRaiseSignedIn;
            _ClientSide.RaiseSignedOut += _Data.OnRaiseSignedOut;
            _ClientSide.RaiseUpdateUserId += _Data.OnRaiseUpdateId;
            _ClientSide.RaiseUpdateUserList += _Data.OnRaiseUpdateUserList;
            _ClientSide.RaiseSendedMsg += _Data.OnRaiseSendedMsg;
        }

        // Properties
        private Data _Data;
        public Data Data { get { return _Data; } }

        private Interface.ClientSide _ClientSide;
        public Interface.ClientSide ClientSide { get { return this._ClientSide; } }

        private Socket _socket;

        // Public Methods
        public void Connect(String host, String port)
        {
            this.Disconnect();
            IPAddress[] ips;

            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ips = Dns.GetHostAddresses(host);
            this._socket.Connect(ips, Convert.ToInt32(port));

            this._ClientSide.Socket = this._socket;
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
            this._ClientSide.Dispose();
        }
    }
}