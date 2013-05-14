using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.Json
{
    [JsonObject]
    public class BaseObject
    {
        [JsonProperty("Int")]
        public Int32 Int { get; set; }

        [JsonProperty("String")]
        public String String { get; set; }
    }
}
