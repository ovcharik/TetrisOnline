using System;
using Newtonsoft.Json;

namespace Interface.Json
{
    [JsonObject]
    public class JsonBaseObject
    {
        [JsonProperty("Int")]
        public Int32 Int { get; set; }

        [JsonProperty("String")]
        public String String { get; set; }
    }
}
