using System;
using System.Net;
using System.Net.Sockets;

using Newtonsoft;
using Newtonsoft.Json;

using Interface;

namespace Server.Models
{
    class User
    {
        private const string Anonymous = "Anonymous";

        private String _name;
        public String Name
        {
            get { return this._name; }
        }

        private Client _client;
        public Client Client
        {
            get { return this._client; }
        }

        private Int32 _id;
        public Int32 Id
        { 
            get { return this._id; }
        }

        private Socket _socket;
        public Socket Socket
        {
            get { return this._socket; }
        }

        public Users Users { get; set; }

        public User(Socket socket)
        {
            this._socket = socket;
            this._id = this.GetHashCode();
            this._client = new Client();
            this._client.Socket = socket;

            this._client.RaiseSignIn += SignIn;
            this._client.RaiseSignOut += SignOut;
            this._client.RaiseSendMsg += SendMessage;
            this._client.RaiseReceiveStoped += ReceiveStoped;
        }

        public void SendMessage(Int32 e, String j)
        {
            this._client.Send(e, j);
        }

        void SignIn(object sender, String j)
        {
            Interface.Json.JsonBaseObject json = JsonConvert.DeserializeObject<Interface.Json.JsonBaseObject>(j);
            String n = json.String;
            if (n == null || n.Length == 0) n = Anonymous;
            this._name = n;
            if (this.Users != null) this.Users.SignedIn(this);
        }

        void SignOut(object sender, String j)
        {
            this._socket.Close();
            this._socket.Dispose();
            if (this.Users != null) this.Users.SignedOut(this);
        }

        void SendMessage(object sender, String j)
        {
            Interface.Json.JsonMessageObject json = JsonConvert.DeserializeObject<Interface.Json.JsonMessageObject>(j);
            this.Users.SendedMsg(this, json);
        }

        void ReceiveStoped(object sender, Exception se)
        {
            SignOut(sender, "");
        }

    }
}
