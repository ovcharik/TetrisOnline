using Interface;
using Interface.Json;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;

namespace Server.Models
{
    class User
    {
        public User(Socket socket, Users users, Rooms rooms)
        {
            this._Socket = socket;
            this._Id = this.GetHashCode();
            this._ClientSide = new ClientSide();
            this._ClientSide.Socket = socket;

            this._ClientSide.RaiseSignIn += OnRaiseSignIn;
            this._ClientSide.RaiseSignOut += OnRaiseSignOut;
            this._ClientSide.RaiseSendMsg += OnRaiseSendMsg;
            this._ClientSide.RaiseReceiveStoped += OnRaiseReceiveStoped;
            this._ClientSide.RaiseCreateRoom += OnRaiseCreateRoom;
            this._ClientSide.RaiseEnterRoom += OnRaiseEnterRoom;
            this._ClientSide.RaiseLeaveRoom += OnRaiseLeaveRoom;
            this._ClientSide.RaiseWatchRoom += OnRaiseWatchRoom;
            this._ClientSide.RaiseNotWatchRoom += OnRaiseNotWatchRoom;
            this._ClientSide.RaiseRoomSendMsg += OnRaiseRoomSendMsg;

            this._Name = null;

            this._Users = users;
            this._Rooms = rooms;
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

        private Room _Room = null;
        private Boolean _isRoomWatcher = false;
        private Boolean _isRoomMember = false;

        private Users _Users;
        private Rooms _Rooms;

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

        public void ResetRoom()
        {
            this._Room = null;
            this._isRoomMember = false;
            this._isRoomWatcher = false;
        }

        // Event Handlers
        private void OnRaiseSignIn(object sender, String j)
        {
            Interface.Json.JsonBaseObject json = JsonConvert.DeserializeObject<Interface.Json.JsonBaseObject>(j);
            String n = json.String;
            if (n == null || n.Length == 0) n = _AnonymousName;
            this._Name = n.Split('\n')[0];
            if (this._Users != null) this._Users.SignedIn(this);
            if (this._Rooms != null) this._Rooms.UpdateRoomList(this);
        }

        private void OnRaiseSignOut(object sender, String j)
        {
            this._Socket.Close();
            this._Socket.Dispose();
            if (this._Users != null) this._Users.SignedOut(this);
            if (this._Room != null)
            {
                if (this._isRoomMember) this._Room.Leaved(this);
                if (this._isRoomWatcher) this._Room.NotWatched(this);
                this._Room = null;
            }
        }

        private void OnRaiseSendMsg(object sender, String j)
        {
            JsonMessageObject json = JsonConvert.DeserializeObject<JsonMessageObject>(j);
            this._Users.SendedMsg(this, json);
        }

        private void OnRaiseCreateRoom(object sender, String j)
        {
            if (this._Room == null)
            {
                JsonBaseObject json = JsonConvert.DeserializeObject<JsonBaseObject>(j);
                this._Room = new Room(this._Rooms, this, json.String, json.Int);
                this._Rooms.CreatedRoom(this, this._Room);
                this._Rooms.EnterRoom(this, this._Room.Id);
                this._isRoomMember = true;
                this._isRoomWatcher = false;
            }
        }

        private void OnRaiseEnterRoom(object sender, String j)
        {
            JsonBaseObject json = JsonConvert.DeserializeObject<JsonBaseObject>(j);
            this._Room = this._Rooms.EnterRoom(this, json.Int);
            this._isRoomMember = true;
            this._isRoomWatcher = false;
        }

        private void OnRaiseWatchRoom(object sender, String j)
        {
            JsonBaseObject json = JsonConvert.DeserializeObject<JsonBaseObject>(j);
            this._Room = this._Rooms.WatchRoom(this, json.Int);
            this._isRoomMember = false;
            this._isRoomWatcher = true;
        }

        private void OnRaiseLeaveRoom(object sender, String j)
        {
            if (this._isRoomMember && this._Room != null)
            {
                this._Rooms.LeaveRoom(this, this._Room.Id);
                this._isRoomMember = false;
                this._Room = null;
            }
        }

        private void OnRaiseNotWatchRoom(object sender, String j)
        {
            if (this._isRoomWatcher && this._Room != null)
            {
                this._Rooms.NotWatchRoom(this, this._Room.Id);
                this._isRoomWatcher = false;
                this._Room = null;
            }
        }

        private void OnRaiseRoomSendMsg(object sender, String j)
        {
            if (this._Room != null)
            {
                JsonMessageObject json = JsonConvert.DeserializeObject<JsonMessageObject>(j);
                json.UserId = this.Id;
                json.DateTime = DateTime.Now;
                this._Room.RoomSendedMsg(this, JsonConvert.SerializeObject(json));
            }
        }

        void OnRaiseReceiveStoped(object sender, Exception se)
        {
            OnRaiseSignOut(sender, "");
        }
    }
}