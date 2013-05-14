using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Server
{
    class Client
    {
        private string _name;
        public string name
        {
            get { return this._name; }
        }

        private Socket _socket;
        public Socket socket
        {
            get { return this._socket; }
        }
        private Server _server;

        public Client(Socket socket, Server server)
        {
            this._socket = socket;
            this._server = server;
        }

        public void Start()
        {
            Thread th = new Thread(delegate()
                {
                    while (this._server.isServerRunning && this._socket.Connected)
                    {
                        this.MessageReceiver();
                    }
                    this.SignOut();
                });
            th.Start();
        }

        private void MessageReceiver()
        {
            try
            {
                Int32 e;
                Int32 s;
                String j;

                byte[] buffer = new byte[4];
                this._socket.Receive(buffer, 4, SocketFlags.Partial);
                e = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[4];
                this._socket.Receive(buffer, 4, SocketFlags.Partial);
                s = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[s];
                this._socket.Receive(buffer, s, SocketFlags.Partial);
                j = Encoding.UTF8.GetString(buffer);
                
                Thread th = new Thread(delegate()
                    {
                        this.EventRoute(e, j);
                    });
                th.Start();
            }
            catch { }
        }

        public void Send(Int32 e, string j)
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

                this._socket.Send(data);
            }
            catch { }
        }

        // Events

        private void EventRoute(Int32 e, String j)
        {
            switch (e)
            {
                case Events.SIGN_IN:
                    this.SignIn(j);
                    break;
                case Events.SIGN_OUT:
                    this.SignOut();
                    break;
            }
        }

        private void SignOut()
        {
            this._socket.Close();
            this._server.SignedOut(this);
        }

        private void SignIn(string name)
        {
            Json.BaseObject j = JsonConvert.DeserializeObject<Json.BaseObject>(name);
            String n = j.String;
            if (n == null || n.Length == 0) n = "Anonymous";
            this._name = n;
            this._server.SignedIn(this, n);
        }
    }
}
