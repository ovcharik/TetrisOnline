using Interface;
using Interface.Json;
using Newtonsoft.Json;
using System;

namespace Client.ServerSide
{
    static public class Sender
    {
        private static Connection _Connection { get { return Connection.Instance; } }

        public static void SignIn(String name)
        {
            JsonBaseObject j = new JsonBaseObject
            {
                String = name
            };

            try
            {
                _Connection.ClientSide.Send(Events.SIGN_IN, JsonConvert.SerializeObject(j));
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
                _Connection.ClientSide.Send(Events.SEND_MSG, JsonConvert.SerializeObject(jmo));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
