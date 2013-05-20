using Interface;
using Interface.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Server.Models
{
    class Room
    {
        public const int MaxCapacity = 5;
        public const int DefaultCapacity = 3;

        public const String DefaultName = "Room";

        public Room(Rooms rooms, User creator, String name = DefaultName, int capacity = DefaultCapacity)
        {
            this._Watchers = new Dictionary<Int32, User>();
            this._Members = new Dictionary<Int32, User>();
            this._Creator = creator;
            this._Id = this.GetHashCode();
            this._Capacity = (capacity <= MaxCapacity && capacity > 0) ? capacity : DefaultCapacity;
            this._Name = name.Split('\n')[0];
            this._isStarted = false;
            this._Rooms = rooms;
        }

        // Propeties
        private Int32 _Id;
        public Int32 Id { get { return _Id; } }

        private String _Name;
        public String Name { get { return _Name; } }

        private Dictionary<Int32, User> _Members;
        public Dictionary<Int32, User> Members { get { return _Members; } }

        private Dictionary<Int32, User> _Watchers;
        public Dictionary<Int32, User> Watchers { get { return _Watchers; } }

        private int _Capacity;
        public int Capacity { get { return _Capacity; } }

        private bool _isStarted;
        public bool isStarted { get { return this._isStarted; } }

        private User _Creator;
        public User Creator { get { return this._Creator; } }

        private Rooms _Rooms;

        private Boolean _isEmpty
        {
            get { return (this._Members.Count <= 0 && this._Watchers.Count == 0); }
        }

        // Private Methods
        private void Broadcast(Int32 e, String j, User self = null)
        {
            Console.WriteLine("Room {0} Broadcast: {1} - {2}", this.Id, Events.EventToString(e), j);
            lock (this._Members)
            {
                foreach (var u in this._Members.Values)
                {
                    if (u != self)
                        u.SendMessage(e, j);
                }
            }
            lock (this._Watchers)
            {
                foreach (var u in this._Watchers.Values)
                {
                    if (u != self)
                        u.SendMessage(e, j);
                }
            }
        }

        // Public Methods
        public JsonRoom toJsonRoom()
        {
            JsonRoom r = new JsonRoom
            {
                RoomId = this.Id,
                Capacity = this.Capacity,
                Name = this.Name,
                CreatorId = this.Creator.Id,
                Members = new List<Int32>(),
                Watchers = new List<Int32>(),
                isGameStarted = this.isStarted
            };
            lock (this._Members)
            {
                foreach (var u in this.Members.Keys)
                {
                    r.Members.Add(u);
                }
            }
            lock (this._Watchers)
            {
                foreach (var u in this.Watchers.Keys)
                {
                    r.Watchers.Add(u);
                }
            }
            return r;
        }

        public void Watched(User user)
        {
            if (user != null)
            {
                lock (this._Watchers)
                {
                    this._Watchers.Add(user.Id, user);
                }
                JsonBaseObject jbo = new JsonBaseObject
                {
                    Int = user.Id
                };
                String j = JsonConvert.SerializeObject(jbo);
                // this.Broadcast(Events.WATCHED_ROOM, j, user);
            }
        }

        public void NotWatched(User user)
        {
            try
            {
                this._Watchers.Remove(user.Id);
                JsonBaseObject jbo = new JsonBaseObject
                {
                    Int = user.Id
                };
                String j = JsonConvert.SerializeObject(jbo);
                // this.Broadcast(Events.NOTWATCHED_ROOM, j);
                if (this._isEmpty) this._Rooms.RemovedRoom(this);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("# Error: {0}", e.Message);
            }
        }

        public void Entered(User user)
        {
            lock(this._Members)
            {
                if (user != null && !this.isStarted && this._Members.Count < this.Capacity)
                {
                    this._Members.Add(user.Id, user);
                    JsonBaseObject jbo = new JsonBaseObject
                    {
                        Int = user.Id
                    };
                    String j = JsonConvert.SerializeObject(jbo);
                    // this.Broadcast(Events.ENTER_ROOM, j);
                    if (this._Members.Count == this.Capacity)
                    {
                        this._isStarted = true;
                        //this._Rooms.GameStartedRoom(this);
                    }
                }
            }
        }

        public void Leaved(User user)
        {
            try
            {
                this._Members.Remove(user.Id);
                JsonBaseObject jbo = new JsonBaseObject
                {
                    Int = user.Id
                };
                String j = JsonConvert.SerializeObject(jbo);
                // this.Broadcast(Events.LEAVED_ROOM, j);
                if (this._isEmpty) this._Rooms.RemovedRoom(this);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("# Error: {0}", e.Message);
            }
        }

        public void RoomSendedMsg(User user, String json)
        {
            this.Broadcast(Events.ROOM_SENDED_MSG, json, user);
        }

        public void Lose(User user)
        {
        }

        public void Win(User user)
        {
        }

        public void CreatedFigure(User user, Interface.Json.Figure.Type type)
        {
        }

        public void MovedFigure(User user, Interface.Json.Figure.MoveAction act)
        {
        }
    }
}
