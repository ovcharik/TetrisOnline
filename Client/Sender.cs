using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft;
using Newtonsoft.Json;
using Interface;
using Interface.Json;

namespace Client
{
    static class Sender
    {
        private static Connection Connection
        {
            get { return Connection.Instance; }
        }

        public static void SignIn(String name)
        {
            JsonBaseObject j = new JsonBaseObject
            {
                String = name
            };

            try
            {
                Connection.Client.Send(Events.SIGN_IN, JsonConvert.SerializeObject(j));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void SendMsg(Int32 id, String data)
        {
            JsonMessageObject jmo = new JsonMessageObject
            {
                UserId = id,
                Data = data
            };

            try
            {
                Connection.Client.Send(Events.SEND_MSG, JsonConvert.SerializeObject(jmo));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
