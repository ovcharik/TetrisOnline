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

        private Interface.Client _client;
        private Socket _socket;
        private Connection()
        {
            this._client = new Interface.Client();
        }

        public Interface.Client Client
        {
            get { return this._client; }
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

            this._client.Socket = this._socket;
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
            this._client.Dispose();
        }
    }
}