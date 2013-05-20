using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Interface
{
    public class ClientSide
    {
        public ClientSide(Stream log = null)
        {
            this.Socket = null;
            this._LogStream = log;
        }

        // Properties
        private Thread _ReceivingThread;

        private Stream _LogStream;

        private Socket _socket;
        public Socket Socket 
        {
            get
            {
                return this._socket;
            }
            set
            {
                if (this._socket != value)
                {
                    if (this._ReceivingThread != null) this._ReceivingThread.Abort();
                    this._socket = value;
                    this._ReceivingThread = new Thread(delegate()
                        {
                            try
                            {
                                while (this.Socket.Connected)
                                {
                                    this._MessageReceiver();
                                }
                            }
                            catch (ThreadAbortException) { }
                            catch (Exception se)
                            {
                                if (this.RaiseReceiveStoped != null) this.RaiseReceiveStoped(this, se);
                            }
                        });
                    this._ReceivingThread.Start();
                }
            }
        }

        // Events
        public event EventHandler<Exception> RaiseReceiveStoped;

        public event EventHandler<String> RaiseSignIn;
        public event EventHandler<String> RaiseSignOut;
        public event EventHandler<String> RaiseSendMsg;
        public event EventHandler<String> RaiseCreateRoom;
        public event EventHandler<String> RaiseEnterRoom;
        public event EventHandler<String> RaiseWatchRoom;
        public event EventHandler<String> RaiseLeaveRoom;
        public event EventHandler<String> RaiseNotWatchRoom;
        public event EventHandler<String> RaiseRoomSendMsg;

        public event EventHandler<String> RaiseSignedIn;
        public event EventHandler<String> RaiseSignedOut;
        public event EventHandler<String> RaiseUpdateUserId;
        public event EventHandler<String> RaiseUpdateUserList;
        public event EventHandler<String> RaiseSendedMsg;
        public event EventHandler<String> RaiseUpdateRoomList;
        public event EventHandler<String> RaiseCreatedRoom;
        public event EventHandler<String> RaiseRemovedRoom;
        public event EventHandler<String> RaiseEnteredRoom;
        public event EventHandler<String> RaiseLeavedRoom;
        public event EventHandler<String> RaiseWatchedRoom;
        public event EventHandler<String> RaiseNotWatchedRoom;
        public event EventHandler<String> RaiseGameStartedRoom;

        // Public Methods
        public void Send(Int32 e, String j)
        {
            byte[] json = Encoding.UTF8.GetBytes(j);
            byte[] size = BitConverter.GetBytes(json.Length);
            byte[] even = BitConverter.GetBytes(e);

            Stream s = new MemoryStream();
            s.Write(even, 0, even.Length);
            s.Write(size, 0, size.Length);
            s.Write(json, 0, json.Length);

            byte[] data = new byte[s.Length];
            s.Position = 0;
            s.Read(data, 0, data.Length);

            this.Socket.Send(data);
            Log.Send(_LogStream, e, j);
        }
        public void Dispose()
        {
            try
            {
                this._ReceivingThread.Abort();
            }
            catch { }
        }

        // Private Methods
        private void _MessageReceiver()
        {
            Int32 e;
            String j;
            try
            {
                Int32 s;
                byte[] buffer;

                buffer = new byte[4];
                this.Socket.Receive(buffer, 4, SocketFlags.Partial);
                e = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[4];
                this.Socket.Receive(buffer, 4, SocketFlags.Partial);
                s = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[s];
                this.Socket.Receive(buffer, s, SocketFlags.Partial);
                j = Encoding.UTF8.GetString(buffer);

                Log.Recieved(_LogStream, e, j);
            }
            catch (Exception se)
            {
                throw se;
            }

            //Thread th = new Thread(delegate()
            //{
                this._EventRoute(e, j);
            //});
            //Thread.Sleep(10);
            //th.Start();
        }

        private void _EventRoute(Int32 e, String j)
        {
            switch (e)
            {
                    // from client
                case Events.SIGN_IN:
                    if (RaiseSignIn != null) RaiseSignIn(this, j);
                    break;
                case Events.SIGN_OUT:
                    if (RaiseSignOut != null) RaiseSignOut(this, j);
                    break;
                case Events.SEND_MSG:
                    if (RaiseSendMsg != null) RaiseSendMsg(this, j);
                    break;
                case Events.CREATE_ROOM:
                    if (RaiseCreateRoom != null) RaiseCreateRoom(this, j);
                    break;
                case Events.ENTER_ROOM:
                    if (RaiseEnterRoom != null) RaiseEnterRoom(this, j);
                    break;
                case Events.LEAVE_ROOM:
                    if (RaiseLeaveRoom != null) RaiseLeaveRoom(this, j);
                    break;
                case Events.WATCH_ROOM:
                    if (RaiseWatchRoom != null) RaiseWatchRoom(this, j);
                    break;
                case Events.NOTWATCH_ROOM:
                    if (RaiseNotWatchRoom != null) RaiseNotWatchRoom(this, j);
                    break;
                case Events.ROOM_SEND_MSG:
                    if (RaiseRoomSendMsg != null) RaiseRoomSendMsg(this, j);
                    break;

                    // from server
                case Events.SIGNED_IN:
                    if (RaiseSignedIn != null) RaiseSignedIn(this, j);
                    break;
                case Events.SIGNED_OUT:
                    if (RaiseSignedOut != null) RaiseSignedOut(this, j);
                    break;
                case Events.UPDATE_ID:
                    if (RaiseUpdateUserId != null) RaiseUpdateUserId(this, j);
                    break;
                case Events.UPDATE_USER_LIST:
                    if (RaiseUpdateUserList != null) RaiseUpdateUserList(this, j);
                    break;
                case Events.SENDED_MSG:
                    if (RaiseSendedMsg != null) RaiseSendedMsg(this, j);
                    break;
                case Events.UPDATE_ROOM_LIST:
                    if (RaiseUpdateRoomList != null) RaiseUpdateRoomList(this, j);
                    break;
                case Events.CREATED_ROOM:
                    if (RaiseCreatedRoom != null) RaiseCreatedRoom(this, j);
                    break;
                case Events.REMOVED_ROOM:
                    if (RaiseRemovedRoom != null) RaiseRemovedRoom(this, j);
                    break;
                case Events.ENTERED_ROOM:
                    if (RaiseEnteredRoom != null) RaiseEnteredRoom(this, j);
                    break;
                case Events.LEAVED_ROOM:
                    if (RaiseLeavedRoom != null) RaiseLeavedRoom(this, j);
                    break;
                case Events.WATCHED_ROOM:
                    if (RaiseWatchedRoom != null) RaiseWatchedRoom(this, j);
                    break;
                case Events.NOTWATCHED_ROOM:
                    if (RaiseNotWatchedRoom != null) RaiseNotWatchedRoom(this, j);
                    break;
                case Events.GAME_STARTED_ROOM:
                    if (RaiseGameStartedRoom != null) RaiseGameStartedRoom(this, j);
                    break;
            }
        }

    }
}
