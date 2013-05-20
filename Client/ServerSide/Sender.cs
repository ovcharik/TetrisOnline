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

            _Connection.ClientSide.Send(Events.SIGN_IN, JsonConvert.SerializeObject(j));
        }

        public static void SendMsg(Int32 id, String data)
        {
            JsonMessageObject jmo = new JsonMessageObject
            {
                UserId = id,
                Data = data
            };

            _Connection.ClientSide.Send(Events.SEND_MSG, JsonConvert.SerializeObject(jmo));
        }

        public static void CreateRoom(String name, Int32 capacity)
        {
            JsonBaseObject jbo = new JsonBaseObject
            {
                String = name,
                Int = capacity
            };

            _Connection.ClientSide.Send(Events.CREATE_ROOM, JsonConvert.SerializeObject(jbo));
        }

    }
}
