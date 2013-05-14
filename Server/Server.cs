using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;

namespace Server
{
    class Server
    {
        private Dictionary<int, Room>  _rooms;
        private Dictionary<int, Client> _clients;

        private List<Json.BaseObject> _users;

        private List<Thread> _threads;

        private IPAddress _address;
        private int _port;
        private IPEndPoint _point;
        private Boolean _isServerRunning;
        public Boolean isServerRunning
        {
            get { return this._isServerRunning; }
        }
        private Socket _listener;

        public Server(IPAddress address, int port)
        {
            this._rooms = new Dictionary<int, Room>();
            this._clients = new Dictionary<int, Client>();

            this._users = new List<Json.BaseObject>();

            this._threads = new List<Thread>();

            this._port = port;
            this._address = address;

            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._point = new IPEndPoint(this._address, this._port);
            this._listener.Bind(this._point);
        }

        public void Start()
        {
            this._listener.Listen(50);
            this._isServerRunning = true;
            this.SocketAccepter();
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

        private void SocketAccepter()
        {
            Thread th = new Thread(delegate()
                {
                    while (this.isServerRunning)
                    {
                        Socket sock = this._listener.Accept();
                        Client clnt = new Client(sock, this);
                        this._clients.Add(clnt.GetHashCode(), clnt);
                        clnt.Start();
                        Thread.Sleep(1);
                        Console.WriteLine("Client {0} accepted, ID: {1}", sock.LocalEndPoint.ToString(), clnt.GetHashCode());
                    }
                });
            th.Start();
        }

        private void Broadcast(Int32 e, String json, Client client = null)
        {
            Console.WriteLine("Broadcast: {0} - {1}", Events.EventToString(e), json);
            foreach (Client clnt in this._clients.Values)
            {
                if (client != clnt) clnt.Send(e, json);
            }
        }

        // Events

        public void SignedOut(Client clnt)
        {
            Int32 id = clnt.GetHashCode();
            this._clients.Remove(id);

            // Какой-то говнокод получился
            this._users.Remove(this._users.Find(delegate(Json.BaseObject u) {return u.Int == id;}));

            if (clnt.name != null)
            {
                Json.BaseObject json = new Json.BaseObject();
                json.Int = clnt.GetHashCode();

                String s = JsonConvert.SerializeObject(json);
                this.Broadcast(Events.SIGNED_OUT, s);
            }
            Console.Write("Client {0} dissconected", clnt.GetHashCode());
        }

        public void SignedIn(Client clnt, string name)
        {
            Json.BaseObject json = new Json.BaseObject();
            json.Int = clnt.GetHashCode();
            json.String = name;

            String s = JsonConvert.SerializeObject(json);
            
            this.Broadcast(Events.SIGNED_IN, s, clnt);
            clnt.Send(Events.UPDATE_ID, s);
            clnt.Send(Events.UPDATE_USER_LIST, JsonConvert.SerializeObject(this._users));

            this._users.Add(json);
        }
    }
}
