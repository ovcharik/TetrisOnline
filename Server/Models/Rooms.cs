using System;
using System.Collections.Generic;
using Interface;
using Interface.Json;
using Newtonsoft.Json;

namespace Server.Models
{
    class Rooms
    {
        public Rooms(Users users)
        {
            this._Users = users;
            this._Rooms = new Dictionary<int, Room>();
        }

        // Properties
        private Dictionary<Int32, Room> _Rooms;
        private Users _Users;

        // Public Methods
        public void CreatedRoom(User user, Room room)
        {
            this._Rooms.Add(room.Id, room);
            String j = JsonConvert.SerializeObject(room.toJsonRoom());
            this._Users.Broadcast(Events.CREATED_ROOM, j);
        }

        public void ClosedRoom(Room room)
        {
            String j = JsonConvert.SerializeObject(room.toJsonRoom());
            this._Users.Broadcast(Events.CLOSED_ROOM, j);
        }

        public void RemovedRoom(Room room)
        {
            JsonBaseObject j = new JsonBaseObject
            {
                Int = room.Id
            };
            this._Users.Broadcast(Events.REMOVED_ROOM, JsonConvert.SerializeObject(j));
            _Rooms.Remove(room.Id);
        }

        public void UpdateRoomList(User user)
        {
            if (user != null)
            {
                List<JsonRoom> lr = new List<JsonRoom>();
                lock (this._Rooms)
                {
                    foreach (var r in this._Rooms.Values)
                    {
                        lr.Add(r.toJsonRoom());
                    }
                }
                user.SendMessage(Events.UPDATE_ROOM_LIST, JsonConvert.SerializeObject(lr));
            }
        }

        public Room EnterRoom(User user, Int32 roomId)
        {
            Room r = null;
            try
            {
                r = this._Rooms[roomId];
                r.Entered(user);
                JsonRoomUpdate jru = new JsonRoomUpdate
                {
                    UserId = user.Id,
                    RoomId = roomId
                };
                this._Users.Broadcast(Events.ENTERED_ROOM, JsonConvert.SerializeObject(jru));
                if (r.isStarted)
                {
                    this.GameStartedRoom(r);
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write("# Error: {0}", ex.Message);
            }
            return r;
        }

        public Room WatchRoom(User user, Int32 roomId)
        {
            Room r = null;
            try
            {
                r = this._Rooms[roomId];
                r.Watched(user);
                JsonRoomUpdate jru = new JsonRoomUpdate
                {
                    UserId = user.Id,
                    RoomId = roomId
                };
                this._Users.Broadcast(Events.WATCHED_ROOM, JsonConvert.SerializeObject(jru));
            }
            catch (Exception ex)
            {
                Console.Error.Write("# Error: {0}", ex.Message);
            }
            return r;
        }

        public Room LeaveRoom(User user, Int32 roomId)
        {
            Room r = null;
            try
            {
                r = this._Rooms[roomId];
                r.Leaved(user);
                JsonRoomUpdate jru = new JsonRoomUpdate
                {
                    UserId = user.Id,
                    RoomId = roomId
                };
                this._Users.Broadcast(Events.LEAVED_ROOM, JsonConvert.SerializeObject(jru));
            }
            catch (Exception ex)
            {
                Console.Error.Write("# Error: {0}", ex.Message);
            }
            return r;
        }

        public Room NotWatchRoom(User user, Int32 roomId)
        {
            Room r = null;
            try
            {
                r = this._Rooms[roomId];
                r.NotWatched(user);
                JsonRoomUpdate jru = new JsonRoomUpdate
                {
                    UserId = user.Id,
                    RoomId = roomId
                };
                this._Users.Broadcast(Events.NOTWATCHED_ROOM, JsonConvert.SerializeObject(jru));
            }
            catch (Exception ex)
            {
                Console.Error.Write("# Error: {0}", ex.Message);
            }
            return r;
        }

        public void GameStartedRoom(Room room)
        {
            try
            {
                JsonRoomUpdate jru = new JsonRoomUpdate
                {
                    RoomId = room.Id
                };
                this._Users.Broadcast(Events.GAME_STARTED_ROOM, JsonConvert.SerializeObject(jru));
            }
            catch (Exception ex)
            {
                Console.Error.Write("# Error: {0}", ex.Message);
            }
        }
    }
}
