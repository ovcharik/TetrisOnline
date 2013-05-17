using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Interface
{
    public class Client
    {
        private Thread receiveThread;
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
                    if (this.receiveThread != null) this.receiveThread.Abort();
                    this._socket = value;
                    this.receiveThread = new Thread(delegate()
                        {
                            try
                            {
                                while (this.Socket.Connected)
                                {
                                    this.MessageReceiver();
                                }
                            }
                            catch (ThreadAbortException) { }
                            catch (Exception se)
                            {
                                if (this.RaiseReceiveStoped != null) this.RaiseReceiveStoped(this, se);
                            }
                        });
                    this.receiveThread.Start();
                }
            }
        }

        public Client()
        {
            this.Socket = null;
        }


        public void Send(Int32 e, String j)
        {
            try
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
            }
            catch (Exception se)
            {
                throw se;
            }
        }

        public void Dispose()
        {
            try
            {
                this.receiveThread.Abort();
            }
            catch { }
        }

        private void MessageReceiver()
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
            }
            catch (Exception se)
            {
                throw se;
            }

            Thread th = new Thread(delegate()
            {
                this.EventRoute(e, j);
            });
            th.Start();
        }

        // Events
        public event EventHandler<Exception> RaiseReceiveStoped;
        public event EventHandler<String> RaiseSignIn;
        public event EventHandler<String> RaiseSignOut;
        public event EventHandler<String> RaiseSendMsg;

        public event EventHandler<String> RaiseSignedIn;
        public event EventHandler<String> RaiseSignedOut;
        public event EventHandler<String> RaiseUpdateUserId;
        public event EventHandler<String> RaiseUpdateUserList;
        public event EventHandler<String> RaiseSendedMsg;

        private void EventRoute(Int32 e, String j)
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
            }
        }

    }
}
