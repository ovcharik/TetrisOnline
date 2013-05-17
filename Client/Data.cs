using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Windows.Threading;

using Newtonsoft;
using Newtonsoft.Json;

using Interface;
using Interface.Json;

namespace Client
{
    public class Data
    {
        Models.User _CurentUser;
        public Models.User CurentUser { get { return this._CurentUser; } }

        ObservableCollection<Models.User> _Users;
        public ObservableCollection<Models.User> Users { get { return _Users; } }

        public Dispatcher Dispatcher { get; set; }

        public Data()
        {
            _CurentUser = null;
            _Users = new ObservableCollection<Models.User>();
        }

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
    }
}
