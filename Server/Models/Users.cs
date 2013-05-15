using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using Interface;
using Interface.Json;

using Newtonsoft;
using Newtonsoft.Json;

namespace Server.Models
{
    class Users
    {
        public Server Server { set; get; }

        private Dictionary<Int32, User> _users;

        private List<JsonBaseObject> _update_list;

        public Users()
        {
            this._users = new Dictionary<Int32, User>();
            this._update_list = new List<JsonBaseObject>();
        }

        public void Add(User user)
        {
            this._users.Add(user.Id, user);
            user.Users = this;
        }

        public void Broadcast(Int32 e, String j, User u = null)
        {
            Console.WriteLine("UserList Broadcast: {0} - {1}", Events.EventToString(e), j);
            foreach (var user in this._users.Values)
            {
                if (user != u) user.SendMessage(e, j);
            }
        }

        public void SignedOut(User user)
        {
            Int32 id = user.Id;
            this._users.Remove(id);
            this._update_list.Remove(this._update_list.Find(delegate(JsonBaseObject u) { return id == u.Int; }));

            if (user.Name != null)
            {
                JsonBaseObject json = new JsonBaseObject();
                json.Int = id;

                String s = JsonConvert.SerializeObject(json);
                this.Broadcast(Events.SIGNED_OUT, s);
            }
            Console.WriteLine("Client {0} dissconected", id);
        }

        public void SignedIn(User user)
        {
            JsonBaseObject json = new JsonBaseObject();
            json.Int = user.Id;
            json.String = user.Name;

            String s = JsonConvert.SerializeObject(json);

            this.Broadcast(Events.SIGNED_IN, s, user);
            user.SendMessage(Events.UPDATE_ID, s);
            user.SendMessage(Events.UPDATE_USER_LIST, JsonConvert.SerializeObject(this._update_list));

            this._update_list.Add(json);
        }
    }
}
