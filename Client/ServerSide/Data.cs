using Interface.Json;
using Newtonsoft.Json;
using System;
using System.Linq;
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

        private Models.Room _CurrentRoom;
        public Models.Room CurrentRoom { get { return this._CurrentRoom; } }

        private Models.Role _CurrentRole;
        public Models.Role CurrentRole { get { return this._CurrentRole; } }

        private ObservableCollection<Models.User> _Users;
        public ObservableCollection<Models.User> Users { get { return _Users; } }

        private ObservableCollection<Models.Room> _Rooms;
        public ObservableCollection<Models.Room> Rooms { get { return _Rooms; } }

        public Dispatcher Dispatcher { get; set; }

        private Models.Room GetRoomFromJsonRoom(JsonRoom jr)
        {
            Models.User creator = _Users.FirstOrDefault(u => u.Id == jr.CreatorId);
            if (creator == null) return null;

            Models.Room r = new Models.Room(jr.RoomId, creator, jr.Name, jr.Capacity, jr.isGameStarted);

            foreach (var mid in jr.Members)
            {
                Models.User m = _Users.FirstOrDefault(u => u.Id == mid);
                if (m != null)
                {
                    m.Room = r;
                    r.Members.Add(m);
                }
            }

            List<Models.User> ws = new List<Models.User>();
            foreach (var wid in jr.Watchers)
            {
                Models.User w = _Users.FirstOrDefault(u => u.Id == wid);
                if (w != null)
                {
                    w.Room = r;
                    r.Watchers.Add(w);
                }
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

                    lock (this._Rooms)
                    {
                        lock (this._Users)
                        {
                            this._Users.Clear();
                            this._Rooms.Clear();
                            this._Users.Add(this._CurentUser);
                            NavigationService.GotoMainPage();
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
                Models.User u = _Users.FirstOrDefault(ur => ur.Id == user.Int);
                lock (this._Users)
                {
                    if (u != null) this.Users.Remove(u);
                    if (u.Room != null)
                    {
                        u.Room.Leaved(u);
                        u.Room.NotWatched(u);
                    }
                    if (u == CurentUser)
                    {
                        this._CurentUser = null;
                        NavigationService.GotoSignInPage();
                    }
                }
            });
        }

        public void OnRaiseSendedMsg(object sender, String json)
        {
            JsonMessageObject msg = JsonConvert.DeserializeObject<JsonMessageObject>(json);
            Dispatcher.Invoke(delegate
            {
                Models.User user = _Users.FirstOrDefault(u => u.Id == msg.UserId);
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
                    Models.Room r = _Rooms.FirstOrDefault(ur => ur.Id == jbo.Int);
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
                    Models.Room r = _Rooms.FirstOrDefault(ur => ur.Id == jru.RoomId);
                    Models.User u = _Users.FirstOrDefault(ur => ur.Id == jru.UserId);
                    if (r != null && u != null)
                    {
                        r.Entered(u);
                        u.Room = r;
                        if (u == CurentUser)
                        {
                            this._CurrentRole = Models.Role.Member;
                            this._CurrentRoom = r;
                            NavigationService.GotoRoomPage();
                        }
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
                    Models.Room r = _Rooms.FirstOrDefault(ur => ur.Id == jru.RoomId);
                    Models.User u = _Users.FirstOrDefault(ur => ur.Id == jru.UserId);
                    if (r != null && u != null)
                    {
                        r.Watched(u);
                        u.Room = r;
                        if (u == CurentUser)
                        {
                            this._CurrentRole = Models.Role.Watcher;
                            this._CurrentRoom = r;
                            NavigationService.GotoRoomPage();
                        }
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
                    Models.Room r = _Rooms.FirstOrDefault(ur => ur.Id == jru.RoomId);
                    Models.User u = _Users.FirstOrDefault(ur => ur.Id == jru.UserId);
                    if (r != null && u != null)
                    {
                        r.Leaved(u);
                        if (u == CurentUser)
                        {
                            this._CurrentRoom = null;
                            NavigationService.GotoMainPage();
                        }
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
                    Models.Room r = _Rooms.FirstOrDefault(ur => ur.Id == jru.RoomId);
                    Models.User u = _Users.FirstOrDefault(ur => ur.Id == jru.UserId);
                    if (r != null && u != null)
                    {
                        r.NotWatched(u);
                        if (u == CurentUser)
                        {
                            this._CurrentRoom = null;
                            NavigationService.GotoMainPage();
                        }
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
                    Models.Room r = _Rooms.FirstOrDefault(ur => ur.Id == jru.RoomId);
                    if (r != null)
                    {
                        r.GameStarted();
                    }
                });
            }
        }
    }
}
