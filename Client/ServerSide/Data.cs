using Interface.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Client.ServerSide
{
    public class Data
    {
        public Data()
        {
            _CurentUser = null;
            _Users = new ObservableCollection<Models.User>();
            _Rooms = new ObservableCollection<Models.Room>();
        }

        // Properties
        private object _Locker = new object();

        private Models.User _CurentUser;
        public Models.User CurentUser { get { return this._CurentUser; } }

        private ObservableCollection<Models.User> _Users;
        public ObservableCollection<Models.User> Users { get { return _Users; } }

        private ObservableCollection<Models.Room> _Rooms;
        public ObservableCollection<Models.Room> Rooms { get { return _Rooms; } }

        public Dispatcher Dispatcher { get; set; }

        // Methods
        private Models.User SearchUserFromId(Int32 id)
        {
            Models.User r = null;
            lock (this._Users)
            {
                foreach (var u in this._Users)
                {
                    if (u.Id == id)
                    {
                        r = u;
                        break;
                    }
                }
            }
            return r;
        }

        private Models.Room SearchRoomFromId(Int32 id)
        {
            Models.Room room = null;
            lock (this._Rooms)
            {
                foreach (var r in this._Rooms)
                {
                    if (r.Id == id)
                    {
                        room = r;
                        break;
                    }
                }
            }
            return room;
        }

        private Models.Room GetRoomFromJsonRoom(JsonRoom jr)
        {
            Models.User creator = SearchUserFromId(jr.CreatorId);
            if (creator == null) return null;

            Models.Room r = new Models.Room(jr.RoomId, creator, jr.Name, jr.Capacity, jr.isGameStarted);

            foreach (var mid in jr.Members)
            {
                Models.User m = SearchUserFromId(mid);
                if (m != null) r.Members.Add(m);
            }

            List<Models.User> ws = new List<Models.User>();
            foreach (var wid in jr.Watchers)
            {
                Models.User w = SearchUserFromId(wid);
                if (w != null) r.Watchers.Add(w);
            }

            return r;
        }

        // Server Event Handlers
        public void OnRaiseUpdateId(object sender, String json)
        {
            lock (this._Locker)
            {
                JsonBaseObject jbo = JsonConvert.DeserializeObject<JsonBaseObject>(json);
                this._CurentUser = new Models.User(jbo.Int, jbo.String, true);
                Dispatcher.Invoke(delegate
                {
                    lock (this._Users)
                    {
                        lock (this._Rooms)
                        {
                            this._Users.Clear();
                            this._Rooms.Clear();
                            this._Users.Add(this._CurentUser);
                        }
                    }
                });
            }
        }

        public void OnRaiseUpdateUserList(object sender, String json)
        {
            lock (this._Locker)
            {
                List<JsonBaseObject> userList = JsonConvert.DeserializeObject<List<JsonBaseObject>>(json);
                Dispatcher.Invoke(delegate
                {
                    foreach (var j in userList)
                    {
                        lock (this._Users)
                        {
                            this._Users.Add(new Models.User(j.Int, j.String));
                        }
                    }
                });
            }
        }

        public void OnRaiseSignedIn(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            Dispatcher.Invoke(delegate
            {
                lock (this._Users)
                {
                    this._Users.Add(new Models.User(user.Int, user.String));
                }
            });
        }

        public void OnRaiseSignedOut(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            Dispatcher.Invoke(delegate
            {
                Models.User u = SearchUserFromId(user.Int);
                lock (this._Users)
                {
                    if (u != null) this.Users.Remove(u);
                }
            });
        }

        public void OnRaiseSendedMsg(object sender, String json)
        {
            JsonMessageObject msg = JsonConvert.DeserializeObject<JsonMessageObject>(json);
            Dispatcher.Invoke(delegate
            {
                Models.User user = SearchUserFromId(msg.UserId);
                if (user == null) return;
                Models.Message m = new Models.Message
                {
                    Data = msg.Data,
                    DateTime = msg.DateTime,
                    Direction = Models.Direction.Input,
                    User = user
                };
                user.AddMessage(m);
                Windows.Messages.Instance.AddUser(user);
            });
        }

        public void OnRaiseUpdateRoomList(object sender, String json)
        {
            lock (this._Locker)
            {
                List<JsonRoom> rooms = JsonConvert.DeserializeObject<List<JsonRoom>>(json);
                Dispatcher.Invoke(delegate
                {
                    foreach (var jr in rooms)
                    {
                        lock (this._Rooms)
                        {
                            Models.Room r = this.GetRoomFromJsonRoom(jr);
                            if (r != null) this._Rooms.Add(r);
                        }
                    }
                });
            }
        }

        public void OnRaiseCreatedRoom(object sender, String json)
        {
            lock (_Locker)
            {
                JsonRoom jr = JsonConvert.DeserializeObject<JsonRoom>(json);
                Dispatcher.Invoke(delegate
                {
                    lock (this._Rooms)
                    {
                        Models.Room r = this.GetRoomFromJsonRoom(jr);
                        if (r != null) this._Rooms.Add(r);
                    }
                });
            }
        }

        public void OnRaiseRemovedRoom(object sender, String json)
        {
            lock (_Locker)
            {
                JsonBaseObject jbo = JsonConvert.DeserializeObject<JsonBaseObject>(json);
                Dispatcher.Invoke(delegate
                {
                    Models.Room r = this.SearchRoomFromId(jbo.Int);
                    lock (this._Rooms)
                    {
                        if (r != null) this._Rooms.Remove(r);
                    }
                });
            }
        }

        public void OnRaiseEnteredRoom(object sender, String json)
        {
            lock (_Locker)
            {
                JsonRoomUpdate jru = JsonConvert.DeserializeObject<JsonRoomUpdate>(json);
                Dispatcher.Invoke(delegate
                {
                    Models.Room r = this.SearchRoomFromId(jru.RoomId);
                    Models.User u = this.SearchUserFromId(jru.UserId);
                    if (r != null && u != null)
                    {
                        r.Entered(u);
                    }
                });
            }
        }

        public void OnRaiseWatchedRoom(object sender, String json)
        {
            lock (_Locker)
            {
                JsonRoomUpdate jru = JsonConvert.DeserializeObject<JsonRoomUpdate>(json);
                Dispatcher.Invoke(delegate
                {
                    Models.Room r = this.SearchRoomFromId(jru.RoomId);
                    Models.User u = this.SearchUserFromId(jru.UserId);
                    if (r != null && u != null)
                    {
                        r.Watched(u);
                    }
                });
            }
        }

        public void OnRaiseLeavedRoom(object sender, String json)
        {
            lock (_Locker)
            {
                JsonRoomUpdate jru = JsonConvert.DeserializeObject<JsonRoomUpdate>(json);
                Dispatcher.Invoke(delegate
                {
                    Models.Room r = this.SearchRoomFromId(jru.RoomId);
                    Models.User u = this.SearchUserFromId(jru.UserId);
                    if (r != null && u != null)
                    {
                        r.Leaved(u);
                    }
                });
            }
        }

        public void OnRaiseNotWatchedRoom(object sender, String json)
        {
            lock (_Locker)
            {
                JsonRoomUpdate jru = JsonConvert.DeserializeObject<JsonRoomUpdate>(json);
                Dispatcher.Invoke(delegate
                {
                    Models.Room r = this.SearchRoomFromId(jru.RoomId);
                    Models.User u = this.SearchUserFromId(jru.UserId);
                    if (r != null && u != null)
                    {
                        r.NotWatched(u);
                    }
                });
            }
        }

        public void OnRaiseGameStartedRoom(object sender, String json)
        {
            lock (_Locker)
            {
                JsonRoomUpdate jru = JsonConvert.DeserializeObject<JsonRoomUpdate>(json);
                Dispatcher.Invoke(delegate
                {
                    Models.Room r = this.SearchRoomFromId(jru.RoomId);
                    if (r != null)
                    {
                        r.GameStarted();
                    }
                });
            }
        }
    }
}
