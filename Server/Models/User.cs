using Interface;
using Interface.Json;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;

namespace Server.Models
{
    class User
    {
        public User(Socket socket)
        {
            this._Socket = socket;
            this._Id = this.GetHashCode();
            this._ClientSide = new ClientSide();
            this._ClientSide.Socket = socket;

            this._ClientSide.RaiseSignIn += OnRaiseSignIn;
            this._ClientSide.RaiseSignOut += OnRaiseSignOut;
            this._ClientSide.RaiseSendMsg += OnRaiseSendMsg;
            this._ClientSide.RaiseReceiveStoped += OnRaiseReceiveStoped;

            this._Name = null;
        }

        // Properties
        private const string _AnonymousName = "Anonymous";

        private String _Name;
        public String Name
        {
            get { return this._Name; }
        }

        private ClientSide _ClientSide;
        public ClientSide ClientSide
        {
            get { return this._ClientSide; }
        }

        private Int32 _Id;
        public Int32 Id
        { 
            get { return this._Id; }
        }

        private Socket _Socket;
        public Socket Socket
        {
            get { return this._Socket; }
        }

        public Users Users { get; set; }

        // Public Methods
        public void SendMessage(Int32 e, String j)
        {
            try
            {
                this._ClientSide.Send(e, j);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("# Error: {0}", ex.Message);
            }
        }

        // Event Handlers
        private void OnRaiseSignIn(object sender, String j)
        {
            Interface.Json.JsonBaseObject json = JsonConvert.DeserializeObject<Interface.Json.JsonBaseObject>(j);
            String n = json.String;
            if (n == null || n.Length == 0) n = _AnonymousName;
            this._Name = n;
            if (this.Users != null) this.Users.SignedIn(this);
        }

        private void OnRaiseSignOut(object sender, String j)
        {
            this._Socket.Close();
            this._Socket.Dispose();
            if (this.Users != null) this.Users.SignedOut(this);
        }

        private void OnRaiseSendMsg(object sender, String j)
        {
            JsonMessageObject json = JsonConvert.DeserializeObject<JsonMessageObject>(j);
            this.Users.SendedMsg(this, json);
        }

        void OnRaiseReceiveStoped(object sender, Exception se)
        {
            OnRaiseSignOut(sender, "");
        }
    }
}
