using Interface;
using Interface.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Server.Models
{
    class Users
    {
        public Users(Server server)
        {
            this._Users = new Dictionary<Int32, User>();
            this._Server = server;
        }

        // Properties
        private Server _Server;
        public Server Server { get { return this._Server; } }

        private Dictionary<Int32, User> _Users;

        // Public Methods
        public void Add(User user)
        {
            lock (this._Users)
            {
                this._Users.Add(user.Id, user);
            }
            user.Users = this;
        }

        public void Broadcast(Int32 e, String j, User u = null)
        {
            Console.WriteLine("UserList Broadcast: {0} - {1}", Events.EventToString(e), j);
            lock (this._Users)
            {
                foreach (var user in this._Users.Values)
                {
                    if (user != u) user.SendMessage(e, j);
                }
            }
        }

        public void SignedOut(User user)
        {
            Int32 id = user.Id;
            lock (this._Users)
            {
                this._Users.Remove(id);
            }

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
            List<JsonBaseObject> ul = new List<JsonBaseObject>();
            lock (this._Users)
            {
                foreach (var u in this._Users.Values)
                {
                    if (u == user) continue;
                    ul.Add(new JsonBaseObject
                    {
                        Int = u.Id,
                        String = u.Name
                    });
                }
            }
            user.SendMessage(Events.UPDATE_USER_LIST, JsonConvert.SerializeObject(ul));
        }

        public void SendedMsg(User user, JsonMessageObject jmo)
        {
            User u;
            try
            {
                u = this._Users[jmo.UserId];
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("# Error: {0}", e.Message);
                return;
            }
            if (u != null)
            {
                jmo.UserId = user.Id;
                jmo.DateTime = DateTime.Now;
                u.SendMessage(Events.SENDED_MSG, JsonConvert.SerializeObject(jmo));
                Console.WriteLine("{0} ({1}) sended message to {2} ({3})", user.Name, user.Id, u.Name, u.Id);
            }
        }
    }
}
