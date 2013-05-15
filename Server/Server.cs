using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;

using Server.Models;
using Interface;
using Interface.Json;

namespace Server
{
    class Server
    {
        private Users _users;

        private List<Thread> _threads;

        private IPAddress _address;
        private int _port;

        private Boolean _isServerRunning;
        public Boolean isServerRunning
        {
            get { return this._isServerRunning; }
        }
        private Socket _listener;

        public Server(IPAddress address, int port)
        {
            this._users = new Users();
            this._users.Server = this;

            this._threads = new List<Thread>();

            this._port = port;
            this._address = address;

            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._listener.Bind(new IPEndPoint(this._address, this._port));
        }

        public void Start()
        {
            this._listener.Listen(50);
            this._isServerRunning = true;
            this.SocketAccepter().Start();
            Console.WriteLine("Server started");
        }

        public void Stop()
        {
            if (this.isServerRunning)
            {
                this._listener.Close();
                this._isServerRunning = false;
            }
        }

        private Thread SocketAccepter()
        {
            Thread th = new Thread(delegate()
                {
                    while (this.isServerRunning)
                    {
                        Socket sock = this._listener.Accept();
                        User user = new User(sock);
                        this._users.Add(user);
                        Thread.Sleep(1);
                        Console.WriteLine("Client {0} accepted, ID: {1}", sock.LocalEndPoint.ToString(), user.Id);
                    }
                });
            return th;
        }
    }
}
