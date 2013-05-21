using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
#if WITH_LOGS
using System.IO;
#endif

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
#if WITH_LOGS
            String logname = "var-" + DateTime.Now.ToShortDateString() + ".log";
            _LogStream = new FileStream(logname, FileMode.Append);
            this._ClientSide = new Interface.ClientSide(_LogStream);
#else
            this._ClientSide = new Interface.ClientSide();
#endif
            this._Data = new Data();

            _ClientSide.RaiseSignedIn += _Data.OnRaiseSignedIn;
            _ClientSide.RaiseSignedOut += _Data.OnRaiseSignedOut;
            _ClientSide.RaiseUpdateUserId += _Data.OnRaiseUpdateId;
            _ClientSide.RaiseUpdateUserList += _Data.OnRaiseUpdateUserList;
            _ClientSide.RaiseSendedMsg += _Data.OnRaiseSendedMsg;
            _ClientSide.RaiseUpdateRoomList += _Data.OnRaiseUpdateRoomList;
            _ClientSide.RaiseCreatedRoom += _Data.OnRaiseCreatedRoom;
            _ClientSide.RaiseRemovedRoom += _Data.OnRaiseRemovedRoom;
            _ClientSide.RaiseEnteredRoom += _Data.OnRaiseEnteredRoom;
            _ClientSide.RaiseLeavedRoom += _Data.OnRaiseLeavedRoom;
            _ClientSide.RaiseWatchedRoom += _Data.OnRaiseWatchedRoom;
            _ClientSide.RaiseNotWatchedRoom += _Data.OnRaiseNotWatchedRoom;
            _ClientSide.RaiseGameStartedRoom += _Data.OnRaiseGameStartedRoom;
            _ClientSide.RaiseSendedMsgRoom += _Data.OnRaiseSendedMsgRoom;
        }

        // Properties
#if WITH_LOGS
        private FileStream _LogStream;
#endif
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
#if WITH_LOGS
            _LogStream.Close();
            _LogStream.Dispose();
#endif
        }
    }
}