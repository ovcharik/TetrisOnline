using Server.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Server
    {
        public Server(IPAddress address, int port)
        {
            this._Users = new Users(this);

            this._UserThreads = new List<Thread>();

            this._Port = port;
            this._IPAddress = address;

            this._Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._Listener.Bind(new IPEndPoint(this._IPAddress, this._Port));
        }

        // Properties
        private Users _Users;

        private List<Thread> _UserThreads;

        private IPAddress _IPAddress;
        private int _Port;

        private Boolean _isServerRunning;
        public Boolean isServerRunning { get { return this._isServerRunning; } }

        private Socket _Listener;

        // Public Methods
        public void Start()
        {
            this._Listener.Listen(50);
            this._isServerRunning = true;
            this.SocketAccepter().Start();
            Console.WriteLine("Server started");
        }

        public void Stop()
        {
            if (this.isServerRunning)
            {
                this._Listener.Close();
                this._isServerRunning = false;
            }
        }

        // Private Methods
        private Thread SocketAccepter()
        {
            Thread th = new Thread(delegate()
                {
                    while (this.isServerRunning)
                    {
                        Socket sock = this._Listener.Accept();
                        User user = new User(sock);
                        this._Users.Add(user);
                        Thread.Sleep(1);
                        Console.WriteLine("Client {0} accepted, ID: {1}", sock.LocalEndPoint.ToString(), user.Id);
                    }
                });
            return th;
        }
    }
}
