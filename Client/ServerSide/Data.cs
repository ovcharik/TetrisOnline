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
        }

        // Properties
        private Models.User _CurentUser;
        public Models.User CurentUser { get { return this._CurentUser; } }

        private ObservableCollection<Models.User> _Users;
        public ObservableCollection<Models.User> Users { get { return _Users; } }

        public Dispatcher Dispatcher { get; set; }

        // Server Event Handlers
        public void OnRaiseUpdateId(object sender, String json)
        {
            JsonBaseObject jbo = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            this._CurentUser = new Models.User
            {
                Id = jbo.Int,
                Name = jbo.String
            };
        }

        public void OnRaiseUpdateUserList(object sender, String json)
        {
            List<JsonBaseObject> userList = JsonConvert.DeserializeObject<List<JsonBaseObject>>(json);
            Dispatcher.Invoke(delegate
            {
                this._Users.Clear();
                foreach (var j in userList)
                {
                    this._Users.Add(new Models.User
                    {
                        Id = j.Int,
                        Name = j.String
                    });
                }
            });
        }

        public void OnRaiseSignedIn(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            Dispatcher.Invoke(delegate
            {
                this._Users.Add(new Models.User
                {
                    Id = user.Int,
                    Name = user.String
                });
            });
        }

        public void OnRaiseSignedOut(object sender, String json)
        {
            JsonBaseObject user = JsonConvert.DeserializeObject<JsonBaseObject>(json);
            Dispatcher.Invoke(delegate
            {
                foreach (var u in this._Users)
                {
                    if (u.Id == user.Int)
                    {
                        this._Users.Remove(u);
                        break;
                    }
                }
            });
        }

        public void OnRaiseSendedMsg(object sender, String json)
        {
            JsonMessageObject msg = JsonConvert.DeserializeObject<JsonMessageObject>(json);
            Dispatcher.Invoke(delegate
            {
                Models.User user = null;
                foreach (var u in this._Users)
                {
                    if (u.Id == msg.UserId)
                    {
                        user = u;
                        break;
                    }
                }
                Models.Message m = new Models.Message
                {
                    Data = msg.Data,
                    DateTime = msg.DateTime,
                    Direction = Models.Direction.Input,
                    User = user
                };
                user.AddMessage(m);

                if (user != null)
                {
                    Windows.Messages.Instance.AddUser(user);
                }
            });
        }
    }
}
