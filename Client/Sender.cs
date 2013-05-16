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

    }
}
